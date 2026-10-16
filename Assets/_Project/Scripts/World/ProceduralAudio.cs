using UnityEngine;

namespace EnganoColorido.World
{
    /// Sonidos generados por código (sin assets): estática, campana y un tono grave para revelaciones.
    public static class ProceduralAudio
    {
        const int Rate = 22050;
        static AudioClip _static, _bell, _tone;

        public static AudioClip Static
        {
            get
            {
                if (_static != null) return _static;
                var d = new float[Rate * 2];
                var rng = new System.Random(7);
                float lp = 0;
                for (int i = 0; i < d.Length; i++)
                {
                    float n = (float)(rng.NextDouble() * 2 - 1);
                    lp += (n - lp) * 0.35f; // ruido algo filtrado, menos chillón
                    float crackle = rng.NextDouble() < 0.002 ? (float)(rng.NextDouble() - 0.5) * 1.5f : 0;
                    d[i] = (lp * 0.5f + crackle) * 0.6f;
                }
                _static = AudioClip.Create("Estatica", d.Length, 1, Rate, false);
                _static.SetData(d, 0);
                return _static;
            }
        }

        public static AudioClip Bell
        {
            get
            {
                if (_bell != null) return _bell;
                int n = Rate * 4;
                var d = new float[n];
                float[] f = { 220f, 440f, 554f, 659f, 880f, 1163f };
                float[] a = { 0.5f, 0.35f, 0.25f, 0.2f, 0.12f, 0.08f };
                for (int i = 0; i < n; i++)
                {
                    float t = (float)i / Rate, s = 0;
                    for (int k = 0; k < f.Length; k++) s += a[k] * Mathf.Sin(2 * Mathf.PI * f[k] * t) * Mathf.Exp(-t * (0.8f + k * 0.5f));
                    d[i] = s * Mathf.Min(1, t * 200) * 0.5f;
                }
                _bell = AudioClip.Create("Campana", n, 1, Rate, false);
                _bell.SetData(d, 0);
                return _bell;
            }
        }

        public static AudioClip Tone
        {
            get
            {
                if (_tone != null) return _tone;
                int n = Rate * 3;
                var d = new float[n];
                for (int i = 0; i < n; i++)
                {
                    float t = (float)i / Rate;
                    float env = Mathf.Min(1, t * 2) * Mathf.Exp(-t * 0.9f);
                    d[i] = (Mathf.Sin(2 * Mathf.PI * 65f * t) * 0.6f + Mathf.Sin(2 * Mathf.PI * 98f * t) * 0.3f) * env * 0.5f;
                }
                _tone = AudioClip.Create("Tono", n, 1, Rate, false);
                _tone.SetData(d, 0);
                return _tone;
            }
        }

        public static void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            var go = new GameObject("Sonido");
            var src = go.AddComponent<AudioSource>();
            src.spatialBlend = 0;
            src.PlayOneShot(clip, volume);
            Object.Destroy(go, clip.length + 0.1f);
        }
    }
}
