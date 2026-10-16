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
    ///   -autoplay            juega la sesión completa (modo guiado en todo), exporta y cierra
    public class DevTools : MonoBehaviour
    {
        public static bool AutoPlay { get; private set; }

        public static string Arg(string name)
        {
            var a = Environment.GetCommandLineArgs();
            int i = Array.IndexOf(a, name);
            return i >= 0 && i + 1 < a.Length ? a[i + 1] : null;
        }

        public static bool Has(string name) => Array.IndexOf(Environment.GetCommandLineArgs(), name) >= 0;

        public static void Init()
        {
            AutoPlay = Has("-autoplay");
            if (Arg("-exportdir") is string dir) Settings.ExportOverride = dir;
            if (Arg("-devscene") == null && Arg("-devshot") == null && !AutoPlay) return;
            var go = new GameObject("DevTools");
            DontDestroyOnLoad(go);
            go.AddComponent<DevTools>();
        }

        IEnumerator Start()
        {
            yield return null;
            if (AutoPlay) { StartCoroutine(Auto()); yield break; }
            var scene = Arg("-devscene");
            if (scene != null && SceneManager.GetActiveScene().name != scene)
            {
                GameFlow.I.StartFreeRoam(scene);
                yield return new WaitForSeconds(2.5f);
            }
            if (Arg("-devyaw") is string yaw && Player.PlayerController.I != null)
                Player.PlayerController.I.transform.rotation = Quaternion.Euler(0, float.Parse(yaw, System.Globalization.CultureInfo.InvariantCulture), 0);
            if (Has("-devclose")) { yield return new WaitForSeconds(1f); UI.UIManager.I.CloseAllModals(); yield return null; }
            switch (Arg("-devui"))
            {
                case "alias": UI.NewSessionPanel.Show(); break;
                case "docente": UI.TeacherMenu.Show(); break;
                case "celular": UI.PhoneUI.Open(Arg("-devtab") ?? "Mapa"); break;
                case "puzzle":
                    foreach (var pc in FindObjectsByType<Puzzles.PuzzleController>(FindObjectsSortMode.None))
                        if (pc.puzzleId == Arg("-devpuzzle"))
                        {
                            int step = int.Parse(Arg("-devstep") ?? "0");
                            Puzzles.AssignmentPanel.Show(pc, Game.DB.Puzzle(pc.puzzleId).pasos[step], step);
                        }
                    break;
                case "pregunta":
                    var q = Game.DB.evaluacion.salida[5];
                    UI.UIManager.I.ShowQuestion("Repasa+ · Prueba de salida · pregunta 6 de 8", q.enunciado, Assessment.AssessmentRunner.Options(q),
                        Data.ContentDatabase.PoemaTexto(Game.DB.Poema(q.contexto)), null);
                    break;
            }
            var shot = Arg("-devshot");
            if (shot == null) yield break;
            yield return new WaitForSeconds(4f);
            yield return new WaitForSeconds(0.5f);
            ScreenCapture.CaptureScreenshot(Path.GetFullPath(shot));
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }

        // ---------------- autoplay: recorre el modo completo usando el modo guiado
        IEnumerator Auto()
        {
            Debug.Log("[AUTOPLAY] inicio");
            GameFlow.SceneReady += _ => StartCoroutine(SolveZone());
            yield return new WaitForSeconds(1f);
            GameFlow.I.StartNewSession("autotest");
        }

        IEnumerator SolveZone()
        {
            yield return new WaitForSeconds(1.5f);
            var z = World.ZoneController.I;
            if (z == null) yield break;
            Debug.Log("[AUTOPLAY] zona " + string.Join(",", z.zonas));
            int guard = 0;
            while (!z.AllDone && guard++ < 40)
            {
                var t = z.CurrentTask;
                Debug.Log("[AUTOPLAY]   tarea " + t.name);
                t.ForceComplete();
                yield return new WaitForSeconds(0.6f);
                if (z == null) yield break; // la tarea cambió de escena o terminó la sesión
            }
            if (z != null && World.ZoneController.I == z && GameFlow.InGameplay) GameFlow.I.ExitScene();
        }

        public static void AutoFinish(string csv)
        {
            Debug.Log("[AUTOPLAY] OK " + csv);
            Application.Quit();
        }
    }
}
