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

        public GameObject ShowText(string title, string body, string button = "Entendido", Action onClose = null, float width = 1100, float height = 760, Tema tema = null, string rotulo = null)
        {
            tema ??= Tema.Liceo;
            var c = UIKit.Window(Panels, width, height, out var root, "Texto", tema);
            UIKit.Header(c, tema, rotulo, title);
            var scroll = UIKit.Scroll(c);
            UIKit.Size(scroll.parent.parent.gameObject, -1, 1);
            UIKit.Text(scroll, body, 26, TextAlignmentOptions.TopLeft, tema.texto);
            UIKit.Primary(c, button, () => { CloseModal(root.gameObject); onClose?.Invoke(); }, tema);
            PushModal(root.gameObject);
            return root.gameObject;
        }

        public GameObject ShowChoice(string title, string body, string[] options, Action<int> onPick, float width = 1000, Tema tema = null, string rotulo = null)
        {
            tema ??= Tema.Liceo;
            float h = 320 + options.Length * 70 + (string.IsNullOrEmpty(body) ? 0 : 170);
            var c = UIKit.Window(Panels, width, Mathf.Min(h, 980), out var root, "Eleccion", tema);
            UIKit.Header(c, tema, rotulo, title);
            if (!string.IsNullOrEmpty(body))
            {
                var scroll = UIKit.Scroll(c);
                UIKit.Size(scroll.parent.parent.gameObject, -1, 1);
                UIKit.Text(scroll, body, 25, TextAlignmentOptions.TopLeft, tema.texto);
            }
            for (int i = 0; i < options.Length; i++)
            {
                int idx = i;
                var b = UIKit.Secondary(c, options[i], () => { CloseModal(root.gameObject); onPick?.Invoke(idx); }, tema, 60);
                UIKit.Label(b).alignment = TextAlignmentOptions.MidlineLeft;
                UIKit.Chip(b, (i + 1).ToString(), tema);
            }
            PushModal(root.gameObject);
            return root.gameObject;
        }

        /// Pregunta de alternativas. Devuelve el texto elegido.
        /// rotulo: quién pregunta (Repasa+, un acertijo). progreso 0..1 dibuja una barra (evaluaciones).
        /// onHint (opcional, puzzles): agrega "Pedir pista" y "Cerrar".
        public GameObject ShowQuestion(string rotulo, string enunciado, List<string> opciones, string contexto, Action<string> onAnswer,
            string footer = null, Func<string> onHint = null, Tema tema = null, float progreso = -1)
        {
            tema ??= Tema.Liceo;
            bool ctx = !string.IsNullOrEmpty(contexto);
            var c = UIKit.Window(Panels, ctx ? 1560 : 1100, ctx ? 960 : 660 + opciones.Count * 24 + (onHint != null ? 80 : 0), out var root, "Pregunta", tema);
            UIKit.Header(c, tema, rotulo, null);
            if (progreso >= 0)
            {
                var track = UIKit.Image(c, new Color(1, 1, 1, 0.08f), "Progreso", false);
                Tema.Redondear(track, 4);
                UIKit.Size(track.gameObject, 8);
                var fill = UIKit.Image(track.transform, tema.acento, "Relleno", false);
                Tema.Redondear(fill, 4);
                var frt = (RectTransform)fill.transform;
                frt.anchorMin = Vector2.zero; frt.anchorMax = new Vector2(Mathf.Clamp01(progreso), 1);
                frt.offsetMin = frt.offsetMax = Vector2.zero;
            }
            if (ctx)
            {
                var row = UIKit.HRow(c, 0, 24);
                UIKit.Size(row.gameObject, -1, 1);
                var card = UIKit.Image(row, new Color(0, 0, 0, 0.28f), "Poema", false);
                Tema.Redondear(card, 14);
                UIKit.Size(card.gameObject, -1, -1, 640, 0);
                var left = UIKit.Scroll(card.transform);
                UIKit.Stretch((RectTransform)left.parent.parent, 22, 18, 12, 18);
                UIKit.Text(left, contexto, 23, TextAlignmentOptions.TopLeft, tema.texto);
                var rightRt = UIKit.Make("Derecha", row);
                UIKit.VList(rightRt, 10);
                UIKit.Size(rightRt.gameObject, -1, -1, 760, 1);
                FillQuestion(rightRt, enunciado, opciones, root.gameObject, onAnswer, tema);
            }
            else FillQuestion(c, enunciado, opciones, root.gameObject, onAnswer, tema);
            if (!string.IsNullOrEmpty(footer)) UIKit.Text(c, footer, 20, TextAlignmentOptions.Left, tema.tenue);
            if (onHint != null)
            {
                var hint = UIKit.Text(c, "", 22, TextAlignmentOptions.Left, tema.acento);
                var row = UIKit.HRow(c, 54, 12);
                UIKit.Secondary(row, "Cerrar", () => CloseModal(root.gameObject), tema, 0, 22);
                UIKit.Secondary(row, "Pedir pista", () =>
                {
                    var h = onHint();
                    hint.text = h ?? "Ya viste las tres pistas. Si no sale, Don Ruperto puede mostrar el camino.";
                }, tema, 0, 22);
            }
            PushModal(root.gameObject);
            return root.gameObject;
        }

        void FillQuestion(Transform c, string enunciado, List<string> opciones, GameObject root, Action<string> onAnswer, Tema tema)
        {
            UIKit.Text(c, enunciado, 30, TextAlignmentOptions.TopLeft, tema.texto);
            for (int i = 0; i < opciones.Count; i++)
            {
                string op = opciones[i];
                var b = UIKit.Secondary(c, op, () => { CloseModal(root); onAnswer?.Invoke(op); }, tema, 66, 24);
                UIKit.Label(b).alignment = TextAlignmentOptions.MidlineLeft;
                UIKit.Chip(b, ((char)('A' + i)).ToString(), tema);
            }
        }

        // ---------- Subtítulos (diálogos lineales) ----------
        public void PlayDialogue(string id, Action onDone = null)
        {
            var d = ContentDatabase.Instance.Dialogo(id);
            if (d == null || d.lineas == null || d.lineas.Length == 0) { onDone?.Invoke(); return; }
            PlayLines(d.lineas, onDone);
        }

        /// Tema de cada voz: El Retrato habla en oro, Fran y su app en celeste, el liceo en ocre.
        public static Tema TemaDe(string quien)
        {
            if (quien == "El Retrato" || quien == "Esqueleto") return Tema.Retrato;
            if (quien == "Fran" || quien == "Repasa+") return Tema.App;
            return Tema.Liceo;
        }

        public void PlayLines(Linea[] lineas, Action onDone = null)
        {
            if (Core.DevTools.AutoPlay) { onDone?.Invoke(); return; }
            var root = UIKit.Stretch(UIKit.Make("Dialogo", Panels));
            var catcher = root.gameObject.AddComponent<UnityEngine.UI.Image>();
            catcher.color = new Color(0, 0, 0, 0.25f);
            var bar = UIKit.Image(root, new Color(0.05f, 0.045f, 0.04f, 0.95f), "Barra");
            Tema.Redondear(bar, 18);
            var brt = (RectTransform)bar.transform;
            brt.anchorMin = new Vector2(0.14f, 0); brt.anchorMax = new Vector2(0.86f, 0); brt.pivot = new Vector2(0.5f, 0);
            brt.sizeDelta = new Vector2(0, 210); brt.anchoredPosition = new Vector2(0, 46);
            // Placa con el nombre de quien habla
            var plate = UIKit.Image(bar.transform, Tema.Liceo.acento, "Placa", false);
            Tema.Redondear(plate, 12);
            var prt = (RectTransform)plate.transform;
            prt.anchorMin = prt.anchorMax = new Vector2(0, 1); prt.pivot = new Vector2(0, 0.5f);
            prt.sizeDelta = new Vector2(260, 44); prt.anchoredPosition = new Vector2(30, 0);
            var who = UIKit.Text(plate.transform, "", 22, TextAlignmentOptions.Center, Tema.Liceo.textoSobreAcento, "Quien");
            who.fontStyle = FontStyles.Bold;
            UIKit.Stretch(who.rectTransform, 12, 0, 12, 0);
            var txt = UIKit.Text(bar.transform, "", 29, TextAlignmentOptions.TopLeft, UIKit.Texto, "Linea");
            UIKit.Stretch(txt.rectTransform, 34, 40, 34, 36);
            var hint = UIKit.Text(bar.transform, "Clic · Espacio · E  para continuar", 17, TextAlignmentOptions.BottomRight, UIKit.Tenue, "Ayuda");
            UIKit.Stretch(hint.rectTransform, 30, 170, 26, 12);
            var runner = root.gameObject.AddComponent<DialogueRunner>();
            runner.Init(lineas, who, txt, () => { CloseModal(root.gameObject); onDone?.Invoke(); });
            runner.OnLine = quien =>
            {
                var t = TemaDe(quien);
                bool narrador = string.IsNullOrEmpty(quien);
                plate.gameObject.SetActive(!narrador);
                plate.color = t.acento;
                who.color = t.textoSobreAcento;
                txt.fontStyle = narrador ? FontStyles.Italic : FontStyles.Normal;
                txt.color = narrador ? UIKit.Tenue : t.texto;
            };
            runner.ShowFirst();
            PushModal(root.gameObject);
        }

        // ---------- Avisos ----------
        public void Toast(string text, float seconds = 5f, bool retrato = false)
        {
            var t = retrato ? Tema.Retrato : Tema.App;
            var img = UIKit.Image(toasts, new Color(t.panel.r, t.panel.g, t.panel.b, 0.94f), "Aviso", false);
            Tema.Redondear(img, 14);
            var o = img.gameObject.AddComponent<UnityEngine.UI.Outline>();
            o.effectColor = new Color(t.acento.r, t.acento.g, t.acento.b, 0.5f);
            o.effectDistance = new Vector2(1.5f, -1.5f);
            var v = UIKit.VList((RectTransform)img.transform, 4, 16);
            v.childAlignment = TextAnchor.UpperLeft;
            img.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>().verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            var k = UIKit.Text(img.transform, retrato ? "STORY · EL RETRATO" : "CELULAR DE FRAN", 15, TextAlignmentOptions.Left, t.acento);
            k.characterSpacing = 5; k.fontStyle = FontStyles.Bold;
            UIKit.Text(img.transform, text, 22, TextAlignmentOptions.Left, t.texto);
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
