using EnganoColorido.UI;
using EnganoColorido.World;

namespace EnganoColorido.Interaction
{
    /// Personaje con el que se conversa (Don Ruperto): repite su diálogo y recuerda el objetivo actual.
    public class Talk : Interactable
    {
        public string dialogo;

        public override void Interact()
        {
            UIManager.I.PlayDialogue(dialogo, () =>
            {
                var z = ZoneController.I;
                if (z != null) UIManager.I.Toast("Objetivo: " + z.Objective, 5f);
            });
        }
    }
}
