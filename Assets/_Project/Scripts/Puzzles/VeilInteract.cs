using EnganoColorido.Interaction;

namespace EnganoColorido.Puzzles
{
    /// Parte interactuable de la figura velada (separada para no mezclar ZoneTask e Interactable).
    public class VeilInteract : Interactable
    {
        public VeiledFigure owner;
        public override string Prompt => owner != null && owner.Done ? "Mirar el esqueleto" : "Levantar el velo";
        public override void Interact() => owner?.Interact();
    }
}
