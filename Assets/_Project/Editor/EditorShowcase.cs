using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EnganoColorido.EditorTools
{
    /// Material para mostrar el proceso: vistas cenitales (planos) de las escenas greybox generadas.
    /// Batch: -executeMethod EnganoColorido.EditorTools.EditorShowcase.RenderPlanos -out <carpeta>
    public static class EditorShowcase
    {
        static readonly string[] Escenas = { "Camarin", "Patio", "Galeria", "Biblioteca", "Teatro" };

        public static void RenderPlanos()
        {
            string outDir = Arg("-out") ?? "Builds/Planos";
            Directory.CreateDirectory(outDir);
            foreach (var nombre in Escenas)
            {
                EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{nombre}.unity");
                foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
                    if (r.name == "Techo" || r.name.StartsWith("MuroS")) r.enabled = false; // sin techo, muro sur abierto
                RenderSettings.fog = false;
                RenderSettings.ambientLight = new Color(0.55f, 0.53f, 0.5f);
                var sol = new GameObject("SolPlano").AddComponent<Light>();
                sol.type = LightType.Directional; sol.intensity = 1.1f; sol.color = new Color(1f, 0.96f, 0.9f);
                sol.transform.eulerAngles = new Vector3(60, 30, 0);

                var b = new Bounds(Vector3.zero, Vector3.zero);
                bool first = true;
                foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
                {
                    if (!r.enabled) continue;
                    if (first) { b = r.bounds; first = false; } else b.Encapsulate(r.bounds);
                }
                var go = new GameObject("CamaraPlano");
                var cam = go.AddComponent<Camera>();
                cam.orthographic = false;
                cam.fieldOfView = 40;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.08f, 0.07f, 0.06f);
                cam.GetUniversalAdditionalCameraData().renderPostProcessing = false;
                float dist = Mathf.Max(b.size.x / 1.6f, b.size.z) * 1.25f + 4f;
                go.transform.position = b.center + new Vector3(0, dist * 0.82f, -dist * 0.58f);
                go.transform.LookAt(b.center);
                var rt = new RenderTexture(1600, 900, 24);
                cam.targetTexture = rt;
                cam.Render();
                RenderTexture.active = rt;
                var tex = new Texture2D(1600, 900, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0);
                tex.Apply();
                File.WriteAllBytes(Path.Combine(outDir, $"plano_{nombre}.png"), tex.EncodeToPNG());
                RenderTexture.active = null;
                Object.DestroyImmediate(rt);
                Debug.Log("[Planos] " + nombre);
            }
        }

        static string Arg(string name)
        {
            var a = System.Environment.GetCommandLineArgs();
            int i = System.Array.IndexOf(a, name);
            return i >= 0 && i + 1 < a.Length ? a[i + 1] : null;
        }
    }
}
