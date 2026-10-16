using System;
using System.Collections.Generic;

namespace EnganoColorido.Puzzles
{
    /// Interfaz común de los puzzles (CLAUDE.md): validar, pistas por nivel, aviso al resolver y modo guiado.
    public interface IPuzzle
    {
        string Id { get; }
        bool Validate(IList<string> answers, out string feedback);
        string GetHint(int level);
        event Action OnSolved;
        void SolveGuided();
    }
}
