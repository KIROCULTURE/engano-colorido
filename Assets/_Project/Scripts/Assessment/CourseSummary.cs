using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EnganoColorido.Assessment
{
    /// Resumen del curso por concepto, juntando los CSV exportados en una misma carpeta.
    public static class CourseSummary
    {
        public static string Build(string folder)
        {
            if (!Directory.Exists(folder)) return "No existe la carpeta:\n" + folder;
            var files = Directory.GetFiles(folder, "EC_*.csv");
            if (files.Length == 0) return "Aún no hay resultados exportados en:\n" + folder;
            string[] conceptos = Scoring.Conceptos;
            var dOk = new int[5]; var sOk = new int[5];
            int n = 0, transf = 0, gan = 0;
            var finales = new Dictionary<string, int>();
            foreach (var f in files)
            {
                var lines = File.ReadAllLines(f, Encoding.UTF8);
                if (lines.Length < 2) continue;
                var h = new List<string>(lines[0].TrimStart('﻿').Split(Exporter.Sep));
                var c = lines[1].Split(Exporter.Sep);
                if (c.Length < h.Count - 2) continue;
                int Col(string name) { int i = h.IndexOf(name); return i >= 0 && i < c.Length && int.TryParse(c[i], out int v) ? v : 0; }
                for (int k = 0; k < 5; k++) { dOk[k] += Col("D" + (k + 1)); sOk[k] += Col("S" + (k + 1)); }
                transf += Col("transferencia_S6_S8");
                gan += Col("ganancia");
                int fi = h.IndexOf("final");
                string fin = fi >= 0 && fi < c.Length && c[fi] != "" ? c[fi] : "sin_elegir";
                finales[fin] = finales.TryGetValue(fin, out int x) ? x + 1 : 1;
                n++;
            }
            if (n == 0) return "Los archivos no tienen el formato esperado.";
            var sb = new StringBuilder($"<b>{n} sesiones</b> en {folder}\n\n<b>Logro por concepto</b> (diagnóstico → salida)\n");
            for (int k = 0; k < 5; k++)
                sb.Append($"• {conceptos[k]}: {100 * dOk[k] / n}% → {100 * sOk[k] / n}%\n");
            sb.Append($"\n<b>Transferencia</b> (Quevedo, S6–S8): {(float)transf / n:0.0} de 3 en promedio\n");
            sb.Append($"<b>Ganancia promedio</b> (S1–S5 − D1–D5): {(float)gan / n:+0.0;-0.0;0}\n\n<b>Finales elegidos</b>\n");
            foreach (var kv in finales) sb.Append($"• {kv.Key}: {kv.Value}\n");
            return sb.ToString();
        }
    }
}
