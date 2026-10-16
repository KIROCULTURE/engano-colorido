using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace EnganoColorido.EditorTools
{
    /// Tools/Engaño Colorido/Build Greybox Scenes: genera todas las escenas con primitivas.
    /// Las escenas se regeneran completas: no editar a mano, editar este generador.
    public static partial class GreyboxBuilder
    {
        public const string ScenesDir = "Assets/_Project/Scenes";
        public const string MatDir = "Assets/_Project/Materials";

        [MenuItem("Tools/Engaño Colorido/Build Greybox Scenes", priority = 20)]
        public static void BuildAll()
        {
            Directory.CreateDirectory(ScenesDir);
            Directory.CreateDirectory(MatDir);
            CreateProfiles();
            var built = new List<string>();
            built.Add(BuildArranque());
            BuildZones(built);
            EditorBuildSettings.scenes = built.ConvertAll(p => new EditorBuildSettingsScene(p, true)).ToArray();
            AssetDatabase.SaveAssets();
            Debug.Log("[Greybox] Escenas generadas: " + string.Join(", ", built));
        }

        static partial void BuildZonesImpl(List<string> built);
        static void BuildZones(List<string> built) => BuildZonesImpl(built);

        // ---------------------------------------------------------------- materiales
        static readonly Dictionary<string, Material> mats = new();

        public static Material Mat(string name, Color color, float metallic = 0f, float smooth = 0.25f, Color? emission = null)
        {
            if (mats.TryGetValue(name, out var cached) && cached != null) return cached;
            string path = $"{MatDir}/M_{name}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (m == null)
            {
                m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(m, path);
            }
            m.SetColor("_BaseColor", color);
            m.SetFloat("_Metallic", metallic);
            m.SetFloat("_Smoothness", smooth);
            if (emission.HasValue)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", emission.Value);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            }
            else m.DisableKeyword("_EMISSION");
            EditorUtility.SetDirty(m);
            mats[name] = m;
            return m;
        }

        // Paleta greybox
        public static Material Piso => Mat("Piso", new Color(0.30f, 0.29f, 0.28f));
        public static Material Muro => Mat("Muro", new Color(0.55f, 0.53f, 0.50f));
        public static Material Madera => Mat("Madera", new Color(0.36f, 0.24f, 0.15f));
        public static Material Oscuro => Mat("Oscuro", new Color(0.08f, 0.08f, 0.09f));
        public static Material Oro => Mat("Oro", new Color(0.80f, 0.62f, 0.25f), 0.85f, 0.55f);
        public static Material Papel => Mat("Papel", new Color(0.88f, 0.84f, 0.74f));
        public static Material Espejo => Mat("Espejo", new Color(0.55f, 0.62f, 0.66f), 0.9f, 0.95f);
        public static Material Vela => Mat("Vela", new Color(0.75f, 0.10f, 0.08f), 0, 0.3f, new Color(0.6f, 0.08f, 0.04f));
        public static Material Llama => Mat("Llama", new Color(1f, 0.75f, 0.3f), 0, 0, new Color(3f, 1.8f, 0.5f));
        public static Material Hueso => Mat("Hueso", new Color(0.85f, 0.82f, 0.72f));
        public static Material Tela => Mat("Tela", new Color(0.25f, 0.22f, 0.30f));
        public static Material Vidrio => Mat("Vidrio", new Color(0.55f, 0.70f, 0.65f), 0.1f, 0.9f);
        public static Material Verde => Mat("Verde", new Color(0.18f, 0.32f, 0.16f));
        public static Material Rosa => Mat("Rosa", new Color(0.80f, 0.08f, 0.18f), 0, 0.4f, new Color(0.35f, 0.02f, 0.06f));
        public static Material Marchita => Mat("Marchita", new Color(0.35f, 0.22f, 0.14f));
        public static Material Boton => Mat("BotonRosa", new Color(0.30f, 0.45f, 0.20f));
        public static Material Metal => Mat("Metal", new Color(0.45f, 0.46f, 0.48f), 0.8f, 0.5f);

        // ---------------------------------------------------------------- postproceso
        public const string ProfNiebla = MatDir + "/PP_Niebla.asset";
        public const string ProfDorado = MatDir + "/PP_Dorado.asset";

        static void CreateProfiles()
        {
            Profile(ProfNiebla, p =>
            {
                var c = p.Add<ColorAdjustments>(true);
                c.postExposure.Override(-0.1f); c.saturation.Override(-45f); c.contrast.Override(12f);
                p.Add<Vignette>(true).intensity.Override(0.38f);
                var g = p.Add<FilmGrain>(true); g.intensity.Override(0.35f); g.type.Override(FilmGrainLookup.Medium1);
                p.Add<Tonemapping>(true).mode.Override(TonemappingMode.ACES);
            });
            Profile(ProfDorado, p =>
            {
                var b = p.Add<Bloom>(true); b.threshold.Override(1.1f); b.intensity.Override(0.6f); b.tint.Override(new Color(1f, 0.85f, 0.6f));
                var c = p.Add<ColorAdjustments>(true);
                c.colorFilter.Override(new Color(1f, 0.9f, 0.72f)); c.saturation.Override(0f); c.postExposure.Override(-0.6f);
                var v = p.Add<Vignette>(true); v.intensity.Override(0.3f); v.color.Override(new Color(0.25f, 0.15f, 0.02f));
                p.Add<Tonemapping>(true).mode.Override(TonemappingMode.ACES);
            });
        }

        static void Profile(string path, System.Action<VolumeProfile> fill)
        {
            var old = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (old != null) AssetDatabase.DeleteAsset(path);
            var p = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(p, path);
            fill(p);
            foreach (var comp in p.components)
            {
                comp.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(comp, p);
            }
            EditorUtility.SetDirty(p);
            AssetDatabase.SaveAssets();
        }

        public static Volume AddVolume(string name, string profilePath, float weight)
        {
            var go = new GameObject(name);
            var v = go.AddComponent<Volume>();
            v.isGlobal = true;
            v.weight = weight;
            v.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
            return v;
        }

        // ---------------------------------------------------------------- escena
        public static Scene NewScene(Color fog, float density, Color ambient)
        {
            var s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fog;
            RenderSettings.fogDensity = density;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = ambient;
            RenderSettings.skybox = null;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            Lightmapping.lightingSettings = LightingSettingsAsset();
            return s;
        }

        static LightingSettings _ls;
        static LightingSettings LightingSettingsAsset()
        {
            const string path = MatDir + "/Iluminacion.lighting";
            _ls = AssetDatabase.LoadAssetAtPath<LightingSettings>(path);
            if (_ls == null)
            {
                _ls = new LightingSettings { bakedGI = false, realtimeGI = false, autoGenerate = false };
                AssetDatabase.CreateAsset(_ls, path);
            }
            return _ls;
        }

        public static string SaveScene(string name)
        {
            string path = $"{ScenesDir}/{name}.unity";
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path);
            return path;
        }

        public static Camera AddCamera(Transform parent, Vector3 pos, Vector3 euler, Color bg)
        {
            var go = new GameObject("Camara");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bg;
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 120f;
            cam.fieldOfView = 70f;
            go.AddComponent<AudioListener>();
            var data = cam.GetUniversalAdditionalCameraData();
            data.renderPostProcessing = true;
            data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            return cam;
        }

        // ---------------------------------------------------------------- primitivas
        public static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool collider = true, Vector3? euler = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            if (euler.HasValue) go.transform.localEulerAngles = euler.Value;
            var r = go.GetComponent<MeshRenderer>();
            r.sharedMaterial = mat;
            r.shadowCastingMode = ShadowCastingMode.Off;
            if (!collider) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }

        public static GameObject Box(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat, bool collider = true, Vector3? euler = null)
            => Prim(PrimitiveType.Cube, name, parent, pos, scale, mat, collider, euler);

        public static GameObject Group(string name, Transform parent = null, Vector3? pos = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.localPosition = pos ?? Vector3.zero;
            return go;
        }

        /// Sala rectangular: piso, techo y cuatro muros con huecos de puerta (posiciones a lo largo de cada muro).
        public static GameObject Room(string name, Vector3 center, Vector3 size, Material wall = null, Material floor = null, bool ceiling = true, float doorNorthX = float.NaN, float doorSouthX = float.NaN, float doorEastZ = float.NaN, float doorWestZ = float.NaN)
            => RoomX(name, center, size, D(doorNorthX), D(doorSouthX), D(doorEastZ), D(doorWestZ), wall, floor, ceiling);

        static float[] D(float v) => float.IsNaN(v) ? new float[0] : new[] { v };

        public static GameObject RoomX(string name, Vector3 center, Vector3 size, float[] north, float[] south, float[] east, float[] west,
            Material wall = null, Material floor = null, bool ceiling = true, string skip = "")
        {
            wall ??= Muro; floor ??= Piso;
            var g = Group(name, null, center);
            var t = g.transform;
            float w = size.x, h = size.y, d = size.z;
            Box("Piso", t, new Vector3(0, -0.05f, 0), new Vector3(w, 0.1f, d), floor);
            if (ceiling) Box("Techo", t, new Vector3(0, h, 0), new Vector3(w, 0.1f, d), Oscuro);
            if (!skip.Contains("N")) Wall(t, "MuroN", new Vector3(0, h / 2, d / 2), w, h, true, north, wall);
            if (!skip.Contains("S")) Wall(t, "MuroS", new Vector3(0, h / 2, -d / 2), w, h, true, south, wall);
            if (!skip.Contains("E")) Wall(t, "MuroE", new Vector3(w / 2, h / 2, 0), d, h, false, east, wall);
            if (!skip.Contains("O")) Wall(t, "MuroO", new Vector3(-w / 2, h / 2, 0), d, h, false, west, wall);
            return g;
        }

        const float DoorW = 1.8f, DoorH = 2.6f;

        /// Muro con huecos de puerta. pos.y = mitad de la altura. doors = posiciones a lo largo del muro.
        public static void Wall(Transform t, string name, Vector3 pos, float len, float h, bool alongX, float[] doors, Material mat, float th = 0.3f)
        {
            Vector3 Sz(float l, float hh) => alongX ? new Vector3(l, hh, th) : new Vector3(th, hh, l);
            Vector3 Off(float a, float y) => alongX ? new Vector3(pos.x + a, y, pos.z) : new Vector3(pos.x, y, pos.z + a);
            var sorted = new List<float>(doors ?? new float[0]);
            sorted.Sort();
            float from = -len / 2;
            int k = 0;
            foreach (var at in sorted)
            {
                float segEnd = at - DoorW / 2;
                if (segEnd - from > 0.01f) Box($"{name}_{k++}", t, Off((from + segEnd) / 2, pos.y), Sz(segEnd - from, h), mat);
                if (h > DoorH) Box($"{name}_dintel{k++}", t, Off(at, DoorH + (h - DoorH) / 2), Sz(DoorW, h - DoorH), mat);
                from = at + DoorW / 2;
            }
            if (len / 2 - from > 0.01f) Box($"{name}_{k}", t, Off((from + len / 2) / 2, pos.y), Sz(len / 2 - from, h), mat);
        }

        public static TextMeshPro Text3D(string text, Transform parent, Vector3 pos, Vector3 euler, float size = 2f, Color? color = null, float width = 4f)
        {
            var go = new GameObject("Texto");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localEulerAngles = euler;
            var t = go.AddComponent<TextMeshPro>();
            t.text = text;
            t.fontSize = size;
            t.color = color ?? new Color(0.92f, 0.88f, 0.8f);
            t.alignment = TextAlignmentOptions.Center;
            t.textWrappingMode = TextWrappingModes.Normal;
            t.rectTransform.sizeDelta = new Vector2(width, 2f);
            return t;
        }

        public static Light PointLight(Transform parent, Vector3 pos, Color color, float intensity = 2f, float range = 8f)
        {
            var go = new GameObject("Luz");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            var l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.None;
            return l;
        }

        public static Light Sun(Color color, float intensity, Vector3 euler)
        {
            var go = new GameObject("LuzAmbiente");
            go.transform.eulerAngles = euler;
            var l = go.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = color;
            l.intensity = intensity;
            l.shadows = LightShadows.None;
            return l;
        }

        /// Vela roja (punto de guardado visual): cilindro + llama + luz.
        public static GameObject Candle(Transform parent, Vector3 pos, bool light = true)
        {
            var g = Group("VelaRoja", parent, pos);
            Prim(PrimitiveType.Cylinder, "Cera", g.transform, new Vector3(0, 0.15f, 0), new Vector3(0.08f, 0.15f, 0.08f), Vela, false);
            Prim(PrimitiveType.Sphere, "Llama", g.transform, new Vector3(0, 0.34f, 0), new Vector3(0.04f, 0.07f, 0.04f), Llama, false);
            if (light) PointLight(g.transform, new Vector3(0, 0.45f, 0), new Color(1f, 0.6f, 0.3f), 0.8f, 3.5f);
            return g;
        }

        /// Marco dorado vertical con superficie interior (espejo, retrato, cuadro).
        public static GameObject Frame(string name, Transform parent, Vector3 pos, Vector3 euler, float w, float h, Material inner)
        {
            var g = Group(name, parent, pos);
            g.transform.localEulerAngles = euler;
            var t = g.transform;
            float b = 0.12f;
            Box("Interior", t, Vector3.zero, new Vector3(w, h, 0.04f), inner, false);
            Box("MarcoArriba", t, new Vector3(0, h / 2 + b / 2, 0), new Vector3(w + 2 * b, b, 0.1f), Oro, false);
            Box("MarcoAbajo", t, new Vector3(0, -h / 2 - b / 2, 0), new Vector3(w + 2 * b, b, 0.1f), Oro, false);
            Box("MarcoIzq", t, new Vector3(-w / 2 - b / 2, 0, 0), new Vector3(b, h, 0.1f), Oro, false);
            Box("MarcoDer", t, new Vector3(w / 2 + b / 2, 0, 0), new Vector3(b, h, 0.1f), Oro, false);
            var col = g.AddComponent<BoxCollider>();
            col.size = new Vector3(w + 2 * b, h + 2 * b, 0.2f);
            return g;
        }

        // ---------------------------------------------------------------- Arranque
        static string BuildArranque()
        {
            NewScene(new Color(0.07f, 0.07f, 0.09f), 0.09f, new Color(0.10f, 0.10f, 0.12f));
            var room = Room("Camarin", new Vector3(0, 0, 0), new Vector3(8, 3.2f, 8));
            var cam = AddCamera(null, new Vector3(1.6f, 1.55f, -2.4f), new Vector3(4, -18, 0), RenderSettings.fogColor);
            Frame("Espejo", room.transform, new Vector3(0, 1.6f, 3.8f), Vector3.zero, 1.4f, 1.9f, Espejo);
            Box("Tocador", room.transform, new Vector3(0, 0.45f, 3.3f), new Vector3(2.4f, 0.9f, 0.7f), Madera);
            Candle(room.transform, new Vector3(-0.8f, 0.9f, 3.3f));
            Candle(room.transform, new Vector3(0.9f, 0.9f, 3.2f));
            Text3D("23:59", room.transform, new Vector3(-2.6f, 2.4f, 3.84f), Vector3.zero, 3f, new Color(0.8f, 0.2f, 0.2f));
            PointLight(room.transform, new Vector3(0, 2.6f, 1.5f), new Color(0.5f, 0.55f, 0.7f), 0.6f, 9f);
            AddVolume("Volumen_Niebla", ProfNiebla, 1f);
            _ = cam;
            return SaveScene("Arranque");
        }
    }
}
