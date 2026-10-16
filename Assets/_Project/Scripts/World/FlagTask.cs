using EnganoColorido.Core;

namespace EnganoColorido.World
{
    /// Tarea que se cumple cuando existe un flag (por ejemplo, leer las notas del espejo).
    public class FlagTask : ZoneTask
    {
        public string flag;
        public override bool Done => Game.State != null && Game.State.Flag(flag);
        public override void ForceComplete() => Game.State?.SetFlag(flag);
    }
}
