using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EnganoColorido.Save
{
    [Serializable] public class Intento
    {
        public string paso;
        public bool correcto;
        public float t;
    }

    [Serializable] public class PuzzleLog
    {
        public string id;
        public string[] conceptTags;
        public int intentos;
        public int pistas;
        public float segundos;
        public bool resuelto;
        public bool conAyuda;
        public List<Intento> registro = new();
    }

    [Serializable] public class Respuesta
    {
        public string id;
        public string concepto;
        public string elegida;
        public bool correcta;
    }

    /// Todo lo que se guarda de una sesión (local, sin red). Solo alias, nunca nombre completo.
    [Serializable] public class SaveData
    {
        public string alias;
        public string curso;
        public int modo = 45;
        public string fecha;
        public int dificultad;

        public float elapsed;        // reloj de sesión (s)
        public string zona = "diagnostico";
        public List<string> zonasCompletas = new();
        public List<string> zonasOmitidas = new();

        public List<string> inventario = new();
        public List<string> fichas = new();
        public List<string> poemas = new();
        public List<string> historias = new();
        public List<string> notas = new();
        public List<string> flags = new();
        public float tiempo = 100f;  // reloj de arena

        public List<PuzzleLog> puzzles = new();
        public List<Respuesta> diagnostico = new();
        public List<Respuesta> salida = new();

        public string final = "";
        public string foto = "";
        public string versoModo = "";
        public List<string> verso = new();
        public bool terminada;

        public PuzzleLog Log(string id, string[] tags = null)
        {
            var l = puzzles.Find(p => p.id == id);
            if (l == null) { l = new PuzzleLog { id = id, conceptTags = tags ?? new string[0] }; puzzles.Add(l); }
            return l;
        }

        public bool Solved(string id) { var l = puzzles.Find(p => p.id == id); return l != null && l.resuelto; }
        public bool Flag(string f) => flags.Contains(f);
        public void SetFlag(string f) { if (!flags.Contains(f)) flags.Add(f); }
        public void AddUnique(List<string> list, string v) { if (!string.IsNullOrEmpty(v) && !list.Contains(v)) list.Add(v); }
    }

    public static class SaveSystem
    {
        static string _folder;
        public static string Folder { get => _folder ??= Application.persistentDataPath; set => _folder = value; }
        public static string PathFile => System.IO.Path.Combine(Folder, "partida.json");

        public static bool HasSave()
        {
            try { return File.Exists(PathFile) && Load() is { terminada: false }; } catch { return false; }
        }

        public static void Save(SaveData d)
        {
            Directory.CreateDirectory(Folder);
            var tmp = PathFile + ".tmp";
            File.WriteAllText(tmp, ToJson(d));
            if (File.Exists(PathFile)) File.Delete(PathFile);
            File.Move(tmp, PathFile);
        }

        public static SaveData Load() => File.Exists(PathFile) ? FromJson(File.ReadAllText(PathFile)) : null;
        public static void Delete() { if (File.Exists(PathFile)) File.Delete(PathFile); }

        public static string ToJson(SaveData d) => JsonUtility.ToJson(d, true);
        public static SaveData FromJson(string json) => JsonUtility.FromJson<SaveData>(json);
    }
}
