using UnityEngine;

namespace EnganoColorido.World
{
    /// Tarea de una zona (puzzle, lectura, escena). La puerta de salida se abre cuando todas están hechas.
    public abstract class ZoneTask : MonoBehaviour
    {
        public string zona;
        public string objetivo = "Explora la sala.";
        public abstract bool Done { get; }
        /// Completa la tarea por modo guiado (registrado como "con ayuda") o por autoplay.
        public abstract void ForceComplete();
        public virtual string Objective => objetivo;
    }
}
