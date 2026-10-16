using System.Collections.Generic;
using EnganoColorido.Data;

namespace EnganoColorido.Session
{
    /// Lógica pura del control de ritmo (GDD 4.3). El tiempo entra por Tick(dt), así se puede testear.
    public class SessionPacer
    {
        readonly SesionConfig cfg;
        readonly Modo modo;
        public float Elapsed { get; private set; }
        public float ZoneElapsed { get; private set; }
        public string Zone { get; private set; }

        public SessionPacer(SesionConfig cfg, int minutos, float elapsed = 0)
        {
            this.cfg = cfg;
            foreach (var m in cfg.modos) if (m.minutos == minutos) modo = m;
            if (modo == null) throw new System.ArgumentException("Modo no configurado: " + minutos);
            Elapsed = elapsed;
        }

        public int Minutos => modo.minutos;
        public float PlannedTotalMinutes { get { float t = 0; foreach (var p in modo.zonas) t += p.minutos; return t; } }
        public void Tick(float dt) { Elapsed += dt; ZoneElapsed += dt; }
        public void EnterZone(string zone) { Zone = zone; ZoneElapsed = 0; }
        public void RestoreZone(string zone, float zoneElapsed) { Zone = zone; ZoneElapsed = zoneElapsed; }

        public float BudgetMinutes(string zone)
        {
            foreach (var p in modo.zonas) if (p.zona == zone) return p.minutos;
            return 0;
        }

        public bool InRoute(string zone)
        {
            foreach (var p in modo.zonas) if (p.zona == zone) return true;
            return false;
        }

        /// Fracción del presupuesto de la zona actual (1 = 100 %).
        public float ZoneFraction
        {
            get
            {
                float b = BudgetMinutes(Zone) * 60f;
                return b <= 0 ? 0 : ZoneElapsed / b;
            }
        }

        public bool HintsAdvanced => ZoneFraction >= cfg.umbralPistas;
        public bool GuidedOffered => ZoneFraction >= cfg.umbralGuiado;

        /// Minuto en que la zona debería empezar según el plan.
        public float PlannedStartMinutes(string zone)
        {
            float t = 0;
            foreach (var p in modo.zonas) { if (p.zona == zone) return t; t += p.minutos; }
            return t;
        }

        int OrderIndex(string zone)
        {
            for (int i = 0; i < cfg.zonas.Length; i++) if (cfg.zonas[i].id == zone) return i;
            return -1;
        }

        public bool BeforeTheater(string zone) => OrderIndex(zone) >= 0 && OrderIndex(zone) < OrderIndex("teatro");
        public bool ShouldForceTheater => Elapsed >= modo.envioTeatroMin * 60f && BeforeTheater(Zone);
        public bool ShouldForceExitTest => Elapsed >= modo.salidaMin * 60f && Zone != "salida";

        bool IsOptional(string zone)
        {
            foreach (var z in cfg.zonas) if (z.id == zone) return z.opcional;
            return false;
        }

        /// Una zona se salta si no está en el modo, o si es opcional y la sesión va atrasada.
        public bool IsSkipped(string zone)
        {
            if (!InRoute(zone)) return true;
            return IsOptional(zone) && Elapsed / 60f > PlannedStartMinutes(zone) + cfg.toleranciaAtrasoMin;
        }

        /// Siguiente zona en el orden del liceo; agrega a 'skipped' las que se saltan.
        public string NextZone(string current, List<string> skipped = null)
        {
            int i = OrderIndex(current);
            for (int j = i + 1; j < cfg.zonas.Length; j++)
            {
                var id = cfg.zonas[j].id;
                if (IsSkipped(id)) { skipped?.Add(id); continue; }
                return id;
            }
            return "salida";
        }

        public string SceneFor(string zone)
        {
            foreach (var z in cfg.zonas) if (z.id == zone) return z.escena;
            return null;
        }
    }
}
