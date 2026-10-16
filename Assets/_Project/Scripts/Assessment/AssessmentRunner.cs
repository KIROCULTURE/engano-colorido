using System;
using System.Collections.Generic;
using EnganoColorido.Data;
using EnganoColorido.Save;
using EnganoColorido.UI;

namespace EnganoColorido.Assessment
{
    /// Aplica una lista de preguntas de alternativas (diagnóstico o prueba de salida) con orden aleatorio.
    public static class AssessmentRunner
    {
        public static void Run(string titulo, List<Pregunta> preguntas, List<Respuesta> respuestas, Action done, int seed)
        {
            var rng = new Random(seed);
            Step(titulo, preguntas, respuestas, done, rng, 0);
        }

        static void Step(string titulo, List<Pregunta> qs, List<Respuesta> into, Action done, Random rng, int i)
        {
            if (i >= qs.Count) { done?.Invoke(); return; }
            var q = qs[i];
            if (into.Exists(r => r.id == q.id)) { Step(titulo, qs, into, done, rng, i + 1); return; }
            var opts = Shuffle(Options(q), rng);
            if (Core.DevTools.AutoPlay)
            {
                // diagnóstico: primera alternativa; salida: la correcta (para ver una ganancia en el CSV)
                into.Add(Answer(q, q.id.StartsWith("S") ? q.correcta : opts[0]));
                Step(titulo, qs, into, done, rng, i + 1);
                return;
            }
            string ctx = string.IsNullOrEmpty(q.contexto) ? null : ContentDatabase.PoemaTexto(ContentDatabase.Instance.Poema(q.contexto));
            UIManager.I.ShowQuestion($"{titulo} · pregunta {i + 1} de {qs.Count}", q.enunciado, opts, ctx, elegida =>
            {
                into.Add(Answer(q, elegida));
                Step(titulo, qs, into, done, rng, i + 1);
            }, null, null, Tema.App, (float)i / qs.Count);
        }

        public static List<string> Options(Pregunta q)
        {
            var l = new List<string> { q.correcta };
            if (q.distractores != null) l.AddRange(q.distractores);
            return l;
        }

        public static Respuesta Answer(Pregunta q, string elegida) => new Respuesta
        {
            id = q.id, concepto = q.concepto, elegida = elegida, correcta = elegida == q.correcta
        };

        public static List<string> Shuffle(List<string> list, Random rng)
        {
            var l = new List<string>(list);
            for (int i = l.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (l[i], l[j]) = (l[j], l[i]);
            }
            return l;
        }
    }
}
