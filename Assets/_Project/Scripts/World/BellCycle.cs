using UnityEngine;

namespace EnganoColorido.World
{
    /// La campana del liceo suena cada cierto tiempo y alterna Niebla ↔ Dorado (P1-B: el momento justo).
    public class BellCycle : MonoBehaviour
    {
        public float intervalo = 20f;
        public float duracionCambio = 2.5f;
        public bool activo;
        float next;

        public void Activate(bool on)
        {
            activo = on;
            next = Time.time + 4f;
            if (!on && WorldManager.I != null && WorldManager.I.Dorado) WorldManager.I.RingBell(false, duracionCambio);
        }

        void Update()
        {
            if (!activo || WorldManager.I == null || Time.time < next) return;
            next = Time.time + intervalo;
            WorldManager.I.RingBell(!WorldManager.I.Dorado, duracionCambio);
        }
    }
}
