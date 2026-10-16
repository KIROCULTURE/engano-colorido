using EnganoColorido.Data;
using UnityEditor;
using UnityEngine;

namespace EnganoColorido.EditorTools
{
    /// Convierte Assets/_Project/Data/*.json en Resources/ContentDatabase.asset.
    public static class ContentImporter
    {
        public const string DataFolder = "Assets/_Project/Data";
        public const string AssetPath = "Assets/_Project/Resources/ContentDatabase.asset";

        [MenuItem("Tools/Engaño Colorido/Import Content", priority = 10)]
        public static void Import()
        {
            var db = AssetDatabase.LoadAssetAtPath<ContentDatabase>(AssetPath);
            bool nuevo = db == null;
            if (nuevo) db = ScriptableObject.CreateInstance<ContentDatabase>();
            ContentLoader.Load(DataFolder, db);
            if (nuevo) AssetDatabase.CreateAsset(db, AssetPath);
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Import Content] {db.fichas.fichas.Count} fichas · {db.poemas.poemas.Count} poemas · {db.puzzles.puzzles.Count} puzzles · " +
                      $"{db.evaluacion.diagnostico.Count}+{db.evaluacion.salida.Count} preguntas · {db.dialogos.dialogos.Count} diálogos");
        }
    }
}
