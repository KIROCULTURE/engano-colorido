using EnganoColorido.UI;
using TMPro;

namespace EnganoColorido.Puzzles
{
    /// Tu verso, camino libre: cuatro versos propios (sin validación; se guardan para la profesora).
    public static class TuVersoWrite
    {
        public static void Show(PuzzleController ctrl)
        {
            var c = UIKit.Window(UIManager.I.Panels, 1100, 720, out var root, "TuVerso");
            UIKit.Text(c, "Tu verso", 34, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(c, "Cuatro versos sobre cómo percibes la vida. No hay respuesta correcta: la profesora los leerá.", 24);
            var lines = new TMP_InputField[4];
            for (int i = 0; i < 4; i++) lines[i] = UIKit.Input(c, $"Verso {i + 1}", 54, false, 80);
            var fb = UIKit.Text(c, "", 22, TextAlignmentOptions.Left, UIKit.Mal);
            var row = UIKit.HRow(c, 58);
            UIKit.Button(row, "Volver", () => UIManager.I.CloseModal(root.gameObject));
            UIKit.Button(row, "Guardar mi estrofa", () =>
            {
                var v = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    v[i] = TeacherMenu.Clean(lines[i].text, "");
                    if (v[i].Length == 0) { fb.text = "Escribe los cuatro versos."; return; }
                }
                UIManager.I.CloseModal(root.gameObject);
                ctrl.CompleteWritten(v);
            }, 0, 24, UIKit.BotonSel);
            UIManager.I.PushModal(root.gameObject);
            lines[0].ActivateInputField();
        }
    }
}
