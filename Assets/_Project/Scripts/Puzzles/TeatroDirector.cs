using EnganoColorido.Core;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// Encadena el cierre en el Teatro: Juicio → Tu verso → elección del final.
    public class TeatroDirector : MonoBehaviour
    {
        public PuzzleController juicio, tuVerso;
        public FinalTask final;

        void Start()
        {
            if (juicio) juicio.OnSolved += () => { if (!DevTools.AutoPlay && tuVerso && !tuVerso.Done) tuVerso.Open(); };
            if (tuVerso) tuVerso.OnSolved += () => { if (!DevTools.AutoPlay && final) final.Offer(); };
        }
    }
}
