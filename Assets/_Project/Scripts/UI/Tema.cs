using UnityEngine;

namespace EnganoColorido.UI
{
    /// Tres familias visuales, cada una ligada a una voz del juego:
    ///   Liceo   → la profe Inés y los acertijos del liceo (papel, madera y ocre).
    ///   App     → el celular de Fran y la app Repasa+ (pantalla fría, celeste).
    ///   Retrato → El Retrato y el Mundo Dorado (pan de oro).
    public sealed class Tema
    {
        public Color panel, superficie, superficieSel, acento, texto, tenue, textoSobreAcento;

        public static readonly Tema Liceo = new Tema
        {
            panel = new Color(0.115f, 0.098f, 0.082f, 0.98f),
            superficie = new Color(0.205f, 0.175f, 0.145f, 1f),
            superficieSel = new Color(0.36f, 0.27f, 0.13f, 1f),
            acento = new Color(0.90f, 0.72f, 0.40f, 1f),
            texto = new Color(0.95f, 0.91f, 0.84f, 1f),
            tenue = new Color(0.68f, 0.62f, 0.55f, 1f),
            textoSobreAcento = new Color(0.13f, 0.09f, 0.04f, 1f),
        };

        public static readonly Tema App = new Tema
        {
            panel = new Color(0.068f, 0.078f, 0.105f, 0.98f),
            superficie = new Color(0.135f, 0.155f, 0.205f, 1f),
            superficieSel = new Color(0.15f, 0.30f, 0.37f, 1f),
            acento = new Color(0.50f, 0.82f, 0.93f, 1f),
            texto = new Color(0.92f, 0.95f, 0.98f, 1f),
            tenue = new Color(0.58f, 0.65f, 0.73f, 1f),
            textoSobreAcento = new Color(0.03f, 0.08f, 0.11f, 1f),
        };

        public static readonly Tema Retrato = new Tema
        {
            panel = new Color(0.135f, 0.095f, 0.035f, 0.98f),
            superficie = new Color(0.25f, 0.185f, 0.075f, 1f),
            superficieSel = new Color(0.50f, 0.37f, 0.12f, 1f),
            acento = new Color(1f, 0.84f, 0.47f, 1f),
            texto = new Color(1f, 0.95f, 0.86f, 1f),
            tenue = new Color(0.78f, 0.68f, 0.52f, 1f),
            textoSobreAcento = new Color(0.16f, 0.10f, 0.02f, 1f),
        };

        // ---------------------------------------------------------- esquinas redondeadas
        static Sprite _round;

        /// Sprite 9-slice con esquinas redondeadas, generado en memoria (sin assets).
        public static Sprite Round
        {
            get
            {
                if (_round != null) return _round;
                const int s = 48, r = 16;
                var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
                var px = new Color32[s * s];
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                    {
                        float cx = Mathf.Clamp(x + 0.5f, r, s - r), cy = Mathf.Clamp(y + 0.5f, r, s - r);
                        float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(cx, cy));
                        byte a = (byte)(Mathf.Clamp01(r - d + 0.5f) * 255);
                        px[y * s + x] = new Color32(255, 255, 255, a);
                    }
                tex.SetPixels32(px);
                tex.Apply();
                _round = Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(r, r, r, r));
                return _round;
            }
        }

        /// Aplica el redondeado a una imagen. radio ≈ píxeles de canvas de la esquina.
        public static void Redondear(UnityEngine.UI.Image img, float radio = 12f)
        {
            img.sprite = Round;
            img.type = UnityEngine.UI.Image.Type.Sliced;
            img.pixelsPerUnitMultiplier = 16f / Mathf.Max(2f, radio);
        }
    }
}
