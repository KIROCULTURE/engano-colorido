using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EnganoColorido.UI
{
    /// Helpers para armar uGUI por código (sin prefabs). Una sola fuente: la de TMP por defecto.
    public static class UIKit
    {
        public static readonly Color Fondo = new Color(0.04f, 0.04f, 0.06f, 0.94f);
        public static readonly Color Panel = new Color(0.11f, 0.11f, 0.13f, 0.97f);
        public static readonly Color Boton = new Color(0.20f, 0.19f, 0.22f, 1f);
        public static readonly Color BotonSel = new Color(0.55f, 0.42f, 0.16f, 1f);
        public static readonly Color Oro = new Color(0.92f, 0.76f, 0.38f, 1f);
        public static readonly Color Texto = new Color(0.93f, 0.91f, 0.86f, 1f);
        public static readonly Color Tenue = new Color(0.62f, 0.60f, 0.58f, 1f);
        public static readonly Color Bien = new Color(0.45f, 0.78f, 0.50f, 1f);
        public static readonly Color Mal = new Color(0.90f, 0.45f, 0.40f, 1f);

        public static RectTransform Make(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = 5;
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            return rt;
        }

        public static RectTransform Stretch(RectTransform rt, float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
            return rt;
        }

        public static RectTransform Center(RectTransform rt, float width, float height, float x = 0, float y = 0)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(width, height);
            rt.anchoredPosition = new Vector2(x, y);
            return rt;
        }

        public static RectTransform Anchor(RectTransform rt, Vector2 anchor, Vector2 size, Vector2 pos)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
            rt.sizeDelta = size;
            rt.anchoredPosition = pos;
            return rt;
        }

        public static UnityEngine.UI.Image Image(Transform parent, Color color, string name = "Image", bool raycast = true)
        {
            var rt = Make(name, parent);
            var img = rt.gameObject.AddComponent<UnityEngine.UI.Image>();
            img.color = color;
            img.raycastTarget = raycast;
            return img;
        }

        /// Panel centrado con fondo y lista vertical interna. Devuelve el contenedor de la lista.
        public static RectTransform Window(Transform parent, float width, float height, out RectTransform root, string name = "Window")
        {
            var dim = Image(parent, new Color(0, 0, 0, 0.55f), name);
            root = Stretch((RectTransform)dim.transform);
            var box = Image(dim.transform, Panel, "Box");
            Center((RectTransform)box.transform, width, height);
            var frame = Image(box.transform, Oro, "Marco", false);
            Stretch((RectTransform)frame.transform, 0, 0, 0, height - 4);
            var content = Stretch(Make("Contenido", box.transform), 28, 24, 28, 24);
            VList(content, 12);
            return content;
        }

        public static UnityEngine.UI.VerticalLayoutGroup VList(RectTransform rt, float spacing = 8, int pad = 0)
        {
            var v = rt.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            v.spacing = spacing;
            v.padding = new RectOffset(pad, pad, pad, pad);
            v.childControlWidth = true;
            v.childControlHeight = true;
            v.childForceExpandWidth = true;
            v.childForceExpandHeight = false;
            return v;
        }

        public static RectTransform HRow(Transform parent, float height, float spacing = 8)
        {
            var rt = Make("Fila", parent);
            var h = rt.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            h.spacing = spacing;
            h.childControlWidth = true;
            h.childControlHeight = true;
            h.childForceExpandWidth = true;
            h.childForceExpandHeight = true;
            Size(rt.gameObject, height);
            return rt;
        }

        public static UnityEngine.UI.LayoutElement Size(GameObject go, float height = -1, float flexibleHeight = -1, float width = -1, float flexibleWidth = -1)
        {
            var le = go.GetComponent<UnityEngine.UI.LayoutElement>() ?? go.AddComponent<UnityEngine.UI.LayoutElement>();
            if (height >= 0) { le.minHeight = height; le.preferredHeight = height; }
            if (flexibleHeight >= 0) le.flexibleHeight = flexibleHeight;
            if (width >= 0) { le.minWidth = width; le.preferredWidth = width; }
            if (flexibleWidth >= 0) le.flexibleWidth = flexibleWidth;
            return le;
        }

        public static TextMeshProUGUI Text(Transform parent, string text, float size = 26, TextAlignmentOptions align = TextAlignmentOptions.TopLeft, Color? color = null, string name = "Texto")
        {
            var rt = Make(name, parent);
            var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.alignment = align;
            t.color = color ?? Texto;
            t.textWrappingMode = TextWrappingModes.Normal;
            t.raycastTarget = false;
            t.richText = true;
            return t;
        }

        public static UnityEngine.UI.Button Button(Transform parent, string label, Action onClick, float height = 54, float fontSize = 24, Color? color = null)
        {
            var img = Image(parent, color ?? Boton, "Boton");
            var btn = img.gameObject.AddComponent<UnityEngine.UI.Button>();
            var cb = btn.colors;
            cb.highlightedColor = new Color(1.25f, 1.2f, 1.1f, 1f);
            cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            cb.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.6f);
            btn.colors = cb;
            var t = Text(img.transform, label, fontSize, TextAlignmentOptions.Center);
            Stretch(t.rectTransform, 10, 4, 10, 4);
            if (onClick != null) btn.onClick.AddListener(new UnityAction(onClick));
            if (height > 0) Size(img.gameObject, height);
            return btn;
        }

        public static TextMeshProUGUI Label(UnityEngine.UI.Button b) => b.GetComponentInChildren<TextMeshProUGUI>();

        /// ScrollRect con Viewport (Mask) y Content (VLG + ContentSizeFitter).
        public static RectTransform Scroll(Transform parent, float spacing = 8, string name = "Scroll")
        {
            var root = Make(name, parent);
            var bg = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0, 0, 0, 0.15f);
            var sr = root.gameObject.AddComponent<UnityEngine.UI.ScrollRect>();
            sr.horizontal = false;
            sr.scrollSensitivity = 30;
            sr.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;

            var vp = Stretch(Make("Viewport", root), 0, 0, 14, 0);
            var vpImg = vp.gameObject.AddComponent<UnityEngine.UI.Image>();
            vpImg.color = new Color(1, 1, 1, 0.01f);
            vp.gameObject.AddComponent<UnityEngine.UI.Mask>().showMaskGraphic = false;

            var content = Make("Content", vp);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.offsetMin = new Vector2(0, 0);
            content.offsetMax = new Vector2(0, 0);
            var v = VList(content, spacing, 6);
            var fit = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            fit.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

            var barBg = Image(root, new Color(1, 1, 1, 0.08f), "Barra");
            var barRt = (RectTransform)barBg.transform;
            barRt.anchorMin = new Vector2(1, 0); barRt.anchorMax = new Vector2(1, 1); barRt.pivot = new Vector2(1, 0.5f);
            barRt.sizeDelta = new Vector2(10, 0); barRt.anchoredPosition = Vector2.zero;
            var bar = barBg.gameObject.AddComponent<UnityEngine.UI.Scrollbar>();
            bar.direction = UnityEngine.UI.Scrollbar.Direction.BottomToTop;
            var area = Stretch(Make("Area", barRt));
            var handle = Image(area, Oro, "Handle");
            Stretch((RectTransform)handle.transform);
            bar.handleRect = (RectTransform)handle.transform;
            bar.targetGraphic = handle;

            sr.viewport = vp;
            sr.content = content;
            sr.verticalScrollbar = bar;
            sr.verticalScrollbarVisibility = UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            return content;
        }

        public static void Clear(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--) UnityEngine.Object.Destroy(t.GetChild(i).gameObject);
        }

        public static TMP_InputField Input(Transform parent, string placeholder, float height = 54, bool multiline = false, int charLimit = 0)
        {
            var bg = Image(parent, new Color(0.05f, 0.05f, 0.06f, 1f), "Entrada");
            Size(bg.gameObject, height);
            var area = Stretch(Make("Area", bg.transform), 12, 6, 12, 6);
            area.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
            var ph = Text(area, placeholder, 24, multiline ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.Left, Tenue, "Placeholder");
            ph.fontStyle = FontStyles.Italic;
            Stretch(ph.rectTransform);
            var txt = Text(area, "", 24, multiline ? TextAlignmentOptions.TopLeft : TextAlignmentOptions.Left, Texto, "Text");
            Stretch(txt.rectTransform);
            var input = bg.gameObject.AddComponent<TMP_InputField>();
            input.textViewport = area;
            input.textComponent = txt;
            input.placeholder = ph;
            input.targetGraphic = bg;
            input.lineType = multiline ? TMP_InputField.LineType.MultiLineNewline : TMP_InputField.LineType.SingleLine;
            input.characterLimit = charLimit;
            input.caretColor = Oro;
            input.customCaretColor = true;
            return input;
        }

        public static UnityEngine.UI.Slider Slider(Transform parent, float min, float max, float value, Action<float> onChange, float height = 36)
        {
            var root = Make("Slider", parent);
            Size(root.gameObject, height);
            var bg = Image(root, new Color(1, 1, 1, 0.12f), "Fondo");
            var bgRt = Stretch((RectTransform)bg.transform, 0, height * 0.35f, 0, height * 0.35f);
            var fillArea = Stretch(Make("FillArea", root), 0, height * 0.35f, 0, height * 0.35f);
            var fill = Image(fillArea, BotonSel, "Fill", false);
            Stretch((RectTransform)fill.transform);
            var handleArea = Stretch(Make("HandleArea", root), 10, 0, 10, 0);
            var handle = Image(handleArea, Oro, "Handle");
            var hrt = (RectTransform)handle.transform;
            hrt.sizeDelta = new Vector2(20, 0);
            hrt.anchorMin = new Vector2(0, 0); hrt.anchorMax = new Vector2(0, 1);
            var s = root.gameObject.AddComponent<UnityEngine.UI.Slider>();
            s.fillRect = (RectTransform)fill.transform;
            s.handleRect = hrt;
            s.targetGraphic = handle;
            s.minValue = min;
            s.maxValue = max;
            s.value = value;
            if (onChange != null) s.onValueChanged.AddListener(v => onChange(v));
            _ = bgRt;
            return s;
        }

        /// Texto escrito por estudiantes: se muestra sin interpretar etiquetas.
        public static string Escape(string s) => string.IsNullOrEmpty(s) ? "" : "<noparse>" + s.Replace("</noparse>", "") + "</noparse>";
    }
}
