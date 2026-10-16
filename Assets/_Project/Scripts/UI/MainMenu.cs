using TMPro;
using UnityEngine;

namespace EnganoColorido.UI
{
    public static class MainMenu
    {
        public static void Show()
        {
            var ui = UIManager.I;
            ui.SetHudVisible(false);
            var root = UIKit.Stretch(UIKit.Make("MenuPrincipal", ui.Panels));
            var col = UIKit.Make("Columna", root);
            UIKit.Anchor(col, new Vector2(0.08f, 0.5f), new Vector2(620, 900), Vector2.zero);
            col.pivot = new Vector2(0, 0.5f);
            UIKit.VList(col, 14);
            UIKit.Text(col, "Engaño Colorido", 72, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(col, "Un serious game del Siglo de Oro · 2° medio", 26, TextAlignmentOptions.Left, UIKit.Tenue);
            UIKit.Size(UIKit.Make("Espacio", col).gameObject, 30);
            foreach (var label in new[] { "Nueva sesión", "Continuar", "Ajustes", "Menú docente" })
            {
                var b = UIKit.Button(col, label + "  —  Próximamente", null, 62, 26);
                b.interactable = false;
            }
            UIKit.Button(col, "Salir", Application.Quit, 62, 26);
            ui.PushModal(root.gameObject);
        }
    }
}
