using EnganoColorido.Core;
using EnganoColorido.UI;
using EnganoColorido.World;

namespace EnganoColorido.Puzzles
{
    /// Los dos finales (GDD 3.4). Ninguno mata a Fran; ambos llevan a la prueba de salida. La elección se registra.
    public class FinalTask : ZoneTask
    {
        public PuzzleController requiere; // Tu verso
        bool running;

        public override bool Done => Game.State != null && !string.IsNullOrEmpty(Game.State.final);

        public void Offer()
        {
            if (Done || running) return;
            if (requiere != null && !requiere.Done) { UIManager.I.Toast("Antes, escribe tu verso en el escritorio del escenario.", 4f); return; }
            UIManager.I.PlayDialogue("final_eleccion", () =>
                UIManager.I.ShowChoice("¿Despertar o quedarse?",
                    "Despertar es desengañarse: volver al mundo real, con su tiempo y sus arrugas.\nQuedarse es seguir en el Mundo Dorado, donde todo brilla y nada pesa.",
                    new[] { "Despertar", "Quedarme: dulce soñar" }, Choose, 1000, Tema.Retrato, "El Retrato"));
        }

        void Choose(int i)
        {
            running = true;
            var s = Game.State;
            if (i == 0)
            {
                s.final = "despertar";
                if (WorldManager.I != null) WorldManager.I.Dawn(4f);
                UIManager.I.PlayDialogue("final_despertar", () =>
                    UIManager.I.PlayDialogue("final_foto", () =>
                        UIManager.I.ShowChoice("La foto sin filtro", "", new[] { "Publicarla", "No publicarla" }, f =>
                        {
                            s.foto = f == 0 ? "si" : "no";
                            End();
                        })));
            }
            else
            {
                s.final = "sonar";
                if (WorldManager.I != null) WorldManager.I.SetWorld(true, 4f);
                UIManager.I.PlayDialogue("final_sonar", End);
            }
        }

        void End() => GameFlow.I.CompleteZones(new[] { "teatro" });

        /// El modo guiado no elige por la persona: solo vuelve a ofrecer la decisión.
        public override void ForceComplete()
        {
            if (DevTools.AutoPlay)
            {
                if (DevTools.Arg("-devfinal") == "sonar") { Choose(1); return; }
                Game.State.final = "despertar"; Game.State.foto = "no"; End(); return;
            }
            Offer();
        }

        public override string Objective => "Responde a El Retrato: ¿despiertas o te quedas?";
    }
}
