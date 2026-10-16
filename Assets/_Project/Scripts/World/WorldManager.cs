using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace EnganoColorido.World
{
    /// Mundo de Niebla (lo humano) ↔ Mundo Dorado (la apariencia). Cambia materiales, luces, niebla y
    /// postproceso con una transición suave (sin destellos). Nada más.
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager I { get; private set; }
        public Volume volNiebla, volDorado;
        public Color fogDorado = new Color(0.36f, 0.27f, 0.12f);
        public float densidadDorado = 0.025f;
        public Color ambienteDorado = new Color(0.20f, 0.15f, 0.07f);
        public Color luzDorado = new Color(0.9f, 0.65f, 0.3f);

        public bool Dorado { get; private set; }
        public bool Transitioning { get; private set; }
        public event Action<bool> TransitionStarted; // arg: hacia el Dorado
        public event Action<bool> TransitionEnded;

        Color fogNiebla, ambNiebla;
        float densNiebla;
        Light[] lights;
        Color[] lightColors;
        WorldVariant[] variants;
        WorldOnly[] onlies;
        AudioSource bellSource;

        void Awake()
        {
            I = this;
            fogNiebla = RenderSettings.fogColor;
            densNiebla = RenderSettings.fogDensity;
            ambNiebla = RenderSettings.ambientLight;
            lights = Array.FindAll(FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None), l => l.type != LightType.Spot); // la linterna no cambia
            lightColors = Array.ConvertAll(lights, l => l.color);
            variants = FindObjectsByType<WorldVariant>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            onlies = FindObjectsByType<WorldOnly>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bellSource = gameObject.AddComponent<AudioSource>();
            bellSource.spatialBlend = 0;
            foreach (var o in onlies) o.Apply(false);
        }

        public void RingBell(bool toDorado, float seconds = 2.5f)
        {
            bellSource.PlayOneShot(ProceduralAudio.Bell, 0.8f);
            SetWorld(toDorado, seconds);
        }

        public void SetWorld(bool dorado, float seconds = 2.5f)
        {
            if (dorado == Dorado && !Transitioning) return;
            StopAllCoroutines();
            StartCoroutine(Transition(dorado, seconds));
        }

        IEnumerator Transition(bool toDorado, float seconds)
        {
            Transitioning = true;
            TransitionStarted?.Invoke(toDorado);
            bool swapped = false;
            for (float e = 0; e < seconds; e += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0, 1, e / seconds);
                Apply(toDorado ? k : 1 - k);
                if (!swapped && e > seconds / 2) { swapped = true; foreach (var o in onlies) o.Apply(toDorado); }
                yield return null;
            }
            Apply(toDorado ? 1 : 0);
            foreach (var o in onlies) o.Apply(toDorado);
            Dorado = toDorado;
            Transitioning = false;
            TransitionEnded?.Invoke(toDorado);
        }

        void Apply(float t)
        {
            RenderSettings.fogColor = Color.Lerp(fogNiebla, fogDorado, t);
            RenderSettings.fogDensity = Mathf.Lerp(densNiebla, densidadDorado, t);
            RenderSettings.ambientLight = Color.Lerp(ambNiebla, ambienteDorado, t);
            if (volNiebla) volNiebla.weight = 1 - t;
            if (volDorado) volDorado.weight = t;
            for (int i = 0; i < lights.Length; i++) if (lights[i]) lights[i].color = Color.Lerp(lightColors[i], luzDorado, t);
            foreach (var v in variants) if (v) v.Apply(t);
            var cam = Camera.main;
            if (cam) cam.backgroundColor = RenderSettings.fogColor;
        }

        /// Amanecer del final Despertar: la niebla se retira.
        public void Dawn(float seconds = 4f) => StartCoroutine(DawnCo(seconds));
        IEnumerator DawnCo(float seconds)
        {
            Color f0 = RenderSettings.fogColor, a0 = RenderSettings.ambientLight;
            float d0 = RenderSettings.fogDensity;
            for (float e = 0; e < seconds; e += Time.deltaTime)
            {
                float k = Mathf.SmoothStep(0, 1, e / seconds);
                RenderSettings.fogColor = Color.Lerp(f0, new Color(0.75f, 0.72f, 0.65f), k);
                RenderSettings.fogDensity = Mathf.Lerp(d0, 0.01f, k);
                RenderSettings.ambientLight = Color.Lerp(a0, new Color(0.55f, 0.52f, 0.48f), k);
                if (volNiebla) volNiebla.weight = 1 - k * 0.7f;
                if (volDorado) volDorado.weight = 0;
                var cam = Camera.main;
                if (cam) cam.backgroundColor = RenderSettings.fogColor;
                yield return null;
            }
        }
    }
}
