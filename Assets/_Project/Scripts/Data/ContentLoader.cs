using System.IO;
using UnityEngine;

namespace EnganoColorido.Data
{
    /// Lee Data/*.json y llena un ContentDatabase. Lo usan el importador del editor y los tests.
    public static class ContentLoader
    {
        public static readonly string[] Archivos = { "poems", "fichas", "topicos", "puzzles", "assessment", "dialogue", "session" };

        public static ContentDatabase Load(string folder, ContentDatabase db = null)
        {
            if (db == null) db = ScriptableObject.CreateInstance<ContentDatabase>();
            db.poemas = Read(folder, "poems", db.poemas);
            db.fichas = Read(folder, "fichas", db.fichas);
            db.topicos = Read(folder, "topicos", db.topicos);
            db.puzzles = Read(folder, "puzzles", db.puzzles);
            db.evaluacion = Read(folder, "assessment", db.evaluacion);
            db.dialogos = Read(folder, "dialogue", db.dialogos);
            db.sesion = Read(folder, "session", db.sesion);
            return db;
        }

        static T Read<T>(string folder, string name, T fallback)
        {
            var path = Path.Combine(folder, name + ".json");
            if (!File.Exists(path)) return fallback;
            var obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
            if (obj == null) throw new System.Exception("JSON inválido: " + path);
            return obj;
        }
    }
}
