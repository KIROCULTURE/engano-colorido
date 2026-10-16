using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.Interaction
{
    public class Door : Interactable
    {
        public string destino = "Siguiente zona";
        [TextArea] public string mensajeCerrada = "Está cerrada.";
        public bool siempreCerrada;   // puertas de zonas que aún no existen (modo 90)

        public override string Prompt => siempreCerrada ? destino + " (cerrada)" : "Ir a: " + destino;

        public override void Interact()
        {
            if (siempreCerrada) { UIManager.I.Toast(mensajeCerrada, 4f); return; }
            var z = ZoneController.I;
            if (z != null && !z.AllDone)
            {
                UIManager.I.Toast(mensajeCerrada + "\n<size=80%>" + z.Objective + "</size>", 4f);
                return;
            }
            GameFlow.I.ExitScene();
        }
    }
}
