using EnganoColorido.UI;
using UnityEngine;

namespace EnganoColorido.Core
{
    /// Crea los sistemas persistentes en cualquier escena (también al probar una escena suelta en el editor).
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            Application.targetFrameRate = 60;
            AudioListener.volume = Settings.Volumen;
            UIManager.Ensure();
            GameFlow.Ensure();
        }
    }
}
