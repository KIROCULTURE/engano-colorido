using System;
using EnganoColorido.Data;
using EnganoColorido.Save;
using EnganoColorido.UI;

namespace EnganoColorido.Core
{
    /// Acceso corto al estado de la sesión y a acciones comunes.
    public static class Game
    {
        public static SaveData State;
        public static ContentDatabase DB => ContentDatabase.Instance;

        /// Muestra una ficha de la profe Inés y la guarda en el Cuaderno.
        public static void ShowFicha(Ficha f, Action onClose = null)
        {
            State?.AddUnique(State.fichas, f.id);
            UIManager.I.ShowText(f.titulo,
                f.texto + "\n\n<size=80%><color=#A09A90>Queda guardada en tu Cuaderno (celular → Cuaderno).</color></size>",
                "Entendido", onClose, 1000, 720, Tema.Liceo, $"Ficha de la profe Inés · texto escolar p. {f.pagina}");
        }

        /// Muestra fichas en cadena (por defecto solo las que aún no se han leído).
        public static void ShowFichas(string[] ids, Action then, bool soloPendientes = true, int i = 0)
        {
            if (ids == null || i >= ids.Length) { then?.Invoke(); return; }
            var f = DB.Ficha(ids[i]);
            if (f == null || (soloPendientes && State != null && State.fichas.Contains(ids[i]))) { ShowFichas(ids, then, soloPendientes, i + 1); return; }
            ShowFicha(f, () => ShowFichas(ids, then, soloPendientes, i + 1));
        }

        public static void Notify(string dialogueId)
        {
            var d = DB.Dialogo(dialogueId);
            if (d == null) return;
            foreach (var l in d.lineas)
            {
                State?.historias.Add(l.quien + ": " + l.texto);
                UIManager.I.Toast($"<b>{l.quien}</b> publicó una story\n{l.texto}", 6f, true);
            }
        }
    }
}
