using EnganoColorido.Interaction;

namespace EnganoColorido.Puzzles
{
    /// Objeto del mundo que abre un puzzle (candado, mesa, estante, libreto…).
    public class PuzzleTrigger : Interactable
    {
        public PuzzleController puzzle;
        public override string Prompt => puzzle != null && puzzle.Done ? prompt + " (resuelto)" : prompt;
        public override void Interact() { if (puzzle != null) puzzle.Open(); }
    }
}
