using EnganoColorido.Core;
using EnganoColorido.Interaction;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// P4-C · Escena de El mágico prodigioso: una figura velada con la ropa de Fran; bajo el velo, un esqueleto.
    /// Se cuenta con texto, un cambio de luz y un sonido (sin sangre, sin susto brusco).
    public class VeiledFigure : ZoneTask
    {
        public PuzzleController requiere;  // P4
        public GameObject velo, esqueleto;
        public Light luz;
        public string flag = "velo";
        public string ficha = "magico";
        public string poemaId = "calderon_magico";

        public override bool Done => Game.State != null && Game.State.Flag(flag);

        void Start()
        {
            if (Done) Reveal(false);
            else if (esqueleto) esqueleto.SetActive(false);
            var it = gameObject.AddComponent<VeilInteract>();
            it.owner = this;
        }

        public void Interact()
        {
            if (Done) { UIManager.I.PlayDialogue("galeria_velo"); return; }
            if (requiere != null && !requiere.Done) { UIManager.I.Toast("Una figura cubierta con un velo. Lleva tu ropa. Algo te dice que primero ordenes los retratos.", 5f); return; }
            Reveal(true);
        }

        void Reveal(bool scene)
        {
            Game.State.SetFlag(flag);
            Game.State.AddUnique(Game.State.poemas, poemaId);
            if (velo) velo.SetActive(false);
            if (esqueleto) esqueleto.SetActive(true);
            if (luz) { luz.color = new Color(0.85f, 0.9f, 1f); luz.intensity = 2.2f; }
            if (!scene) return;
            ProceduralAudio.PlayOneShot(ProceduralAudio.Tone, 0.7f);
            UIManager.I.PlayDialogue("galeria_velo", () =>
            {
                var f = Game.DB.Ficha(ficha);
                if (f != null) Game.ShowFicha(f);
            });
        }

        public override void ForceComplete()
        {
            Game.State.AddUnique(Game.State.fichas, ficha);
            Reveal(false);
        }
    }
}
