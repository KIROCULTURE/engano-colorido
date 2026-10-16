using EnganoColorido.Core;
using EnganoColorido.Interaction;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// P1-B, carpe diem jugable: botón en la Niebla, marchita en el Dorado, florece ~4 s durante el cambio de mundo.
    public class RoseBush : Interactable
    {
        public PuzzleController puzzle;
        public GameObject boton, flor, marchita;
        public float ventana = 4f;
        float bloomUntil = -1;

        void Start()
        {
            if (WorldManager.I != null) WorldManager.I.TransitionStarted += OnTransition;
            Show();
        }

        void OnDestroy() { if (WorldManager.I != null) WorldManager.I.TransitionStarted -= OnTransition; }

        void OnTransition(bool toDorado) => bloomUntil = Time.time + ventana;

        bool Blooming => Time.time < bloomUntil;

        void Update() => Show();

        void Show()
        {
            bool dorado = WorldManager.I != null && WorldManager.I.Dorado;
            bool bloom = Blooming;
            if (flor) flor.SetActive(bloom);
            if (boton) boton.SetActive(!bloom && !dorado);
            if (marchita) marchita.SetActive(!bloom && dorado);
        }

        public override bool CanInteract => puzzle != null && puzzle.PanelsDone && !puzzle.Done;
        public override string Prompt => Blooming ? "¡Cortar la rosa ahora!" : "Mirar el rosal";

        public override void Interact()
        {
            if (Blooming)
            {
                Game.State.AddUnique(Game.State.inventario, "rosa");
                UIManager.I.Toast("Inventario: + Rosa", 3f);
                puzzle.CompleteWorld();
                return;
            }
            bool dorado = WorldManager.I != null && WorldManager.I.Dorado;
            UIManager.I.Toast(dorado
                ? "Está marchita. En el Mundo Dorado todo brilla… menos esto."
                : "Es solo un botón. Florecerá un instante cuando suene la campana: hay que aprovechar el momento.", 4f);
        }
    }
}
