using EnganoColorido.Core;
using EnganoColorido.Interaction;
using EnganoColorido.Player;
using EnganoColorido.UI;
using EnganoColorido.World;
using TMPro;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// H · Las tres radios: se gira el dial entre la estática hasta escuchar un fragmento (subtitulado).
    /// El texto del fragmento sale del hueco correspondiente del puzzle H en puzzles.json.
    public class RadioTuner : Interactable
    {
        public PuzzleController tablero;
        public int indice;
        public float estacion = 50f;
        public string poemaId;
        const float Tolerancia = 4f;

        string Flag => "radio_" + indice;
        public override string Prompt => (Game.State != null && Game.State.Flag(Flag) ? "Volver a escuchar la radio " : "Sintonizar la radio ") + (indice + 1);

        public override void Interact()
        {
            var def = Game.DB.Puzzle(tablero != null ? tablero.puzzleId : "H");
            Game.ShowFichas(def?.fichas, OpenPanel);
        }

        void OpenPanel()
        {
            var def = Game.DB.Puzzle(tablero != null ? tablero.puzzleId : "H");
            string fragmento = def != null && indice < def.pasos[0].slots.Length ? def.pasos[0].slots[indice].etiqueta : "";
            int dash = fragmento.IndexOf("—");
            if (dash >= 0) fragmento = fragmento.Substring(dash + 1).Trim();

            var c = UIKit.Window(UIManager.I.Panels, 1100, 560, out var root, "Radio");
            UIKit.Text(c, $"Radio {indice + 1}", 34, TextAlignmentOptions.Left, UIKit.Oro);
            UIKit.Text(c, "Gira el dial hasta que la voz se escuche entre la estática.", 22, TextAlignmentOptions.Left, UIKit.Tenue);
            var estado = UIKit.Text(c, "", 26);
            UIKit.Size(estado.gameObject, 150);
            var ps = PlayerController.I != null ? PlayerController.I.GetComponent<PhoneStatic>() : null;
            bool tuned = Game.State.Flag(Flag);
            void OnDial(float v)
            {
                float d = Mathf.Abs(v - estacion);
                if (ps) ps.extra = d < Tolerancia ? 0.05f : Mathf.Clamp01(d / 40f) * 0.8f + 0.1f;
                if (d < Tolerancia)
                {
                    estado.text = $"<i>{fragmento}</i>";
                    if (!Game.State.Flag(Flag))
                    {
                        Game.State.SetFlag(Flag);
                        Game.State.AddUnique(Game.State.poemas, poemaId);
                        UIManager.I.Toast($"Radio {indice + 1} sintonizada. El fragmento quedó en tu Cuaderno.", 3f);
                    }
                }
                else estado.text = d < 15 ? "<color=#A09A90>…kshhh… una voz… kshhh…</color>" : "<color=#A09A90>kshhhhhhhhhhhhh</color>";
            }
            UIKit.Slider(c, 0, 100, tuned ? estacion : (estacion > 50 ? 5 : 95), OnDial, 44);
            OnDial(tuned ? estacion : (estacion > 50 ? 5 : 95));
            UIKit.Button(c, "Dejar la radio", () => { if (ps) ps.extra = 0; UIManager.I.CloseModal(root.gameObject); }, 56, 24);
            UIManager.I.PushModal(root.gameObject);
        }
    }
}
