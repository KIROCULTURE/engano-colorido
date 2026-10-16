using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.UI;
using UnityEngine;

namespace EnganoColorido.Interaction
{
    public class Pickup : Interactable
    {
        public string itemId;

        void Start()
        {
            if (Game.State != null && Game.State.inventario.Contains(itemId)) gameObject.SetActive(false);
        }

        public override string Prompt => "Recoger: " + Items.Name(itemId);

        public override void Interact()
        {
            Game.State?.AddUnique(Game.State.inventario, itemId);
            UIManager.I.Toast("Inventario: + " + Items.Name(itemId), 3f);
            gameObject.SetActive(false);
        }
    }
}
