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
            var t = Tema.Liceo;
            var root = UIKit.Stretch(UIKit.Make("MenuPrincipal", ui.Panels));

            // Velo oscuro a la izquierda para leer sobre la escena del camarín
            var scrim = UIKit.Image(root, new Color(0, 0, 0, 0.62f), "Velo", false);
            var srt = (RectTransform)scrim.transform;
            srt.anchorMin = Vector2.zero; srt.anchorMax = new Vector2(0.46f, 1); srt.offsetMin = srt.offsetMax = Vector2.zero;

            var col = UIKit.Make("Columna", root);
            UIKit.Anchor(col, new Vector2(0.05f, 0.54f), new Vector2(700, 900), Vector2.zero);
            col.pivot = new Vector2(0, 0.5f);
            UIKit.VList(col, 12);

            var kicker = UIKit.Text(col, "SERIOUS GAME · LENGUA Y LITERATURA · 2° MEDIO", 18, TextAlignmentOptions.Left, t.acento);
            kicker.characterSpacing = 6; kicker.fontStyle = FontStyles.Bold;
            UIKit.Text(col, "Engaño Colorido", 84, TextAlignmentOptions.Left, t.acento);
            var epi = UIKit.Text(col, "«Este, que ves, engaño colorido…»\n<size=80%>— Sor Juana Inés de la Cruz</size>", 26, TextAlignmentOptions.Left, t.tenue);
            epi.fontStyle = FontStyles.Italic;
            UIKit.Size(UIKit.Make("Espacio", col).gameObject, 18);

            // Tarjeta de sesión: lo que la profesora dejó configurado + la acción principal
            var card = UIKit.Image(col, t.superficie, "TarjetaSesion");
            Tema.Redondear(card, 16);
            var c = (RectTransform)card.transform;
            UIKit.VList(c, 10, 20);
            var info = UIKit.Text(c, $"Curso <b>{Settings.Curso}</b>   ·   Modo <b>{Settings.Modo} min</b>   ·   Acertijos <b>{Settings.Dificultades[Settings.Dificultad]}</b>", 22, TextAlignmentOptions.Left, t.texto);
            info.raycastTarget = false;
            UIKit.Primary(c, "Nueva sesión", NewSessionPanel.Show, t, 64, 28);
            bool hay = SaveSystem.HasSave();
            var cont = UIKit.Secondary(c, hay ? "Continuar partida guardada" : "Continuar  ·  no hay partida guardada", () => { Close(root.gameObject); GameFlow.I.Continue(); }, t, 54, 22);
            cont.interactable = hay;
            UIKit.Text(c, "Modo 90 (Capilla y Torre): Próximamente · Entrega 2", 19, TextAlignmentOptions.Left, t.tenue);

            UIKit.Size(UIKit.Make("Espacio", col).gameObject, 8);
            var row = UIKit.HRow(col, 54, 10);
            UIKit.Secondary(row, "Recorrido libre", FreeRoamMenu, t, 0, 22);
            UIKit.Secondary(row, "Ajustes", () => PhoneUI.Open("Ajustes"), t, 0, 22);
            var row2 = UIKit.HRow(col, 54, 10);
            UIKit.Secondary(row2, "Menú docente", TeacherMenu.Ask, t, 0, 22);
            UIKit.Secondary(row2, "Salir", Application.Quit, t, 0, 22);

            // Barra de controles abajo
            var bar = UIKit.Image(root, new Color(0, 0, 0, 0.7f), "Controles", false);
            var brt = (RectTransform)bar.transform;
            brt.anchorMin = new Vector2(0, 0); brt.anchorMax = new Vector2(1, 0); brt.pivot = new Vector2(0.5f, 0);
            brt.sizeDelta = new Vector2(0, 54); brt.anchoredPosition = Vector2.zero;
            string K(string k) => $"<color=#{ColorUtility.ToHtmlStringRGB(t.acento)}><b>{k}</b></color>";
            var ctl = UIKit.Text(bar.transform, $"{K("WASD")} moverse    {K("Mouse")} mirar    {K("E")} interactuar    {K("F")} linterna    {K("C")} agacharse    {K("Shift")} correr    {K("Tab")} celular    {K("Esc")} pausa", 21, TextAlignmentOptions.Left, t.texto);
            UIKit.Stretch(ctl.rectTransform, 96, 0, 340, 0);
            ctl.alignment = TextAlignmentOptions.MidlineLeft;
            var ver = UIKit.Text(bar.transform, "Entrega 1 · avance", 19, TextAlignmentOptions.MidlineRight, t.tenue);
            UIKit.Stretch(ver.rectTransform, 0, 0, 40, 0);

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

        static void Close(GameObject root) => UIManager.I.CloseModal(root);
    }
}
