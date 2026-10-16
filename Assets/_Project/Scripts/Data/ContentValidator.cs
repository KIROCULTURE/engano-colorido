using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace EnganoColorido.Data
{
    /// Revisa el contenido editable: así una corrección en el JSON no rompe un puzzle en silencio.
    public static class ContentValidator
    {
        public static readonly string[] PuzzlesModo45 = { "P1", "H", "P3", "P4", "P5", "P7", "JUICIO", "TUVERSO" };

        public static int Palabras(string texto)
        {
            var plain = Regex.Replace(texto ?? "", "<[^>]+>", " ");
            return plain.Split(new[] { ' ', '\n', '\t' }, System.StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static List<string> Check(ContentDatabase db)
        {
            var e = new List<string>();
            foreach (var f in db.fichas.fichas)
                if (Palabras(f.texto) > 80) e.Add($"Ficha {f.id}: {Palabras(f.texto)} palabras (máx. 80).");
            foreach (var id in PuzzlesModo45)
            {
                var p = db.Puzzle(id);
                if (p == null) { e.Add($"Falta el puzzle {id}."); continue; }
                if (p.pistas == null || p.pistas.Length != 3) e.Add($"{id}: necesita 3 pistas.");
                if (p.conceptTags == null || p.conceptTags.Length == 0) e.Add($"{id}: sin conceptTags.");
                foreach (var f in p.fichas ?? new string[0]) if (db.Ficha(f) == null) e.Add($"{id}: la ficha «{f}» no existe.");
                foreach (var paso in p.pasos)
                {
                    if (paso.tipo == "pregunta")
                    {
                        if (string.IsNullOrEmpty(paso.correcta)) e.Add($"{id}/{paso.id}: pregunta sin respuesta correcta.");
                        continue;
                    }
                    if (paso.tipo == "abba") continue;
                    foreach (var s in paso.slots)
                        if (!ContentDatabase.Opciones(paso, s).Contains(s.correcta)) e.Add($"{id}/{paso.id}: «{s.correcta}» no está entre las opciones de «{s.etiqueta}».");
                }
            }
            foreach (var q in db.evaluacion.diagnostico) CheckQ(q, e);
            foreach (var q in db.evaluacion.salida) CheckQ(q, e);
            return e;
        }

        static void CheckQ(Pregunta q, List<string> e)
        {
            if (q.distractores == null || q.distractores.Length < 2) e.Add($"{q.id}: necesita al menos 2 distractores.");
            else if (System.Array.IndexOf(q.distractores, q.correcta) >= 0) e.Add($"{q.id}: la correcta aparece como distractor.");
        }
    }
}
