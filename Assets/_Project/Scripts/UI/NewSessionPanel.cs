using EnganoColorido.Core;
using TMPro;

namespace EnganoColorido.UI
{
    /// Inicio de sesión del estudiante: solo un alias.
    public static class NewSessionPanel
    {
        public static void Show()
        {
            var c = UIKit.Window(UIManager.I.Panels, 900, 520, out var root, "NuevaSesion");
            UIKit.Text(c, "Nueva sesión", 34, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(c, $"Curso: <b>{Settings.Curso}</b>   ·   Modo: <b>{Settings.Modo} min</b>   ·   Acertijos: <b>{Settings.Dificultades[Settings.Dificultad]}</b>", 22);
            UIKit.Text(c, "Escribe un <b>alias</b> (un apodo, no tu nombre completo). Si juegas en pareja, pongan los dos alias.", 24);
            var alias = UIKit.Input(c, "Alias", 56, false, 24);
            var row = UIKit.HRow(c, 58);
            UIKit.Button(row, "Volver", () => UIManager.I.CloseModal(root.gameObject));
            UIKit.Button(row, "Comenzar", () =>
            {
                var a = TeacherMenu.Clean(alias.text, "");
                if (a.Length < 2) { UIManager.I.Toast("Escribe un alias de al menos 2 letras."); return; }
                UIManager.I.CloseAllModals();
                GameFlow.I.StartNewSession(a);
            }, 0, 24, UIKit.BotonSel);
            UIManager.I.PushModal(root.gameObject);
            alias.ActivateInputField();
        }
    }
}
