using System;
using EnganoColorido.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnganoColorido.UI
{
    public class DialogueRunner : MonoBehaviour
    {
        Linea[] lines; int i = -1; TextMeshProUGUI who, txt; Action done; float shownAt;

        public void Init(Linea[] l, TextMeshProUGUI w, TextMeshProUGUI t, Action d)
        {
            lines = l; who = w; txt = t; done = d;
            var btn = gameObject.AddComponent<UnityEngine.UI.Button>();
            btn.transition = UnityEngine.UI.Selectable.Transition.None;
            btn.onClick.AddListener(Next);
            Next();
        }

        void Next()
        {
            if (Time.unscaledTime - shownAt < 0.2f && i >= 0) return;
            i++;
            if (i >= lines.Length) { done?.Invoke(); done = null; return; }
            who.text = lines[i].quien;
            txt.text = lines[i].texto;
            shownAt = Time.unscaledTime;
        }

        void Update()
        {
            var k = Keyboard.current;
            if (k != null && (k.spaceKey.wasPressedThisFrame || k.eKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame)) Next();
        }
    }
}
