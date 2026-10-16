using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnganoColorido.Core
{
    /// Solo para verificar builds sin intervención (no aparece en el juego):
    ///   -devscene Patio      abre la escena en recorrido libre
    ///   -devshot salida.png  saca una captura a los 4 s y cierra
    ///   -devyaw 90           gira al jugador antes de la captura
    public class DevTools : MonoBehaviour
    {
        public static string Arg(string name)
        {
            var a = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(a, name);
            return i >= 0 && i + 1 < a.Length ? a[i + 1] : null;
        }

        public static bool Has(string name) => Array.IndexOf(Environment.GetCommandLineArgs(), name) >= 0;

        public static void Init()
        {
            if (Arg("-devscene") == null && Arg("-devshot") == null) return;
            var go = new GameObject("DevTools");
            DontDestroyOnLoad(go);
            go.AddComponent<DevTools>();
        }

        IEnumerator Start()
        {
            yield return null;
            var scene = Arg("-devscene");
            if (scene != null && SceneManager.GetActiveScene().name != scene)
            {
                GameFlow.I.StartFreeRoam(scene);
                yield return new WaitForSeconds(2.5f);
            }
            if (Arg("-devyaw") is string yaw && Player.PlayerController.I != null)
                Player.PlayerController.I.transform.rotation = Quaternion.Euler(0, float.Parse(yaw, System.Globalization.CultureInfo.InvariantCulture), 0);
            var shot = Arg("-devshot");
            if (shot == null) yield break;
            yield return new WaitForSeconds(4f);
            if (Has("-devclose")) UI.UIManager.I.CloseAllModals();
            yield return new WaitForSeconds(0.5f);
            ScreenCapture.CaptureScreenshot(Path.GetFullPath(shot));
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }
    }
}
