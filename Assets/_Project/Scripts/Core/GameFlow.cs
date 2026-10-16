using System;
using EnganoColorido.Save;
using EnganoColorido.Session;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnganoColorido.Core
{
    /// Orquesta la sesión: menú → diagnóstico → zonas → Juicio → prueba de salida → export.
    public class GameFlow : MonoBehaviour
    {
        public static GameFlow I { get; private set; }
        public static bool InGameplay { get; private set; }
        public const string MenuScene = "Arranque";

        public SessionPacer Pacer { get; private set; }
        public bool FreeRoam { get; private set; }

        public static GameFlow Ensure()
        {
            if (I != null) return I;
            var go = new GameObject("GameFlow");
            DontDestroyOnLoad(go);
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
            Pacer = null;
            UIManager.I.CloseAllModals();
            UIManager.I.Fade(true, 0.5f, () => SceneManager.LoadScene(MenuScene));
        }

        public void StartFreeRoam(string scene)
        {
            Game.State = NewState("recorrido");
            FreeRoam = true;
            Pacer = null;
            LoadScene(scene);
        }

        public void Continue()
        {
            var s = SaveSystem.Load();
            if (s == null) { UIManager.I.Toast("No hay partida guardada."); return; }
            Game.State = s;
            FreeRoam = false;
            LoadScene(Game.DB.Zona(s.zona)?.escena is string sc && sc != MenuScene ? sc : "Camarin");
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
            UIManager.I.ShowZoneTitle(z.titulo);
            var st = Game.State;
            string seen = "intro_" + z.zonas[0];
            if (st != null && !st.Flag(seen) && !string.IsNullOrEmpty(z.intro))
            {
                st.SetFlag(seen);
                UIManager.I.PlayDialogue(z.intro, () => { if (!string.IsNullOrEmpty(z.notif)) Game.Notify(z.notif); });
            }
        }

        /// Puerta de salida: la zona queda completa, se autoguarda y se pasa a la siguiente.
        public void ExitScene()
        {
            var z = ZoneController.I;
            var st = Game.State;
            if (z != null && st != null)
                foreach (var id in z.zonas) st.AddUnique(st.zonasCompletas, id);
            if (FreeRoam)
            {
                SaveNow();
                UIManager.I.ShowText("Fin del recorrido",
                    "Esta build muestra el núcleo jugable (M1): moverse, linterna, interactuar, celular y autoguardado por zona.\n\n" +
                    "La partida se guardó al salir de la zona.\n\n<color=#A09A90>Próximamente: Patio de las rosas y el resto del modo 45.</color>",
                    "Volver al menú", ReturnToMenu);
                return;
            }
        }

        public void SaveNow()
        {
            if (Game.State == null || FreeRoam && Game.State.alias == "editor") return;
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
