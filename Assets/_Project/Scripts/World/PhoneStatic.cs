using UnityEngine;

namespace EnganoColorido.World
{
    /// Estática procedural del celular: sube cuando hay una StaticSource cerca.
    public class PhoneStatic : MonoBehaviour
    {
        AudioSource src;
        public float Level { get; private set; }
        public float extra; // estática forzada (p. ej. sintonizando una radio)

        void Start()
        {
            src = gameObject.AddComponent<AudioSource>();
            src.clip = ProceduralAudio.Static;
            src.loop = true;
            src.spatialBlend = 0;
            src.volume = 0;
            src.Play();
        }

        void Update()
        {
            float target = extra;
            foreach (var s in StaticSource.All)
            {
                if (s == null || !s.isActiveAndEnabled) continue;
                float d = Vector3.Distance(transform.position, s.transform.position);
                target = Mathf.Max(target, Mathf.Clamp01(1 - d / s.radio) * s.fuerza);
            }
            Level = Mathf.MoveTowards(Level, target, Time.unscaledDeltaTime * 2f);
            if (src) src.volume = Level * 0.5f;
        }
    }
}
