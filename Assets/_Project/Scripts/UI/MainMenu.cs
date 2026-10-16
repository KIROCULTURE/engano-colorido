using EnganoColorido.Core;
using EnganoColorido.Save;
using TMPro;
using UnityEngine;

namespace EnganoColorido.UI
{
    /// Menú principal. Lo que aún no existe aparece bloqueado con «Próximamente».
    public static class MainMenu
    {
        public static void Show()
        {
            var ui = UIManager.I;
            var root = UIKit.Stretch(UIKit.Make("MenuPrincipal", ui.Panels));
            var col = UIKit.Make("Columna", root);
            UIKit.Anchor(col, new Vector2(0.06f, 0.5f), new Vector2(680, 960), Vector2.zero);
            col.pivot = new Vector2(0, 0.5f);
            UIKit.VList(col, 14);
            UIKit.Text(col, "Engaño Colorido", 76, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(col, "Serious game del Siglo de Oro · 2° medio · Lengua y Literatura", 24, TextAlignmentOptions.Left, UIKit.Tenue);
            UIKit.Size(UIKit.Make("Espacio", col).gameObject, 24);

            UIKit.Button(col, $"Nueva sesión (modo {Settings.Modo} · {Settings.Curso})", NewSessionPanel.Show, 62, 26, UIKit.BotonSel);
            var cont = UIKit.Button(col, "Continuar partida guardada", () => { Close(root.gameObject); GameFlow.I.Continue(); }, 62, 26);
            cont.interactable = SaveSystem.HasSave();
            UIKit.Button(col, "Recorrido libre por zona (sin registro)", FreeRoamMenu, 62, 26);
            UIKit.Button(col, "Ajustes", () => PhoneUI.Open("Ajustes"), 62, 26);
            UIKit.Button(col, "Menú docente (PIN)", TeacherMenu.Ask, 62, 26);
            UIKit.Button(col, "Salir", Application.Quit, 62, 26);
            UIKit.Text(col, "Controles: WASD moverse · Mouse mirar · E interactuar · F linterna · C agacharse · Shift correr · Tab celular · Esc pausa", 20, TextAlignmentOptions.Left, UIKit.Tenue);
            ui.PushModal(root.gameObject);
        }

        static readonly (string escena, string nombre)[] Zonas =
        {
            ("Camarin", "Prólogo · Camarín"), ("Patio", "Patio de las rosas y radios"), ("Galeria", "Galería"),
            ("Biblioteca", "Biblioteca"), ("Teatro", "Teatro · Juicio")
        };

        static void FreeRoamMenu()
        {
            var disponibles = System.Array.FindAll(Zonas, z => Application.CanStreamedLevelBeLoaded(z.escena));
            var labels = new string[disponibles.Length + 1];
            for (int i = 0; i < disponibles.Length; i++) labels[i] = disponibles[i].nombre;
            labels[disponibles.Length] = "Volver";
            UIManager.I.ShowChoice("Recorrido libre", "Para revisar una zona suelta. No se guarda ni se exporta nada.", labels, i =>
            {
                if (i >= disponibles.Length) return;
                UIManager.I.CloseAllModals();
                GameFlow.I.StartFreeRoam(disponibles[i].escena);
            });
        }

        static void Locked(Transform col, string label)
        {
            var b = UIKit.Button(col, label + "  —  Próximamente", null, 62, 26);
            b.interactable = false;
        }

        static void Close(GameObject root) => UIManager.I.CloseModal(root);
    }
}
