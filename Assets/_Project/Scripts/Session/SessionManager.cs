using EnganoColorido.Core;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.Session
{
    /// Reloj de sesión y control de ritmo (GDD 4.3): presupuestos por zona, pistas adelantadas al 100 %,
    /// modo guiado al 130 %, salto de zonas opcionales y envío forzado al Teatro y a la prueba de salida.
    public class SessionManager : MonoBehaviour
    {
        public static SessionPacer Pacer { get; private set; }
        static string hintedZone, lastTask;
        static float nextOffer;

        public static void Begin(int modo, float elapsed)
        {
            Pacer = new SessionPacer(Game.DB.sesion, modo, elapsed);
            hintedZone = null; lastTask = null; nextOffer = 0;
        }

        public static void Stop() { Pacer = null; }

        void Update()
        {
            if (Pacer == null) return;
            Pacer.Tick(Time.unscaledDeltaTime);
            var flow = GameFlow.I;
            if (flow == null) return;

            if (Pacer.ShouldForceExitTest && Pacer.Zone != "diagnostico") { flow.ForceExitTest(); return; }
            if (!GameFlow.InGameplay) return;
            if (Pacer.ShouldForceTheater) { flow.ForceTheater(); return; }

            var zc = ZoneController.I;
            if (zc == null || UIManager.I.AnyModal) return;
            var task = zc.CurrentTask;
            if (task == null) return;

            // 100 %: las pistas se adelantan (una vez por zona).
            if (Pacer.HintsAdvanced && hintedZone != Pacer.Zone && task is IHintable h)
            {
                hintedZone = Pacer.Zone;
                if (h.HintsShown < 3) h.RequestHint();
                UIManager.I.PlayDialogue("ruperto_pistas");
                return;
            }

            // 130 %: Don Ruperto ofrece mostrar el camino (modo guiado, queda registrado "con ayuda").
            if (Pacer.GuidedOffered && Time.unscaledTime >= nextOffer)
            {
                if (lastTask != task.name) { lastTask = task.name; }
                nextOffer = Time.unscaledTime + 60f;
                OfferGuided(task);
            }
        }

        public static void OfferGuided(ZoneTask task)
        {
            var d = Game.DB.Dialogo("ruperto_guiado");
            string line = d != null && d.lineas.Length > 0 ? d.lineas[0].texto : "¿Le muestro el camino?";
            UIManager.I.ShowChoice("Don Ruperto", line + "\n\n<size=80%><color=#A09A90>" + task.Objective + "</color></size>",
                new[] { "Sí, muéstrame", "Sigo intentando" },
                i => { if (i == 0) task.ForceComplete(); });
        }
    }
}
