using EnganoColorido.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnganoColorido.Core
{
    /// Orquesta la sesión: menú → diagnóstico → zonas → Juicio → prueba de salida → export.
    public class GameFlow : MonoBehaviour
    {
        public static GameFlow I { get; private set; }
        public const string MenuScene = "Arranque";

        public static GameFlow Ensure()
        {
            if (I != null) return I;
            var go = new GameObject("GameFlow");
            DontDestroyOnLoad(go);
            return go.AddComponent<GameFlow>();
        }

        void Awake() { I = this; }

        void Start()
        {
            if (SceneManager.GetActiveScene().name == MenuScene) MainMenu.Show();
        }
    }
}
