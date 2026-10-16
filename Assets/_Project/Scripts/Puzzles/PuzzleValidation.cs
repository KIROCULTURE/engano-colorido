using System.Collections.Generic;
using EnganoColorido.Data;

namespace EnganoColorido.Puzzles
{
    /// Validadores puros de cada tipo de paso (sin Unity): se testean en EditMode.
    public static class PuzzleValidation
    {
        public static bool Validate(Paso p, IList<string> answers, out string feedback)
        {
            feedback = null;
            switch (p.tipo)
            {
                case "pregunta": return answers.Count > 0 && Question(p, answers[0]);
                case "abba": return Abba(p, answers, out feedback);
                case "juicio": return Juicio(p, answers, out feedback);
                default:
                    if (p.estante)
                    {
                        var wrong = WrongGroups(p, answers);
                        if (wrong.Count > 0) feedback = "Repisas con errores: " + string.Join(", ", wrong) + ".";
                        return wrong.Count == 0;
                    }
                    return Assign(p, answers);
            }
        }

        public static bool Question(Paso p, string answer) => answer == p.correcta;

        public static bool Assign(Paso p, IList<string> answers)
        {
            if (answers == null || answers.Count != p.slots.Length) return false;
            for (int i = 0; i < p.slots.Length; i++) if (answers[i] != p.slots[i].correcta) return false;
            return true;
        }

        /// Grupos (repisas) que tienen al menos un error. Vacío = todo bien.
        public static List<int> WrongGroups(Paso p, IList<string> answers)
        {
            var wrong = new List<int>();
            for (int i = 0; i < p.slots.Length; i++)
            {
                bool ok = answers != null && i < answers.Count && answers[i] == p.slots[i].correcta;
                if (!ok && !wrong.Contains(p.slots[i].grupo)) wrong.Add(p.slots[i].grupo);
            }
            return wrong;
        }

        /// Tu verso: cuatro versos distintos con rima abba (1 con 4, 2 con 3, y a ≠ b).
        public static bool Abba(Paso p, IList<string> a, out string feedback)
        {
            feedback = null;
            if (a == null || a.Count != 4) { feedback = "Faltan versos."; return false; }
            var t = new string[4];
            for (int i = 0; i < 4; i++)
            {
                if (string.IsNullOrEmpty(a[i])) { feedback = "Completa los cuatro versos."; return false; }
                for (int j = 0; j < i; j++) if (a[j] == a[i]) { feedback = "No repitas el mismo verso."; return false; }
                t[i] = ContentDatabase.Tag(p, a[i]);
            }
            for (int i = 0; i < 4; i++)
                if (string.IsNullOrEmpty(t[i])) { feedback = $"«{a[i]}» no rima con ningún otro verso del banco."; return false; }
            if (t[0] != t[3]) { feedback = "El verso 1 debe rimar con el verso 4 (a … a)."; return false; }
            if (t[1] != t[2]) { feedback = "El verso 2 debe rimar con el verso 3 (b … b)."; return false; }
            if (t[0] == t[1]) { feedback = "En abba, la rima a y la rima b tienen que ser distintas."; return false; }
            return true;
        }

        /// Juicio: conector + evidencia + conector. El feedback nombra la categoría del conector.
        public static bool Juicio(Paso p, IList<string> a, out string feedback)
        {
            feedback = null;
            if (a == null || a.Count != p.slots.Length) { feedback = "Completa los tres huecos."; return false; }
            for (int i = 0; i < p.slots.Length; i++)
            {
                if (a[i] == p.slots[i].correcta) continue;
                string tagElegido = ContentDatabase.Tag(p, a[i]);
                string tagEsperado = ContentDatabase.Tag(p, p.slots[i].correcta);
                if (tagEsperado == "evidencia")
                    feedback = "Ese verso es real, pero no responde a la tesis de El Retrato.";
                else if (tagElegido == tagEsperado)
                    feedback = $"«{a[i]}» es de la categoría correcta, pero no calza con la puntuación de la frase.";
                else
                    feedback = $"«{a[i]}» {Que(tagElegido)}, pero aquí se necesita un conector que {Que(tagEsperado)}.";
                return false;
            }
            return true;
        }

        public static string Que(string tag) => tag switch
        {
            "adversativo" => "opone ideas (adversativo)",
            "consecutivo" => "introduce una consecuencia (consecutivo)",
            "ordenacion" => "ordena o enumera ideas (de ordenación)",
            _ => "no es un conector"
        };

        /// Solución correcta de un paso (modo guiado y tests).
        public static List<string> Solution(Paso p)
        {
            var l = new List<string>();
            if (p.tipo == "pregunta") { l.Add(p.correcta); return l; }
            if (p.tipo == "abba")
            {
                // primer par de rimas distintas del banco: a b b a
                string a1 = null, a2 = null, b1 = null, b2 = null, ta = null;
                foreach (var o in p.opciones)
                {
                    if (string.IsNullOrEmpty(o.tag)) continue;
                    if (ta == null) ta = o.tag;
                    if (o.tag == ta) { if (a1 == null) a1 = o.texto; else if (a2 == null) a2 = o.texto; }
                    else { if (b1 == null) b1 = o.texto; else if (b2 == null) b2 = o.texto; }
                }
                l.AddRange(new[] { a1, b1, b2, a2 });
                return l;
            }
            foreach (var s in p.slots) l.Add(s.correcta);
            return l;
        }
    }
}
