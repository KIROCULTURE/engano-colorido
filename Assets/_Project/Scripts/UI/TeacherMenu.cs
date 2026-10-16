using EnganoColorido.Assessment;
using EnganoColorido.Core;
using EnganoColorido.Data;
using TMPro;

namespace EnganoColorido.UI
{
    /// Menú docente protegido con PIN: curso, modo, dificultad de acertijos, carpeta de exportación y resumen del curso.
    public static class TeacherMenu
    {
        public static void Ask()
        {
            var c = UIKit.Window(UIManager.I.Panels, 700, 380, out var root, "PIN");
            UIKit.Text(c, "Menú docente", 34, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(c, "Ingresa el PIN (se cambia en Data/session.json).", 22, TextAlignmentOptions.Left, UIKit.Tenue);
            var pin = UIKit.Input(c, "PIN", 54, false, 8);
            pin.contentType = TMP_InputField.ContentType.Pin;
            var row = UIKit.HRow(c, 56);
            UIKit.Button(row, "Cancelar", () => UIManager.I.CloseModal(root.gameObject));
            UIKit.Button(row, "Entrar", () =>
            {
                if (pin.text == ContentDatabase.Instance.sesion.pinDocente) { UIManager.I.CloseModal(root.gameObject); Show(); }
                else { pin.text = ""; UIManager.I.Toast("PIN incorrecto."); }
            });
            UIManager.I.PushModal(root.gameObject);
            pin.ActivateInputField();
        }

        public static void Show()
        {
            var c = UIKit.Window(UIManager.I.Panels, 1000, 900, out var root, "Docente");
            UIKit.Text(c, "Menú docente", 34, TextAlignmentOptions.Left, UIKit.Oro);

            UIKit.Text(c, "Curso", 22);
            var curso = UIKit.Input(c, "Ej: 2°A", 50, false, 12);
            curso.text = Settings.Curso;

            UIKit.Text(c, "Modo de la sesión", 22);
            var modos = UIKit.HRow(c, 54);
            var m45 = UIKit.Button(modos, "Modo 45 (~40 min de juego)", null, 0, 22, UIKit.BotonSel);
            var m90 = UIKit.Button(modos, "Modo 90 — Próximamente (Entrega 2)", null, 0, 22);
            m90.interactable = ContentDatabase.Instance.Modo(90)?.disponible ?? false;
            m45.onClick.AddListener(() => { Settings.Modo = 45; m45.image.color = UIKit.BotonSel; m90.image.color = UIKit.Boton; });
            m90.onClick.AddListener(() => { Settings.Modo = 90; m90.image.color = UIKit.BotonSel; m45.image.color = UIKit.Boton; });
            if (!m90.interactable) Settings.Modo = 45;

            UIKit.Text(c, "Dificultad de acertijos", 22);
            var dif = UIKit.Button(c, "Acertijos: " + Settings.Dificultades[Settings.Dificultad], null, 54, 22);
            dif.onClick.AddListener(() =>
            {
                Settings.Dificultad = (Settings.Dificultad + 1) % 3;
                UIKit.Label(dif).text = "Acertijos: " + Settings.Dificultades[Settings.Dificultad];
            });

            UIKit.Text(c, "Carpeta de exportación (CSV + JSON). Puede ser una carpeta compartida o un pendrive.", 22);
            var carpeta = UIKit.Input(c, "Carpeta", 50);
            carpeta.text = Settings.CarpetaExport;

            UIKit.Button(c, "Ver resumen del curso (junta los CSV de la carpeta)", () =>
                UIManager.I.ShowText("Resumen del curso", CourseSummary.Build(carpeta.text), "Cerrar"), 54, 22);
            UIKit.Text(c, "Privacidad: solo alias, todo queda en este computador. Sin internet ni cuentas.", 20, TextAlignmentOptions.Left, UIKit.Tenue);
            UIKit.Button(c, "Guardar y cerrar", () =>
            {
                Settings.Curso = Clean(curso.text, "2°A");
                Settings.CarpetaExport = string.IsNullOrWhiteSpace(carpeta.text) ? Settings.CarpetaExport : carpeta.text.Trim();
                Settings.Save();
                UIManager.I.CloseModal(root.gameObject);
                GameFlow.I.ShowMenu();
            }, 58, 24, UIKit.BotonSel);
            UIManager.I.PushModal(root.gameObject);
        }

        /// Quita caracteres que romperían el CSV.
        public static string Clean(string s, string fallback)
        {
            s = (s ?? "").Replace(";", " ").Replace("\"", "").Replace("\n", " ").Trim();
            return s.Length == 0 ? fallback : s;
        }
    }
}
