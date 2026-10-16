using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.UI;
using UnityEngine;

namespace EnganoColorido.Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        public string prompt = "Interactuar";
        public virtual string Prompt => prompt;
        public virtual bool CanInteract => true;
        public abstract void Interact();
    }
}
