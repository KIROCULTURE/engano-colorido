using System.Collections.Generic;

namespace EnganoColorido.Core
{
    /// Nombres de los objetos del inventario.
    public static class Items
    {
        static readonly Dictionary<string, (string nombre, string desc)> all = new()
        {
            { "celular", ("Celular", "Linterna, Cuaderno, Mapa y Pensar.") },
            { "rosa", ("Rosa", "Cortada en el instante justo del cambio de mundo.") },
            { "calavera", ("Calavera", "Del laboratorio de biología. De yeso, por suerte.") },
            { "reloj", ("Reloj de arena", "De la sala de profesores. La arena no cae.") },
            { "corona", ("Corona", "Utilería del teatro. Pesa más de lo que parece.") },
            { "paloma", ("Paloma de cerámica", "Un adorno de la galería.") },
            { "ancla", ("Ancla", "Un modelo del laboratorio.") },
            { "llave", ("Llave antigua", "No abre nada de aquí.") },
            { "pelota", ("Pelota", "Alguien la dejó en la bodega.") },
        };

        public static string Name(string id) => all.TryGetValue(id, out var v) ? v.nombre : id;
        public static string Desc(string id) => all.TryGetValue(id, out var v) ? v.desc : "";
    }
}
