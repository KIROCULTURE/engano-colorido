using System;
using System.Globalization;
using System.IO;
using System.Text;
using EnganoColorido.Save;
using UnityEngine;

namespace EnganoColorido.Assessment
{
    /// Exporta la sesión a CSV (UTF-8 con BOM, separador «;», para Excel en Chile) y JSON. Solo local.
    public static class Exporter
    {
        public const char Sep = ';';
        public static readonly string[] Puzzles = { "P1", "H", "P3", "P4", "P5", "P7", "JUICIO", "TUVERSO" };
        static readonly string[] D = { "D1", "D2", "D3", "D4", "D5" };
        static readonly string[] S = { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };

        public static string Header()
        {
            var sb = new StringBuilder("alias;curso;modo;fecha");
            foreach (var d in D) sb.Append(Sep).Append(d);
            foreach (var s in S) sb.Append(Sep).Append(s);
            sb.Append(";puntaje_diagnostico;puntaje_salida_S1_S5;transferencia_S6_S8;ganancia;final;foto_publicada;verso_modo;verso");
            foreach (var p in Puzzles) sb.Append($";{p}_intentos;{p}_pistas;{p}_segundos;{p}_resuelto;{p}_con_ayuda");
            sb.Append(";zonas_omitidas;duracion_min");
            return sb.ToString();
        }

        public static string Row(SaveData s)
        {
            var inv = CultureInfo.InvariantCulture;
            var sb = new StringBuilder();
            sb.Append(Cell(s.alias)).Append(Sep).Append(Cell(s.curso)).Append(Sep).Append(s.modo).Append(Sep).Append(Cell(s.fecha));
            foreach (var d in D) sb.Append(Sep).Append(Bit(Scoring.Acierto(s.diagnostico, d)));
            foreach (var x in S) sb.Append(Sep).Append(Bit(Scoring.Acierto(s.salida, x)));
            sb.Append(Sep).Append(Scoring.Diagnostico(s))
              .Append(Sep).Append(Scoring.Salida15(s))
              .Append(Sep).Append(Scoring.Transferencia(s))
              .Append(Sep).Append(Scoring.Ganancia(s))
              .Append(Sep).Append(Cell(s.final))
              .Append(Sep).Append(Cell(s.foto))
              .Append(Sep).Append(Cell(s.versoModo))
              .Append(Sep).Append(Cell(string.Join(" / ", s.verso)));
            foreach (var p in Puzzles)
            {
                var l = s.puzzles.Find(x => x.id == p);
                if (l == null) { sb.Append(";;;;;"); continue; }
                sb.Append(Sep).Append(l.intentos).Append(Sep).Append(l.pistas)
                  .Append(Sep).Append(Mathf.RoundToInt(l.segundos).ToString(inv))
                  .Append(Sep).Append(l.resuelto ? 1 : 0).Append(Sep).Append(l.conAyuda ? 1 : 0);
            }
            sb.Append(Sep).Append(Cell(string.Join(" ", s.zonasOmitidas)));
            sb.Append(Sep).Append((s.elapsed / 60f).ToString("0.0", inv));
            return sb.ToString();
        }

        static string Bit(bool? b) => b == null ? "" : b.Value ? "1" : "0";

        /// Celda segura: comillas si hace falta y sin fórmulas (evita inyección en Excel).
        public static string Cell(string v)
        {
            if (string.IsNullOrEmpty(v)) return "";
            v = v.Replace("\r", " ").Replace("\n", " ");
            if ("=+-@".IndexOf(v[0]) >= 0) v = "'" + v;
            if (v.IndexOfAny(new[] { Sep, '"' }) >= 0) v = "\"" + v.Replace("\"", "\"\"") + "\"";
            return v;
        }

        public static string Csv(SaveData s) => Header() + "\r\n" + Row(s) + "\r\n";
        public static string Json(SaveData s) => JsonUtility.ToJson(s, true);

        public static string SafeName(string s)
        {
            if (string.IsNullOrEmpty(s)) return "sin_alias";
            var sb = new StringBuilder();
            foreach (var c in s) sb.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            return sb.ToString();
        }

        /// Escribe los dos archivos y devuelve la ruta del CSV.
        public static string Export(SaveData s, string folder)
        {
            Directory.CreateDirectory(folder);
            string name = $"EC_{SafeName(s.curso)}_{SafeName(s.alias)}_{DateTime.Now:yyyyMMdd_HHmmss}";
            string csv = Path.Combine(folder, name + ".csv");
            File.WriteAllText(csv, Csv(s), new UTF8Encoding(true));
            File.WriteAllText(Path.Combine(folder, name + ".json"), Json(s), new UTF8Encoding(false));
            return csv;
        }
    }
}
