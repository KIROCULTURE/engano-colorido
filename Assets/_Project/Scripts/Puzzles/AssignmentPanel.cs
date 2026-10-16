using System.Collections.Generic;
using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.UI;
using TMPro;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// Panel genérico de "poner cada cosa en su lugar": diales del candado, etiquetas, retratos, estante,
    /// máscaras, huecos del Juicio y versos de Tu verso. Clic en un hueco → clic en una opción.
    public class AssignmentPanel : MonoBehaviour
    {
        PuzzleController ctrl;
        Paso paso;
        int stepIndex;
        string[] answers;
        int selected;
        RectTransform slotsList, optionsList;
        TextMeshProUGUI preview, feedback;
        readonly List<UnityEngine.UI.Button> slotButtons = new();

        static readonly Dictionary<string, Color> RimaColor = new()
        {
            { "A", new Color(0.55f, 0.22f, 0.22f) }, { "B", new Color(0.20f, 0.30f, 0.55f) },
            { "C", new Color(0.22f, 0.45f, 0.25f) }, { "D", new Color(0.45f, 0.28f, 0.50f) }
        };

        public static void Show(PuzzleController ctrl, Paso paso, int stepIndex)
        {
            var c = UIKit.Window(UIManager.I.Panels, 1640, 980, out var root, "Asignar");
            var p = root.gameObject.AddComponent<AssignmentPanel>();
            p.ctrl = ctrl; p.paso = paso; p.stepIndex = stepIndex;
            p.answers = new string[paso.slots.Length];
            p.Build(c, root.gameObject);
            UIManager.I.PushModal(root.gameObject);
        }

        bool Facil => Settings.Dificultad == 0;
        bool Normal => Settings.Dificultad == 1;

        void Build(RectTransform c, GameObject root)
        {
            UIKit.Text(c, $"{ctrl.HintTitle} · {paso.titulo}", 32, TextAlignmentOptions.Left, UIKit.Oro);
            if (!string.IsNullOrEmpty(paso.voz))
                UIKit.Text(c, $"<color=#EBC261>El Retrato:</color> «{paso.voz}»", 30);
            if (!string.IsNullOrEmpty(paso.enunciado)) UIKit.Text(c, paso.enunciado, 24);
            if (!string.IsNullOrEmpty(paso.plantilla))
            {
                preview = UIKit.Text(c, "", 26, TextAlignmentOptions.Left, UIKit.Texto);
                preview.fontStyle = FontStyles.Italic;
            }

            var row = UIKit.HRow(c, 0, 20);
            UIKit.Size(row.gameObject, -1, 1);
            var left = UIKit.Make("Huecos", row);
            UIKit.VList(left, 6);
            UIKit.Size(left.gameObject, -1, -1, 760, 1);
            UIKit.Text(left, "Huecos (elige uno)", 20, TextAlignmentOptions.Left, UIKit.Tenue);
            slotsList = UIKit.Scroll(left, 6);
            UIKit.Size(slotsList.parent.parent.gameObject, -1, 1);
            var right = UIKit.Make("Opciones", row);
            UIKit.VList(right, 6);
            UIKit.Size(right.gameObject, -1, -1, 760, 1);
            UIKit.Text(right, "Opciones", 20, TextAlignmentOptions.Left, UIKit.Tenue);
            optionsList = UIKit.Scroll(right, 6);
            UIKit.Size(optionsList.parent.parent.gameObject, -1, 1);

            feedback = UIKit.Text(c, "", 22, TextAlignmentOptions.Left, UIKit.Mal);
            var buttons = UIKit.HRow(c, 58, 12);
            UIKit.Button(buttons, "Cerrar", () => UIManager.I.CloseModal(root), 0, 24);
            UIKit.Button(buttons, "Pedir pista", ShowHint, 0, 24);
            UIKit.Button(buttons, "Comprobar", Submit, 0, 24, UIKit.BotonSel);

            for (int i = 0; i < paso.slots.Length; i++)
            {
                int idx = i;
                var b = UIKit.Button(slotsList, "", () => Select(idx), -1, 22);
                UIKit.Size(b.gameObject, paso.slots.Length > 8 ? 50 : 76);
                UIKit.Label(b).alignment = TextAlignmentOptions.Left;
                slotButtons.Add(b);
            }
            Select(0);
        }

        string SlotLabel(int i)
        {
            var s = paso.slots[i];
            string label = s.etiqueta;
            if (paso.estante && (Facil || Normal))
                label += $"  <color=#EBC261>({ContentDatabase.Tag(paso, s.correcta)})</color>";
            string val = answers[i] ?? "<color=#A09A90>— vacío —</color>";
            return $"<b>{label}</b>\n<size=90%>{val}</size>";
        }

        void Refresh()
        {
            for (int i = 0; i < slotButtons.Count; i++)
            {
                UIKit.Label(slotButtons[i]).text = SlotLabel(i);
                var col = i == selected ? UIKit.BotonSel : UIKit.Boton;
                if (i != selected && paso.estante && Facil && answers[i] != null && RimaColor.TryGetValue(ContentDatabase.Tag(paso, answers[i]) ?? "", out var rc)) col = rc;
                slotButtons[i].image.color = col;
            }
            if (preview != null)
            {
                var fill = new object[paso.slots.Length];
                for (int i = 0; i < fill.Length; i++) fill[i] = answers[i] != null ? $"<b>{answers[i]}</b>" : "_______";
                string txt = string.Format(paso.plantilla, fill);
                if (txt.StartsWith("<b>") && txt.Length > 3) txt = "<b>" + char.ToUpper(txt[3]) + txt.Substring(4);
                preview.text = "«" + txt + "»";
            }
        }

        void Select(int i)
        {
            selected = i;
            UIKit.Clear(optionsList);
            // Orden barajado pero estable (mismo orden cada vez que se abre): la lista no delata la solución.
            var all = Assessment.AssessmentRunner.Shuffle(ContentDatabase.Opciones(paso, paso.slots[i]), new System.Random((ctrl.puzzleId + paso.id).GetHashCode()));
            int shown = 0;
            foreach (var o in all)
            {
                if (paso.usaInventario)
                {
                    var tag = ContentDatabase.Tag(paso, o);
                    if (Game.State != null && !Game.State.inventario.Contains(tag)) continue;
                }
                string opt = o;
                var color = UIKit.Boton;
                if (paso.estante && Facil && RimaColor.TryGetValue(ContentDatabase.Tag(paso, o) ?? "", out var rc)) color = rc;
                var b = UIKit.Button(optionsList, opt, () => Assign(opt), -1, 22, color);
                UIKit.Size(b.gameObject, all.Count > 8 ? 50 : 64);
                UIKit.Label(b).alignment = TextAlignmentOptions.Left;
                shown++;
            }
            if (paso.usaInventario)
                UIKit.Text(optionsList, shown == 0
                    ? "No tienes objetos para poner aquí. Explora la galería y sus salas."
                    : "<size=85%>Solo aparecen los objetos que llevas. ¿Te falta alguno? Explora.</size>", 20, TextAlignmentOptions.Left, UIKit.Tenue);
            Refresh();
        }

        void Assign(string opt)
        {
            answers[selected] = opt;
            feedback.text = "";
            int next = -1;
            for (int k = 1; k <= answers.Length; k++)
            {
                int j = (selected + k) % answers.Length;
                if (answers[j] == null) { next = j; break; }
            }
            if (next >= 0) Select(next); else Refresh();
        }

        void Submit()
        {
            for (int i = 0; i < answers.Length; i++)
                if (answers[i] == null) { feedback.text = "Completa todos los huecos."; Select(i); return; }
            if (ctrl.Check(stepIndex, answers, out var fb))
            {
                UIManager.I.CloseModal(gameObject);
                ctrl.Next(stepIndex);
            }
            else { feedback.color = UIKit.Mal; feedback.text = fb ?? "Algo no calza. Revisa y vuelve a intentar, o pide una pista."; }
        }

        void ShowHint()
        {
            var h = ctrl.RequestHint();
            feedback.color = UIKit.Oro;
            feedback.text = h != null ? $"Pista {ctrl.HintsShown}: {h}" : "Ya viste las tres pistas. Si no sale, Don Ruperto puede mostrar el camino.";
        }
    }
}
