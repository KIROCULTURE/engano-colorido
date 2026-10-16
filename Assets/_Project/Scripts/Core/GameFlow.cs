using System;
using System.Collections.Generic;
using EnganoColorido.Assessment;
using EnganoColorido.Save;
using EnganoColorido.Session;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnganoColorido.Core
{
    /// Orquesta la sesión: menú → diagnóstico → zonas → Juicio → prueba de salida → export.
    /// El control de ritmo (GDD 4.3) vive en SessionManager; aquí se aplican sus decisiones.
    public class GameFlow : MonoBehaviour
    {
        public static GameFlow I { get; private set; }
        public static bool InGameplay { get; private set; }
        public const string MenuScene = "Arranque";

        public SessionPacer Pacer => SessionManager.Pacer;
        public bool FreeRoam { get; private set; }
        bool exitStarted;
        public static event Action<string> SceneReady; // para autoplay

        public static GameFlow Ensure()
        {
            if (I != null) return I;
            var go = new GameObject("GameFlow");
            DontDestroyOnLoad(go);
            go.AddComponent<SessionManager>();
            return go.AddComponent<GameFlow>();
        }

        void Awake() { I = this; SceneManager.sceneLoaded += OnSceneLoaded; }
        void OnDestroy() { SceneManager.sceneLoaded -= OnSceneLoaded; }

        void Start()
        {
            UIManager.I.SetUIScale(Settings.TamanoTexto);
            if (SceneManager.GetActiveScene().name == MenuScene) ShowMenu();
            else if (Game.State == null)
            {
                // Escena de zona abierta directo en el editor: sesión de prueba.
                Game.State = NewState("editor");
                FreeRoam = true;
            }
        }

        public static SaveData NewState(string alias) => new SaveData
        {
            alias = alias,
            curso = Settings.Curso,
            modo = Settings.Modo,
            dificultad = Settings.Dificultad,
            fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            inventario = { "celular" }
        };

        // ------------------------------------------------------------ menú
        public void ShowMenu()
        {
            InGameplay = false;
            UIManager.I.CloseAllModals();
            UIManager.I.SetHudVisible(false);
            MainMenu.Show();
        }

        public void ReturnToMenu()
        {
            InGameplay = false;
            SessionManager.Stop();
            UIManager.I.CloseAllModals();
            UIManager.I.Fade(true, 0.5f, () => SceneManager.LoadScene(MenuScene));
        }

        public void StartFreeRoam(string scene)
        {
            Game.State = NewState("recorrido");
            FreeRoam = true;
            SessionManager.Stop();
            LoadScene(scene);
        }

        // ------------------------------------------------------------ sesión
        public void StartNewSession(string alias)
        {
            Game.State = NewState(alias);
            FreeRoam = false;
            exitStarted = false;
            SessionManager.Begin(Game.State.modo, 0);
            Pacer.EnterZone("diagnostico");
            Game.State.zona = "diagnostico";
            UIManager.I.SetHudVisible(false);
            UIManager.I.PlayDialogue("diag_intro", () =>
                AssessmentRunner.Run("Repasa+ · Diagnóstico", Game.DB.evaluacion.diagnostico, Game.State.diagnostico, () =>
                    UIManager.I.PlayDialogue("diag_fin", () => CompleteZones(new[] { "diagnostico" })), Seed()));
        }

        int Seed() => (Game.State.alias + Game.State.fecha).GetHashCode();

        public void Continue()
        {
            var s = SaveSystem.Load();
            if (s == null) { UIManager.I.Toast("No hay partida guardada."); return; }
            Game.State = s;
            FreeRoam = false;
            exitStarted = false;
            SessionManager.Begin(s.modo, s.elapsed);
            GoToZone(s.zona);
        }

        /// Marca zonas como completas, decide la siguiente (saltando las omitidas), autoguarda y avanza.
        public void CompleteZones(string[] zonas)
        {
            var st = Game.State;
            foreach (var id in zonas) st.AddUnique(st.zonasCompletas, id);
            if (FreeRoam || Pacer == null) { EndFreeRoam(); return; }
            var skipped = new List<string>();
            string next = Pacer.NextZone(zonas[zonas.Length - 1], skipped);
            var missing = new List<string>();
            // Zonas cuya escena no está en esta build (builds de avance): se omiten sin romper la sesión.
            while (next != "salida" && !SceneAvailable(Pacer.SceneFor(next)))
            {
                missing.Add(next);
                next = Pacer.NextZone(next, skipped);
            }
            foreach (var z in skipped) Omit(z, Pacer.InRoute(z) ? "por tiempo" : $"no se visita en el modo {st.modo}");
            foreach (var z in missing) Omit(z, "aún no está disponible en esta build");
            st.zona = next;
            SaveNow();
            GoToZone(next);
        }

        void Omit(string zone, string why)
        {
            var st = Game.State;
            if (st.zonasOmitidas.Contains(zone)) return;
            st.zonasOmitidas.Add(zone);
            st.notas.Add($"Se omitió {Game.DB.Zona(zone)?.nombre ?? zone}: {why}.");
        }

        static bool SceneAvailable(string scene) => !string.IsNullOrEmpty(scene) && Application.CanStreamedLevelBeLoaded(scene);

        public void GoToZone(string zone)
        {
            if (zone == "salida" || zone == "diagnostico" && Game.State.diagnostico.Count >= 5) { StartExitTest(); return; }
            if (zone == "diagnostico") { StartNewSession(Game.State.alias); return; }
            Pacer.EnterZone(zone);
            var scene = Pacer.SceneFor(zone);
            if (SceneManager.GetActiveScene().name == scene && ZoneController.I != null) return; // sub-zona en la misma escena
            LoadScene(scene);
        }

        /// Sub-zonas de una escena (Patio → Radios): cambia el presupuesto sin cargar escena.
        public void AdvanceSubZone(string done, string next)
        {
            Game.State.AddUnique(Game.State.zonasCompletas, done);
            Game.State.zona = next;
            Pacer?.EnterZone(next);
            SaveNow();
        }

        void EndFreeRoam()
        {
            SaveNow();
            UIManager.I.ShowText("Fin del recorrido",
                "Terminaste la zona.\n\n<color=#A09A90>En una sesión real aquí se autoguarda y el juego sigue a la siguiente zona.</color>",
                "Volver al menú", ReturnToMenu);
        }

        // ------------------------------------------------------------ escenas
        public void LoadScene(string scene)
        {
            InGameplay = false;
            UIManager.I.CloseAllModals();
            UIManager.I.SetHudVisible(false);
            UIManager.I.Fade(true, 0.6f, () => SceneManager.LoadScene(scene));
        }

        void OnSceneLoaded(Scene s, LoadSceneMode m)
        {
            if (s.name == MenuScene)
            {
                UIManager.I.Fade(false, 0.6f);
                ShowMenu();
            }
        }

        /// Lo llama el ZoneController de cada escena al iniciar.
        public void OnZoneLoaded(ZoneController z)
        {
            UIManager.I.Fade(false, 0.8f);
            UIManager.I.SetHudVisible(true);
            InGameplay = true;
            if (Pacer != null && Array.IndexOf(z.zonas, Pacer.Zone) < 0) Pacer.EnterZone(z.zonas[0]);
            UIManager.I.ShowZoneTitle(z.titulo);
            var st = Game.State;
            string seen = "intro_" + z.zonas[0];
            if (st != null && !st.Flag(seen) && !string.IsNullOrEmpty(z.intro))
            {
                st.SetFlag(seen);
                if (z.introEnDorado && WorldManager.I != null) WorldManager.I.RingBell(true, 3f);
                UIManager.I.PlayDialogue(z.intro, () => { if (!string.IsNullOrEmpty(z.notif)) Game.Notify(z.notif); });
            }
            SceneReady?.Invoke(SceneManager.GetActiveScene().name);
        }

        /// Puerta de salida de la escena.
        public void ExitScene()
        {
            var z = ZoneController.I;
            CompleteZones(z != null ? z.zonas : new[] { Game.State.zona });
        }

        // ------------------------------------------------------------ ritmo
        public void ForceTheater()
        {
            if (FreeRoam) return;
            Debug.Log("[Ritmo] envío forzado al Teatro, min " + (Pacer.Elapsed / 60f).ToString("0.0"));
            UIManager.I.CloseAllModals();
            Game.State.notas.Add("Don Ruperto llevó a Fran al Teatro para el Juicio (minuto " + Mathf.RoundToInt(Pacer.Elapsed / 60f) + ").");
            InGameplay = false;
            UIManager.I.PlayDialogue("ruperto_teatro", () =>
            {
                string target = SceneAvailable(Pacer.SceneFor("teatro")) ? "teatro" : "salida";
                foreach (var zi in Game.DB.sesion.zonas)
                    if (Pacer.BeforeTheater(zi.id) && !Game.State.zonasCompletas.Contains(zi.id) && zi.id != "diagnostico") Omit(zi.id, "por tiempo (envío al Teatro)");
                Game.State.zona = target;
                SaveNow();
                GoToZone(target);
            });
        }

        public void ForceExitTest()
        {
            if (FreeRoam || exitStarted) return;
            UIManager.I.CloseAllModals();
            Game.State.notas.Add("La prueba de salida empezó por tiempo.");
            Debug.Log("[Ritmo] prueba de salida forzada, min " + (Pacer.Elapsed / 60f).ToString("0.0"));
            StartExitTest();
        }

        // ------------------------------------------------------------ final
        public void StartExitTest()
        {
            if (exitStarted) return;
            exitStarted = true;
            InGameplay = false;
            Pacer?.EnterZone("salida");
            Game.State.zona = "salida";
            SaveNow();
            UIManager.I.CloseAllModals();
            UIManager.I.SetHudVisible(false);
            UIManager.I.PlayDialogue("salida_intro", () =>
                AssessmentRunner.Run("Repasa+ · Prueba de salida", Game.DB.evaluacion.salida, Game.State.salida, FinishSession, Seed() + 7));
        }

        void FinishSession()
        {
            var st = Game.State;
            st.terminada = true;
            if (Pacer != null) st.elapsed = Pacer.Elapsed;
            string path = null, error = null;
            try { path = Exporter.Export(st, Settings.CarpetaExport); }
            catch (Exception e) { error = e.Message; Debug.LogError("Export: " + e); }
            try { System.IO.File.WriteAllText(System.IO.Path.Combine(SaveSystem.Folder, "ultima_sesion.json"), Exporter.Json(st)); } catch { }
            SaveSystem.Delete();
            SessionManager.Stop();
            Debug.Log("[Sesion] Exportada: " + (path ?? error));
            ResultsScreen.Show(st, path, error);
        }

        // ------------------------------------------------------------ guardado
        public void SaveNow()
        {
            if (Game.State == null || FreeRoam) return; // el recorrido libre no pisa la partida real
            if (Pacer != null) Game.State.elapsed = Pacer.Elapsed;
            try { SaveSystem.Save(Game.State); }
            catch (Exception e) { Debug.LogWarning("No se pudo guardar: " + e.Message); }
        }

        public void SaveAndQuitToMenu()
        {
            SaveNow();
            ReturnToMenu();
        }
    }
}
