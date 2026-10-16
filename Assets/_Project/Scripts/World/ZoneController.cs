using System.Linq;
using EnganoColorido.Core;
using UnityEngine;

namespace EnganoColorido.World
{
    /// Uno por escena: qué zonas (de la tabla 4.1) contiene, sus tareas y su presentación.
    public class ZoneController : MonoBehaviour
    {
        public static ZoneController I { get; private set; }
        public string[] zonas = { "camarin" };
        public string titulo = "Camarín";
        public string intro;
        public string notif;
        public ZoneTask[] tasks = new ZoneTask[0];

        void Awake() { I = this; }
        void Start() { GameFlow.Ensure().OnZoneLoaded(this); }

        public bool ZoneDone(string z) => tasks.Where(t => t != null && t.zona == z).All(t => t.Done);
        public bool AllDone => tasks.All(t => t == null || t.Done);
        public ZoneTask CurrentTask => tasks.FirstOrDefault(t => t != null && !t.Done);
        public string Objective => CurrentTask != null ? CurrentTask.Objective : "Busca la salida de esta zona.";
    }
}
