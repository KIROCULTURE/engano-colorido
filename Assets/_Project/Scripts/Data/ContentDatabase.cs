using System.Collections.Generic;
using UnityEngine;

namespace EnganoColorido.Data
{
    /// ScriptableObject generado por Tools/Engaño Colorido/Import Content desde Data/*.json.
    public class ContentDatabase : ScriptableObject
    {
        public FichasFile fichas = new();
        public PoemasFile poemas = new();
        public TopicosFile topicos = new();
        public PuzzlesFile puzzles = new();
        public DialogosFile dialogos = new();
        public EvaluacionFile evaluacion = new();
        public SesionConfig sesion = new();

        static ContentDatabase _instance;
        public static ContentDatabase Instance
        {
            get
            {
                if (_instance == null) _instance = Resources.Load<ContentDatabase>("ContentDatabase");
                if (_instance == null)
                {
                    Debug.LogError("Falta Resources/ContentDatabase. Usa Tools/Engaño Colorido/Import Content.");
                    _instance = CreateInstance<ContentDatabase>();
                }
                return _instance;
            }
        }

        public Ficha Ficha(string id) => fichas.fichas.Find(f => f.id == id);
        public Poema Poema(string id) => poemas.poemas.Find(p => p.id == id) ?? (evaluacion.poemaSalida != null && evaluacion.poemaSalida.id == id ? evaluacion.poemaSalida : null);
        public Puzzle Puzzle(string id) => puzzles.puzzles.Find(p => p.id == id);
        public Dialogo Dialogo(string id) => dialogos.dialogos.Find(d => d.id == id);
        public ZonaInfo Zona(string id) => System.Array.Find(sesion.zonas ?? new ZonaInfo[0], z => z.id == id);

        public Modo Modo(int minutos)
        {
            foreach (var m in sesion.modos ?? new Modo[0]) if (m.minutos == minutos) return m;
            return null;
        }

        public static string PoemaTexto(Poema p, bool conTitulo = true)
        {
            if (p == null) return "";
            var sb = new System.Text.StringBuilder();
            if (conTitulo) sb.Append("<b>").Append(p.titulo).Append("</b>\n<i>").Append(p.autor).Append("</i>  ·  p. ").Append(p.pagina).Append("\n\n");
            foreach (var v in p.versos) sb.Append(v).Append('\n');
            if (!string.IsNullOrEmpty(p.glosario)) sb.Append("\n<size=80%><color=#A09A90>Glosario: ").Append(p.glosario).Append("</color></size>");
            return sb.ToString();
        }

        public static List<string> Opciones(Paso paso, Slot slot)
        {
            var list = new List<string>();
            if (slot.opciones != null && slot.opciones.Length > 0) list.AddRange(slot.opciones);
            else if (paso.opciones != null) foreach (var o in paso.opciones) list.Add(o.texto);
            return list;
        }

        public static string Tag(Paso paso, string texto)
        {
            if (paso.opciones == null) return null;
            foreach (var o in paso.opciones) if (o.texto == texto) return o.tag;
            return null;
        }
    }
}
