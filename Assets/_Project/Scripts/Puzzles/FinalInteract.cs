using EnganoColorido.Interaction;

namespace EnganoColorido.Puzzles
{
    /// Punto del escenario para responder a El Retrato (si la cadena automática se interrumpió).
    public class FinalInteract : Interactable
    {
        public FinalTask final;
        public override bool CanInteract => final != null && !final.Done;
        public override void Interact() => final?.Offer();
    }
}
