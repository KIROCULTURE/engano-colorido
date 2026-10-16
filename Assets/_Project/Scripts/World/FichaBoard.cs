using EnganoColorido.Core;
using EnganoColorido.Interaction;
using EnganoColorido.UI;

namespace EnganoColorido.World
{
    /// Notas de la profe Inés: diálogo opcional antes, fichas en cadena, diálogo después y un flag.
    public class FichaBoard : Interactable
    {
        public string[] fichas;
        public string dialogoAntes;
        public string dialogoDespues;
        public string flag;

        public override void Interact()
        {
            var s = Game.State;
            bool first = s == null || !s.Flag(flag);
            void Fichas() => Game.ShowFichas(fichas, After);
            void After()
            {
                if (first && !string.IsNullOrEmpty(dialogoDespues)) UIManager.I.PlayDialogue(dialogoDespues);
                s?.SetFlag(flag);
            }
            if (!first)
            {
                Game.ShowFichas(fichas, null, false); // releer
                return;
            }
            if (!string.IsNullOrEmpty(dialogoAntes)) UIManager.I.PlayDialogue(dialogoAntes, Fichas);
            else Fichas();
        }
    }
}
