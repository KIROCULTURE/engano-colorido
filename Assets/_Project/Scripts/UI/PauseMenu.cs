using EnganoColorido.Core;

namespace EnganoColorido.UI
{
    public static class PauseMenu
    {
        public static void Show()
        {
            UIManager.I.ShowChoice("Pausa",
                "El reloj de la sesión sigue corriendo: es el tiempo de la clase.",
                new[] { "Continuar", "Abrir celular", "Guardar y volver al menú", "Salir del juego" },
                i =>
                {
                    if (i == 1) PhoneUI.Open();
                    else if (i == 2) GameFlow.I.SaveAndQuitToMenu();
                    else if (i == 3) { GameFlow.I.SaveNow(); UnityEngine.Application.Quit(); }
                });
        }
    }
}
