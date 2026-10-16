using System;
using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.World;
using TMPro;
using UnityEngine;

namespace EnganoColorido.UI
{
    /// Celular de Fran (UI diegética): Cuaderno, Mapa, Inventario, Stories, Pensar, Ajustes.
    public class PhoneUI : MonoBehaviour
    {
        static PhoneUI current;
        public static bool IsOpen => current != null;
        static string lastTab = "Cuaderno";

        RectTransform content;
        TextMeshProUGUI header;
        string tab;
        int openedFrame;

        public static void Open(string tabName = null)
        {
            if (current != null) return;
            var ui = UIManager.I;
            var root = UIKit.Stretch(UIKit.Make("Celular", ui.Panels));
            var dim = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            dim.color = new Color(0, 0, 0, 0.45f);
            var body = UIKit.Image(root, new Color(0.03f, 0.03f, 0.04f, 1f), "Cuerpo");
            UIKit.Anchor((RectTransform)body.transform, new Vector2(0.5f, 0.5f), new Vector2(760, 1000), Vector2.zero);
            var screen = UIKit.Image(body.transform, new Color(0.10f, 0.10f, 0.12f, 1f), "Pantalla");
            var srt = UIKit.Stretch((RectTransform)screen.transform, 18, 18, 18, 18);
            UIKit.VList(srt, 8, 14);

            current = root.gameObject.AddComponent<PhoneUI>();
            current.openedFrame = Time.frameCount;
            current.header = UIKit.Text(srt, "", 22, TextAlignmentOptions.Left, UIKit.Tenue, "Barra");
            var tabs1 = UIKit.HRow(srt, 48, 6);
            var tabs2 = UIKit.HRow(srt, 48, 6);
            string[] names = { "Cuaderno", "Mapa", "Inventario", "Stories", "Pensar", "Ajustes" };
            for (int i = 0; i < names.Length; i++)
            {
                string n = names[i];
                UIKit.Button(i < 3 ? tabs1 : tabs2, n, () => current.Show(n), 0, 22);
            }
            current.content = UIKit.Scroll(srt, 8);
            UIKit.Size(current.content.parent.parent.gameObject, -1, 1);
            UIKit.Button(srt, "Cerrar celular (Tab / Esc)", Close, 50, 22);
            ui.PushModal(root.gameObject);
            current.Show(tabName ?? lastTab);
        }

        public static void Close()
        {
            if (current == null) return;
            var go = current.gameObject;
            current = null;
            UIManager.I.CloseModal(go);
        }

        void Update()
        {
            var k = UnityEngine.InputSystem.Keyboard.current;
            if (k != null && k.tabKey.wasPressedThisFrame && Time.frameCount > openedFrame + 1) Close();
            if (header != null) header.text = HeaderText();
        }

        static string HeaderText()
        {
            var s = Game.State;
            string t = "23:59";
            if (s != null) t += $"   ·   <color=#EBC261>Tiempo (reloj de arena): {Mathf.CeilToInt(s.tiempo)}%</color>";
            var p = GameFlow.I != null ? GameFlow.I.Pacer : null;
            if (p != null) t += $"\nSesión {Clock(p.Elapsed)} de ~{p.PlannedTotalMinutes:0}:00   ·   {ZonaNombre(p.Zone)}";
            return t;
        }

        static string Clock(float s) => $"{(int)(s / 60):00}:{(int)(s % 60):00}";
        static string ZonaNombre(string id) => Game.DB.Zona(id)?.nombre ?? id;

        void Show(string name)
        {
            tab = lastTab = name;
            UIKit.Clear(content);
            switch (name)
            {
                case "Cuaderno": Cuaderno(); break;
                case "Mapa": Mapa(); break;
                case "Inventario": Inventario(); break;
                case "Stories": Stories(); break;
                case "Pensar": Pensar(); break;
                case "Ajustes": Ajustes(); break;
            }
        }

        TextMeshProUGUI T(string s, float size = 24, Color? c = null) => UIKit.Text(content, s, size, TextAlignmentOptions.Left, c);

        void Item(string label, Action onClick)
        {
            var b = UIKit.Button(content, label, onClick, 56, 22);
            UIKit.Label(b).alignment = TextAlignmentOptions.Left;
        }

        void Cuaderno()
        {
            var s = Game.State; var db = Game.DB;
            T("Fichas de la profe Inés", 28, UIKit.Oro);
            if (s == null || s.fichas.Count == 0) T("Todavía no hay fichas. Búscalas pegadas cerca de cada acertijo.", 22, UIKit.Tenue);
            else foreach (var id in s.fichas) { var f = db.Ficha(id); if (f != null) Item(f.titulo + "  · p. " + f.pagina, () => Game.ShowFicha(f)); }
            T("Versos", 28, UIKit.Oro);
            if (s == null || s.poemas.Count == 0) T("Aún no anotas versos.", 22, UIKit.Tenue);
            else foreach (var id in s.poemas) { var p = db.Poema(id); if (p != null) Item(p.titulo + " — " + p.autor, () => UIManager.I.ShowText(p.titulo, ContentDatabase.PoemaTexto(p), "Cerrar")); }
            if (s != null && s.notas.Count > 0)
            {
                T("Notas", 28, UIKit.Oro);
                foreach (var n in s.notas) T("• " + n, 22);
            }
        }

        void Mapa()
        {
            var s = Game.State; var db = Game.DB; var p = GameFlow.I?.Pacer;
            T("Mapa del liceo", 28, UIKit.Oro);
            T("√ resuelto   ·   ? pendiente   ·   X cerrado u omitido", 20, UIKit.Tenue);
            foreach (var z in db.sesion.zonas ?? new ZonaInfo[0])
            {
                if (z.id == "diagnostico" || z.id == "salida") continue;
                string mark;
                if (s != null && s.zonasCompletas.Contains(z.id)) mark = "<color=#73C780>√</color>";
                else if ((s != null && s.zonasOmitidas.Contains(z.id)) || (p != null && !p.InRoute(z.id))) mark = "<color=#E07366>X</color>";
                else mark = "<color=#EBC261>?</color>";
                bool here = p != null && p.Zone == z.id;
                T($"{mark}   {z.nombre}{(here ? "   <color=#A09A90>(aquí)</color>" : "")}", 26);
            }
        }

        void Inventario()
        {
            var s = Game.State;
            T("Inventario", 28, UIKit.Oro);
            if (s == null || s.inventario.Count == 0) { T("Vacío.", 22, UIKit.Tenue); return; }
            foreach (var id in s.inventario) T($"<b>{Items.Name(id)}</b>\n<size=85%><color=#A09A90>{Items.Desc(id)}</color></size>", 24);
        }

        void Stories()
        {
            var s = Game.State;
            T("Stories de El Retrato", 28, UIKit.Oro);
            if (s == null || s.historias.Count == 0) { T("Nada nuevo… por ahora.", 22, UIKit.Tenue); return; }
            for (int i = s.historias.Count - 1; i >= 0; i--) T(s.historias[i], 23);
        }

        void Pensar()
        {
            var z = ZoneController.I;
            T("Pensar", 28, UIKit.Oro);
            T(z != null ? "Objetivo: " + z.Objective : "Explora.", 24);
            if (z != null && z.CurrentTask is IHintable h)
            {
                T(h.HintTitle, 24, UIKit.Oro);
                for (int i = 1; i <= h.HintsShown; i++) T($"Pista {i}: {h.Hint(i)}", 23);
                if (h.HintsShown < 3)
                    Item($"Pedir pista {h.HintsShown + 1} de 3", () => { h.RequestHint(); Show("Pensar"); });
                else T("Ya viste las tres pistas.", 20, UIKit.Tenue);
            }
        }

        void Ajustes()
        {
            T("Ajustes", 28, UIKit.Oro);
            T("Sensibilidad del mouse", 22);
            UIKit.Slider(content, 0.2f, 3f, Settings.Sensibilidad, v => Settings.Sensibilidad = v);
            T("Campo de visión (FOV)", 22);
            UIKit.Slider(content, 55f, 95f, Settings.FOV, v => Settings.FOV = v);
            T("Tamaño de la interfaz y el texto", 22);
            UIKit.Slider(content, 0.8f, 1.4f, Settings.TamanoTexto, v => { Settings.TamanoTexto = v; UIManager.I.SetUIScale(v); });
            T("Volumen", 22);
            UIKit.Slider(content, 0f, 1f, Settings.Volumen, v => Settings.Volumen = v);
            T("Subtítulos: siempre activos. Sin destellos.", 20, UIKit.Tenue);
            T($"Acertijos: {Settings.Dificultades[Settings.Dificultad]} (lo define la profesora en el menú docente).", 20, UIKit.Tenue);
        }
    }
}
