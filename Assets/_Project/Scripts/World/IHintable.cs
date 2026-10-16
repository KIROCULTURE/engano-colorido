namespace EnganoColorido.World
{
    /// Tarea con pistas en 3 niveles (para la app «Pensar»).
    public interface IHintable
    {
        string HintTitle { get; }
        int HintsShown { get; }
        string Hint(int level);
        /// Revela la siguiente pista y la registra. Devuelve null si no quedan.
        string RequestHint();
    }
}
