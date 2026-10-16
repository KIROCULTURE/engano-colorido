using System.Collections.Generic;
using EnganoColorido.Save;

namespace EnganoColorido.Assessment
{
    /// Puntajes del diagnóstico (D1–D5) y la prueba de salida (S1–S8). Ganancia = S1–S5 − D1–D5.
    public static class Scoring
    {
        public static readonly string[] Conceptos = { "hablante", "connotacion", "soneto", "topicos", "simbolos" };

        public static int Correctas(List<Respuesta> r, int desde = 1, int hasta = 99)
        {
            int n = 0;
            foreach (var x in r)
            {
                int num = Num(x.id);
                if (x.correcta && num >= desde && num <= hasta) n++;
            }
            return n;
        }

        public static int Diagnostico(SaveData s) => Correctas(s.diagnostico);
        public static int Salida15(SaveData s) => Correctas(s.salida, 1, 5);
        public static int Transferencia(SaveData s) => Correctas(s.salida, 6, 8);
        public static int Ganancia(SaveData s) => Salida15(s) - Diagnostico(s);

        static int Num(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length < 2) return 0;
            int.TryParse(id.Substring(1), out int n);
            return n;
        }

        public static bool? Acierto(List<Respuesta> r, string id)
        {
            var x = r.Find(a => a.id == id);
            return x == null ? (bool?)null : x.correcta;
        }
    }
}
