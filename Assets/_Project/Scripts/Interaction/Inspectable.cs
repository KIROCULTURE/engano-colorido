using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.UI;
using UnityEngine;

namespace EnganoColorido.Interaction
{
    public class Inspectable : Interactable
    {
        public string titulo;
        [TextArea] public string texto;
        public string fichaId;
        public string poemaId;
        public string flagAlLeer;

        public override void Interact()
        {
            var db = ContentDatabase.Instance;
            var s = Game.State;
            if (!string.IsNullOrEmpty(fichaId))
            {
                var f = db.Ficha(fichaId);
                if (f != null) { Game.ShowFicha(f, Done); return; }
            }
            string body = texto;
            string title = titulo;
            if (!string.IsNullOrEmpty(poemaId))
            {
                var p = db.Poema(poemaId);
                if (p != null)
                {
                    s?.AddUnique(s.poemas, p.id);
                    title = string.IsNullOrEmpty(title) ? p.titulo : title;
                    body = (string.IsNullOrEmpty(texto) ? "" : texto + "\n\n") + ContentDatabase.PoemaTexto(p);
                }
            }
            UIManager.I.ShowText(title, body, "Cerrar", Done);
        }

        void Done()
        {
            if (!string.IsNullOrEmpty(flagAlLeer)) Game.State?.SetFlag(flagAlLeer);
        }
    }
}
