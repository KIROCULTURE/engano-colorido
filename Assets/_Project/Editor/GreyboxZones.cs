using System.Collections.Generic;
using EnganoColorido.Interaction;
using EnganoColorido.Player;
using EnganoColorido.World;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace EnganoColorido.EditorTools
{
    /// Escenas de zona del modo 45 (greybox con atmósfera).
    public static partial class GreyboxBuilder
    {
        static readonly Color FogNiebla = new Color(0.42f, 0.44f, 0.47f);

        static partial void BuildZonesImpl(List<string> built)
        {
            built.Add(BuildCamarin());
            built.Add(BuildPatio());
            built.Add(BuildGaleria());
            built.Add(BuildBiblioteca());
        }

        // ------------------------------------------------------------ comunes
        public static PlayerController AddPlayer(Vector3 pos, float yaw)
        {
            var go = new GameObject("Jugador");
            go.transform.position = pos;
            go.transform.eulerAngles = new Vector3(0, yaw, 0);
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.7f; cc.radius = 0.3f; cc.center = new Vector3(0, 0.85f, 0); cc.stepOffset = 0.35f;
            var cam = AddCamera(go.transform, new Vector3(0, 1.6f, 0), Vector3.zero, RenderSettings.fogColor);
            var lgo = new GameObject("Linterna");
            lgo.transform.SetParent(cam.transform, false);
            lgo.transform.localPosition = new Vector3(0.15f, -0.1f, 0);
            var l = lgo.AddComponent<Light>();
            l.type = LightType.Spot; l.range = 16f; l.spotAngle = 52f; l.innerSpotAngle = 30f;
            l.intensity = 3.2f; l.color = new Color(1f, 0.95f, 0.85f); l.shadows = LightShadows.None;
            var pc = go.AddComponent<PlayerController>();
            pc.cam = cam;
            pc.linterna = l;
            go.AddComponent<PhoneStatic>();
            return pc;
        }

        static readonly Color PanDeOro = new Color(0.85f, 0.64f, 0.24f);

        /// Volúmenes de los dos mundos + WorldManager. Tiñe de oro los renderers (no emisivos) bajo 'roots'.
        public static WorldManager AddWorld(params GameObject[] roots)
        {
            var vn = AddVolume("Volumen_Niebla", ProfNiebla, 1f);
            var vd = AddVolume("Volumen_Dorado", ProfDorado, 0f);
            var wm = new GameObject("Mundo").AddComponent<WorldManager>();
            wm.volNiebla = vn; wm.volDorado = vd;
            foreach (var root in roots)
                foreach (var r in root.GetComponentsInChildren<MeshRenderer>(true))
                {
                    var m = r.sharedMaterial;
                    if (m == null || m.IsKeywordEnabled("_EMISSION") || r.GetComponent<WorldVariant>() != null) continue;
                    var c = m.GetColor("_BaseColor");
                    r.gameObject.AddComponent<WorldVariant>().dorado = Color.Lerp(c, PanDeOro, 0.75f);
                }
            return wm;
        }

        /// Decorado que solo aparece en el Mundo Dorado: inscripción latina, velas y flores marchitas.
        public static GameObject DoradoDecor(Transform parent, Vector3 pos, float rotY, string latin)
        {
            var g = Group("SoloDorado", parent, pos);
            g.transform.localEulerAngles = new Vector3(0, rotY, 0);
            Text3D(latin, g.transform, new Vector3(0, 2.3f, 0), Vector3.zero, 1.8f, new Color(1f, 0.85f, 0.45f), 3f);
            Candle(g.transform, new Vector3(-0.6f, 0, 0.1f), false);
            Candle(g.transform, new Vector3(0.6f, 0, 0.1f), false);
            for (int i = 0; i < 3; i++)
                Prim(PrimitiveType.Sphere, "FlorMarchita", g.transform, new Vector3(-0.2f + i * 0.2f, 0.12f, 0.15f), Vector3.one * 0.12f, Marchita, false);
            g.AddComponent<WorldOnly>().soloDorado = true;
            return g;
        }

        public static ZoneController AddZone(string titulo, string intro, string notif, params string[] zonas)
        {
            var go = new GameObject("Zona");
            var z = go.AddComponent<ZoneController>();
            z.zonas = zonas; z.titulo = titulo; z.intro = intro; z.notif = notif;
            return z;
        }

        public static T With<T>(GameObject go, System.Action<T> init = null) where T : Component
        {
            var c = go.AddComponent<T>();
            init?.Invoke(c);
            return c;
        }

        public static Inspectable Inspect(GameObject go, string prompt, string titulo, string texto, string poema = null, string ficha = null)
            => With<Inspectable>(go, i => { i.prompt = prompt; i.titulo = titulo; i.texto = texto; i.poemaId = poema; i.fichaId = ficha; });

        /// Puerta de madera que tapa un hueco de muro. rotY 0 = muro norte/sur, 90 = muro este/oeste.
        public static Door AddDoor(Transform parent, Vector3 pos, float rotY, string destino, string cerrada, bool siempreCerrada = false)
        {
            var rot = Quaternion.Euler(0, rotY, 0);
            var d = Box("Puerta_" + destino, parent, pos + new Vector3(0, 1.3f, 0), new Vector3(1.8f, 2.6f, 0.12f), siempreCerrada ? Oscuro : Madera, true, new Vector3(0, rotY, 0));
            var fwd = rot * Vector3.forward;
            Text3D(destino, parent, pos + new Vector3(0, 2.85f, 0) - fwd * 0.1f, new Vector3(0, rotY, 0), 2.2f, null, 1.7f);
            Text3D(destino, parent, pos + new Vector3(0, 2.85f, 0) + fwd * 0.1f, new Vector3(0, rotY + 180, 0), 2.2f, null, 1.7f);
            var door = d.AddComponent<Door>();
            door.destino = destino; door.mensajeCerrada = cerrada; door.siempreCerrada = siempreCerrada;
            return door;
        }

        // ------------------------------------------------------------ 0 · Camarín (prólogo)
        static string BuildCamarin()
        {
            NewScene(new Color(0.20f, 0.21f, 0.24f), 0.06f, new Color(0.13f, 0.13f, 0.16f));
            var room = Room("Camarin", Vector3.zero, new Vector3(8, 3.2f, 8), null, null, true, float.NaN, 0f);
            var t = room.transform;
            AddPlayer(new Vector3(0, 0.05f, 0.6f), 0);

            // Espejo con las notas de la profe Inés
            var espejo = Frame("Espejo", t, new Vector3(0, 1.65f, 3.8f), Vector3.zero, 1.4f, 1.9f, Espejo);
            Text3D("EL RETRATO", t, new Vector3(0, 1.85f, 3.75f), Vector3.zero, 1.6f, new Color(0.95f, 0.8f, 0.4f, 0.8f), 1.3f);
            for (int i = 0; i < 2; i++)
                Box("Nota", espejo.transform, new Vector3(-0.85f + i * 1.7f, -0.6f, -0.06f), new Vector3(0.22f, 0.28f, 0.01f), Papel, false, new Vector3(0, 0, i == 0 ? 8 : -6));
            var board = With<FichaBoard>(espejo, b =>
            {
                b.prompt = "Leer las notas de la profe Inés";
                b.fichas = new[] { "hablante", "desengano" };
                b.dialogoDespues = "camarin_listo";
                b.flag = "camarin_notas";
                b.campanaAlTerminar = true;
            });
            espejo.AddComponent<StaticSource>().radio = 4f;
            var task = With<FlagTask>(espejo, f => { f.zona = "camarin"; f.flag = "camarin_notas"; f.objetivo = "Lee las notas de la profe Inés pegadas en el espejo."; });
            _ = board;

            Box("Tocador", t, new Vector3(0, 0.45f, 3.3f), new Vector3(2.4f, 0.9f, 0.7f), Madera);
            Candle(t, new Vector3(-0.9f, 0.9f, 3.3f));
            Candle(t, new Vector3(0.95f, 0.9f, 3.2f));
            for (int i = 0; i < 5; i++) // ampolletas del espejo
                Prim(PrimitiveType.Sphere, "Ampolleta", t, new Vector3(-0.95f + i * 0.475f, 2.75f, 3.75f), Vector3.one * 0.1f, Llama, false);
            PointLight(t, new Vector3(0, 2.5f, 3.2f), new Color(1f, 0.8f, 0.55f), 1.2f, 6f);
            PointLight(t, new Vector3(0, 2.8f, -2f), new Color(0.45f, 0.5f, 0.65f), 0.5f, 8f);

            var silla = Box("Silla", t, new Vector3(0, 0.25f, 2.5f), new Vector3(0.5f, 0.5f, 0.5f), Madera);
            _ = silla;
            var reloj = Group("Reloj", t, new Vector3(-2.6f, 2.3f, 3.82f));
            Prim(PrimitiveType.Cylinder, "Esfera", reloj.transform, Vector3.zero, new Vector3(0.6f, 0.03f, 0.6f), Papel, true, new Vector3(90, 0, 0));
            Text3D("23:59", reloj.transform, new Vector3(0, 0, -0.05f), Vector3.zero, 2.2f, new Color(0.6f, 0.1f, 0.1f), 1f);
            Inspect(reloj, "Mirar el reloj", "El reloj del camarín", "Las manecillas marcan 23:59 y no se mueven. Ni el segundero.");

            // Perchero de vestuario y afiche del ensayo
            var perchero = Group("Perchero", t, new Vector3(3.2f, 0, 1.5f));
            Prim(PrimitiveType.Cylinder, "Barra", perchero.transform, new Vector3(0, 1.8f, 0), new Vector3(0.05f, 1.0f, 0.05f), Metal, true, new Vector3(90, 0, 0));
            for (int i = 0; i < 4; i++)
                Box("Ropa", perchero.transform, new Vector3(0, 1.25f, -0.75f + i * 0.5f), new Vector3(0.12f, 1.0f, 0.42f), Tela);
            Inspect(perchero, "Revisar el vestuario", "Vestuario", "Capa de príncipe, una casaca de rey, un vestido de dama y un traje de bufón. Todo para La vida es sueño.");

            var afiche = Box("Afiche", t, new Vector3(-3.83f, 1.6f, 0.5f), new Vector3(0.02f, 1.1f, 0.8f), Papel);
            Text3D("ENSAYO GENERAL\nLa vida es sueño\nCalderón de la Barca", t, new Vector3(-3.81f, 1.6f, 0.5f), new Vector3(0, -90, 0), 1.2f, new Color(0.2f, 0.1f, 0.05f), 0.75f);
            Inspect(afiche, "Leer el afiche del ensayo", "Afiche del ensayo", "Ensayo general: La vida es sueño (Jornada II). Ale ensaya el monólogo de Segismundo. Abajo, alguien pegó el libreto con cinta:", "calderon_vida");

            AddDoor(t, new Vector3(0, 0, -4f), 0, "Patio", "Antes de salir… las notas del espejo.");

            DoradoDecor(t, new Vector3(2.0f, 0, 3.4f), 0, "VANITAS VANITATUM");
            var zone = AddZone("Prólogo · Camarín", "camarin_despertar", null, "camarin");
            zone.tasks = new ZoneTask[] { task };
            zone.introEnDorado = true;
            AddWorld(room);
            return SaveScene("Camarin");
        }
    }
}
