using System;
using System.Collections;
using System.Collections.Generic;
using EnganoColorido.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace EnganoColorido.UI
{
    /// Canvas único del juego: HUD, ventanas modales, avisos, subtítulos y fundidos.
    public class UIManager : MonoBehaviour
    {
        public static UIManager I { get; private set; }

        public RectTransform Hud { get; private set; }
        public RectTransform Panels { get; private set; }
        RectTransform toasts;
        CanvasGroup fade;
        TextMeshProUGUI prompt, zoneTitle, crosshair;
        UnityEngine.UI.CanvasScaler scaler;
        readonly List<GameObject> modals = new();

        public bool AnyModal { get { modals.RemoveAll(m => m == null); return modals.Count > 0; } }
        public static event Action ModalsChanged;

        public static UIManager Ensure()
        {
            if (I != null) return I;
            var go = new GameObject("UI");
            DontDestroyOnLoad(go);
            return go.AddComponent<UIManager>();
        }

        void Awake()
        {
            I = this;
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            gameObject.layer = 5;

            Hud = UIKit.Stretch(UIKit.Make("HUD", transform));
            Panels = UIKit.Stretch(UIKit.Make("Paneles", transform));
            toasts = UIKit.Make("Avisos", transform);
            UIKit.Anchor(toasts, new Vector2(1, 1), new Vector2(560, 600), new Vector2(-24, -24));
            var tv = UIKit.VList(toasts, 10);
            tv.childAlignment = TextAnchor.UpperRight;
            var fadeImg = UIKit.Image(transform, Color.black, "Fundido");
            UIKit.Stretch((RectTransform)fadeImg.transform);
            fade = fadeImg.gameObject.AddComponent<CanvasGroup>();
            fade.alpha = 0;
            fade.blocksRaycasts = false;

            crosshair = UIKit.Text(Hud, "·", 48, TextAlignmentOptions.Center, new Color(1, 1, 1, 0.7f), "Mira");
            UIKit.Center(crosshair.rectTransform, 40, 40);
            prompt = UIKit.Text(Hud, "", 28, TextAlignmentOptions.Center, UIKit.Texto, "Indicacion");
            UIKit.Anchor(prompt.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(900, 50), new Vector2(0, -70));
            zoneTitle = UIKit.Text(Hud, "", 44, TextAlignmentOptions.Center, UIKit.Oro, "TituloZona");
            UIKit.Anchor(zoneTitle.rectTransform, new Vector2(0.5f, 1), new Vector2(1200, 80), new Vector2(0, -60));
            SetHudVisible(false);

            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var es = new GameObject("EventSystem");
                DontDestroyOnLoad(es);
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<InputSystemUIInputModule>();
            }
            ApplyTextScale();
        }

        public void ApplyTextScale() => scaler.scaleFactor = 1f;

        public void SetUIScale(float s)
        {
            // Tamaño de texto: escalamos la resolución de referencia (más chica = todo más grande).
            scaler.referenceResolution = new Vector2(1920, 1080) / Mathf.Clamp(s, 0.8f, 1.4f);
        }

        public void SetHudVisible(bool v) { Hud.gameObject.SetActive(v); }
        public void SetPrompt(string text) { prompt.text = text ?? ""; }

        // ---------- Modales ----------
        public void PushModal(GameObject go) { modals.Add(go); ModalsChanged?.Invoke(); }
        public int LastCloseFrame { get; private set; } = -10;
        public bool JustClosed => Time.frameCount - LastCloseFrame <= 1;

        public void CloseModal(GameObject go)
        {
            LastCloseFrame = Time.frameCount;
            modals.Remove(go);
            if (go != null) Destroy(go);
            ModalsChanged?.Invoke();
        }
        public void CloseAllModals()
        {
            foreach (var m in modals) if (m != null) Destroy(m);
            modals.Clear();
            ModalsChanged?.Invoke();
        }

        public GameObject ShowText(string title, string body, string button = "Entendido", Action onClose = null, float width = 1100, float height = 760)
        {
            var c = UIKit.Window(Panels, width, height, out var root, "Texto");
            UIKit.Text(c, title, 36, TextAlignmentOptions.Left, UIKit.Oro);
            var scroll = UIKit.Scroll(c);
            UIKit.Size(scroll.parent.parent.gameObject, -1, 1);
            UIKit.Text(scroll, body, 27);
            UIKit.Button(c, button, () => { CloseModal(root.gameObject); onClose?.Invoke(); });
            PushModal(root.gameObject);
            return root.gameObject;
        }

        public GameObject ShowChoice(string title, string body, string[] options, Action<int> onPick, float width = 1000)
        {
            float h = 300 + options.Length * 66 + (string.IsNullOrEmpty(body) ? 0 : 160);
            var c = UIKit.Window(Panels, width, Mathf.Min(h, 980), out var root, "Eleccion");
            UIKit.Text(c, title, 34, TextAlignmentOptions.Left, UIKit.Oro);
            if (!string.IsNullOrEmpty(body))
            {
                var scroll = UIKit.Scroll(c);
                UIKit.Size(scroll.parent.parent.gameObject, -1, 1);
                UIKit.Text(scroll, body, 26);
            }
            for (int i = 0; i < options.Length; i++)
            {
                int idx = i;
                UIKit.Button(c, options[i], () => { CloseModal(root.gameObject); onPick?.Invoke(idx); }, 58);
            }
            PushModal(root.gameObject);
            return root.gameObject;
        }

        /// Pregunta de alternativas. Devuelve el texto elegido.
        public GameObject ShowQuestion(string header, string enunciado, List<string> opciones, string contexto, Action<string> onAnswer, string footer = null)
        {
            bool ctx = !string.IsNullOrEmpty(contexto);
            var c = UIKit.Window(Panels, ctx ? 1500 : 1100, ctx ? 960 : 640 + opciones.Count * 20, out var root, "Pregunta");
            UIKit.Text(c, header, 24, TextAlignmentOptions.Left, UIKit.Tenue);
            if (ctx)
            {
                var row = UIKit.HRow(c, 0, 24);
                UIKit.Size(row.gameObject, -1, 1);
                var left = UIKit.Scroll(row);
                UIKit.Text(left, contexto, 23);
                var rightRt = UIKit.Make("Derecha", row);
                UIKit.VList(rightRt, 10);
                FillQuestion(rightRt, enunciado, opciones, root.gameObject, onAnswer);
            }
            else FillQuestion(c, enunciado, opciones, root.gameObject, onAnswer);
            if (!string.IsNullOrEmpty(footer)) UIKit.Text(c, footer, 20, TextAlignmentOptions.Left, UIKit.Tenue);
            PushModal(root.gameObject);
            return root.gameObject;
        }

        void FillQuestion(Transform c, string enunciado, List<string> opciones, GameObject root, Action<string> onAnswer)
        {
            UIKit.Text(c, enunciado, 30);
            foreach (var o in opciones)
            {
                string op = o;
                var b = UIKit.Button(c, op, () => { CloseModal(root); onAnswer?.Invoke(op); }, -1, 25);
                UIKit.Size(b.gameObject, 64);
                UIKit.Label(b).alignment = TextAlignmentOptions.Left;
            }
        }

        // ---------- Subtítulos (diálogos lineales) ----------
        public void PlayDialogue(string id, Action onDone = null)
        {
            var d = ContentDatabase.Instance.Dialogo(id);
            if (d == null || d.lineas == null || d.lineas.Length == 0) { onDone?.Invoke(); return; }
            PlayLines(d.lineas, onDone);
        }

        public void PlayLines(Linea[] lineas, Action onDone = null)
        {
            var root = UIKit.Stretch(UIKit.Make("Dialogo", Panels));
            var catcher = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            catcher.color = new Color(0, 0, 0, 0.2f);
            var bar = UIKit.Image(root, new Color(0, 0, 0, 0.82f), "Barra");
            var brt = (RectTransform)bar.transform;
            brt.anchorMin = new Vector2(0.12f, 0); brt.anchorMax = new Vector2(0.88f, 0); brt.pivot = new Vector2(0.5f, 0);
            brt.sizeDelta = new Vector2(0, 230); brt.anchoredPosition = new Vector2(0, 40);
            var who = UIKit.Text(bar.transform, "", 28, TextAlignmentOptions.TopLeft, UIKit.Oro, "Quien");
            UIKit.Stretch(who.rectTransform, 30, 18, 30, 170);
            var txt = UIKit.Text(bar.transform, "", 30, TextAlignmentOptions.TopLeft, UIKit.Texto, "Linea");
            UIKit.Stretch(txt.rectTransform, 30, 62, 30, 40);
            var hint = UIKit.Text(bar.transform, "Clic / Espacio para continuar", 18, TextAlignmentOptions.BottomRight, UIKit.Tenue, "Ayuda");
            UIKit.Stretch(hint.rectTransform, 30, 190, 30, 10);
            var runner = root.gameObject.AddComponent<DialogueRunner>();
            runner.Init(lineas, who, txt, () => { CloseModal(root.gameObject); onDone?.Invoke(); });
            PushModal(root.gameObject);
        }

        // ---------- Avisos ----------
        public void Toast(string text, float seconds = 5f, bool retrato = false)
        {
            var img = UIKit.Image(toasts, retrato ? new Color(0.35f, 0.25f, 0.08f, 0.92f) : new Color(0, 0, 0, 0.8f), "Aviso", false);
            var v = UIKit.VList((RectTransform)img.transform, 4, 14);
            v.childAlignment = TextAnchor.UpperLeft;
            img.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            UIKit.Text(img.transform, text, 23);
            img.transform.SetAsFirstSibling();
            Destroy(img.gameObject, seconds);
        }

        public void ShowZoneTitle(string title) { StopCoroutine(nameof(ZoneTitleCo)); StartCoroutine(ZoneTitleCo(title)); }
        IEnumerator ZoneTitleCo(string title)
        {
            zoneTitle.text = title;
            zoneTitle.alpha = 0;
            for (float t = 0; t < 1; t += Time.unscaledDeltaTime) { zoneTitle.alpha = t; yield return null; }
            yield return new WaitForSecondsRealtime(2.5f);
            for (float t = 1; t > 0; t -= Time.unscaledDeltaTime) { zoneTitle.alpha = t; yield return null; }
            zoneTitle.text = "";
        }

        // ---------- Fundido ----------
        public void Fade(bool toBlack, float seconds, Action then = null) => StartCoroutine(FadeCo(toBlack, seconds, then));
        IEnumerator FadeCo(bool toBlack, float seconds, Action then)
        {
            fade.blocksRaycasts = true;
            float from = fade.alpha, to = toBlack ? 1 : 0;
            for (float t = 0; t < seconds; t += Time.unscaledDeltaTime)
            {
                fade.alpha = Mathf.Lerp(from, to, t / seconds);
                yield return null;
            }
            fade.alpha = to;
            fade.blocksRaycasts = toBlack;
            then?.Invoke();
        }

        void Update()
        {
            bool cursorFree = AnyModal || !Hud.gameObject.activeSelf;
            Cursor.lockState = cursorFree ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = cursorFree;
            crosshair.enabled = !AnyModal;
            if (AnyModal) prompt.text = "";
        }
    }
}
