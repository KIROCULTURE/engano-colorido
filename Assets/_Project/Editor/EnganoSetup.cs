using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace EnganoColorido.EditorTools
{
    /// Configuración inicial del proyecto. Batch: -executeMethod EnganoColorido.EditorTools.EnganoSetup.Run
    public static class EnganoSetup
    {
        [MenuItem("Tools/Engaño Colorido/Setup Project", priority = 0)]
        public static void Run()
        {
            if (AssetDatabase.FindAssets("t:TMP_Settings").Length == 0)
            {
                TMP_PackageResourceImporter.ImportResources(true, false, false);
                Debug.Log("[Setup] TMP Essential Resources importados.");
            }
            PlayerSettings.companyName = "Liceo";
            PlayerSettings.productName = "Engaño Colorido";
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.runInBackground = true;
            PlayerSettings.visibleInBackground = true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "cl.liceo.enganocolorido");
            foreach (var d in new[] { "Scenes", "Materials", "Resources", "Audio", "Prefabs" })
                Directory.CreateDirectory("Assets/_Project/" + d);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Setup] Listo.");
        }

        /// Batch sin -quit: la importación de TMP es asíncrona, así que salimos al terminar.
        public static void RunBatch()
        {
            bool needTmp = AssetDatabase.FindAssets("t:TMP_Settings").Length == 0;
            if (needTmp)
            {
                AssetDatabase.importPackageCompleted += _ => { Debug.Log("[Setup] TMP importado."); Run(); EditorApplication.Exit(0); };
                AssetDatabase.importPackageFailed += (_, err) => { Debug.LogError("[Setup] TMP falló: " + err); EditorApplication.Exit(1); };
                TMP_PackageResourceImporter.ImportResources(true, false, false);
            }
            else { Run(); EditorApplication.Exit(0); }
        }

        /// Todo en uno para batch: setup + contenido + escenas.
        public static void BatchAll()
        {
            Run();
            ContentImporter.Import();
            GreyboxBuilder.BuildAll();
        }
    }
}
