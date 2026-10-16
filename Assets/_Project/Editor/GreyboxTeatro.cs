using EnganoColorido.Interaction;
using EnganoColorido.Puzzles;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.EditorTools
{
    /// Teatro: P7 (libreto y maniquíes), Juicio del Espejo, Tu verso y los dos finales.
    public static partial class GreyboxBuilder
    {
        static string BuildTeatro()
        {
            NewScene(new Color(0.22f, 0.20f, 0.22f), 0.035f, new Color(0.22f, 0.20f, 0.22f));
            var sala = RoomX("Teatro", Vector3.zero, new Vector3(20, 7, 22), new float[0], new[] { 0f }, new float[0], new float[0]);
            var t = sala.transform;
            AddPlayer(new Vector3(0, 0.05f, -9f), 0);
            AddDoor(t, new Vector3(0, 0, -11f), 0, "Biblioteca", "Esta noche termina aquí.", true);

            // Butacas y escenario
            for (int r = 0; r < 5; r++)
                for (int side = -1; side <= 1; side += 2)
                    Box("Butacas", t, new Vector3(side * 4.5f, 0.45f, -6.5f + r * 1.6f), new Vector3(6f, 0.9f, 0.7f), Mat("Butaca", new Color(0.35f, 0.08f, 0.1f)));
            var escenario = Box("Escenario", t, new Vector3(0, 0.5f, 7.5f), new Vector3(20f, 1f, 7f), Madera);
            for (int s = 0; s < 3; s++)
            {
                float h = (s + 1) * 0.33f;
                Box("Peldaño", t, new Vector3(0, h / 2, 2.95f + s * 0.4f), new Vector3(3f, h, 0.4f), Madera);
            }
            Box("TelonI", t, new Vector3(-8.6f, 3.8f, 4.3f), new Vector3(2.8f, 5.6f, 0.3f), Mat("Telon", new Color(0.45f, 0.06f, 0.08f)));
            Box("TelonD", t, new Vector3(8.6f, 3.8f, 4.3f), new Vector3(2.8f, 5.6f, 0.3f), Mat("Telon", new Color(0.45f, 0.06f, 0.08f)));
            PointLight(t, new Vector3(0, 6f, 5f), new Color(1f, 0.85f, 0.6f), 9f, 14f);
            PointLight(t, new Vector3(-5f, 5f, 7f), new Color(1f, 0.8f, 0.55f), 5f, 9f);
            PointLight(t, new Vector3(5f, 5f, 7f), new Color(1f, 0.8f, 0.55f), 5f, 9f);
            PointLight(t, new Vector3(0, 5f, -5f), new Color(0.6f, 0.65f, 0.8f), 3f, 12f);

            // P7 · El libreto de Segismundo
            var mesa = Box("MesaLibreto", t, new Vector3(-4.5f, 1.45f, 6f), new Vector3(2f, 0.9f, 1f), Madera);
            for (int i = 0; i < 4; i++) Box("Hoja", t, new Vector3(-5.2f + i * 0.45f, 1.91f, 6f), new Vector3(0.35f, 0.01f, 0.48f), Papel, false, new Vector3(0, i * 9 - 12, 0));
            var p7 = MakePuzzle(mesa, "P7", "teatro", "Arma el libreto de Segismundo y ponle su máscara a cada maniquí.");
            p7.placa = "El libreto de La vida es sueño está desarmado: cuatro hojas sin rótulo. Al lado, cuatro maniquíes sin cara, cada uno con la ropa de un personaje.";
            p7.poemasPlaca = new[] { "calderon_vida" };
            Trigger(mesa, p7, "Revisar el libreto");
            string[] nombres = { "SEGISMUNDO", "ROSAURA", "CLARÍN", "BASILIO" };
            Color[] ropa = { new Color(0.2f, 0.25f, 0.5f), new Color(0.5f, 0.2f, 0.35f), new Color(0.6f, 0.5f, 0.15f), new Color(0.45f, 0.1f, 0.1f) };
            var mascaras = Group("Mascaras", t);
            var maniquies = Group("Maniquies", t, new Vector3(4.5f, 1f, 7f));
            for (int i = 0; i < 4; i++)
            {
                float x = -2.25f + i * 1.5f;
                Prim(PrimitiveType.Cylinder, "Cuerpo", maniquies.transform, new Vector3(x, 0.8f, 0), new Vector3(0.45f, 0.8f, 0.35f), Mat("Ropa" + i, ropa[i]), false);
                Prim(PrimitiveType.Sphere, "Cabeza", maniquies.transform, new Vector3(x, 1.85f, 0), Vector3.one * 0.32f, Hueso, false);
                Text3D(nombres[i], t, new Vector3(4.5f + x, 1.25f, 6.6f), Vector3.zero, 1.6f, new Color(0.95f, 0.85f, 0.6f), 1.4f);
                Box("Mascara", mascaras.transform, new Vector3(4.5f + x, 2.88f, 6.82f), new Vector3(0.26f, 0.2f, 0.04f), i == 2 ? Mat("MascaraGracioso", new Color(0.9f, 0.7f, 0.2f)) : Oro, false);
            }
            var mcol = maniquies.AddComponent<BoxCollider>(); mcol.size = new Vector3(6f, 2.3f, 0.8f); mcol.center = new Vector3(0, 1.15f, 0);
            Trigger(maniquies, p7, "Mirar los maniquíes");
            mascaras.SetActive(false);
            p7.mostrarAlResolver = new[] { mascaras };

            // Juicio del Espejo
            var espejo = Frame("GranEspejo", t, new Vector3(0, 3.6f, 10.6f), Vector3.zero, 3f, 4f, Espejo);
            Text3D("EL RETRATO", t, new Vector3(0, 3.8f, 10.5f), Vector3.zero, 3f, new Color(0.95f, 0.8f, 0.4f, 0.85f), 3f);
            espejo.AddComponent<StaticSource>().radio = 6f;
            var juicio = MakePuzzle(espejo, "JUICIO", "teatro", "Enfrenta a El Retrato en el gran espejo del escenario.");
            juicio.requierePuzzle = p7;
            juicio.mensajeBloqueado = "El reflejo espera. Primero arma el libreto de Segismundo.";
            juicio.dialogoAlAbrir = "juicio_intro";
            juicio.mundoDoradoAlAbrir = true;
            juicio.dialogoResuelto = "juicio_fin";
            Trigger(espejo, juicio, "Enfrentar a El Retrato");

            // Tu verso
            var escritorio = Box("Escritorio", t, new Vector3(-1.6f, 1.4f, 5.0f), new Vector3(1.4f, 0.8f, 0.8f), Madera);
            Box("Cuaderno", t, new Vector3(-1.6f, 1.81f, 5.0f), new Vector3(0.5f, 0.02f, 0.35f), Papel, false);
            Candle(t, new Vector3(-1.1f, 1.8f, 5.1f));
            var verso = MakePuzzle(escritorio, "TUVERSO", "teatro", "Escribe tu verso en el escritorio del escenario.");
            verso.tuVerso = true;
            verso.requierePuzzle = juicio;
            verso.mensajeBloqueado = "Primero, el Juicio del Espejo.";
            Trigger(escritorio, verso, "Escribir tu verso");

            // Elección del final
            var pie = Box("PieDelEspejo", t, new Vector3(0, 1.5f, 9.9f), new Vector3(3.4f, 1f, 0.8f), Madera);
            var final = pie.AddComponent<FinalTask>();
            final.zona = "teatro"; final.requiere = verso;
            var fi = pie.AddComponent<FinalInteract>(); fi.final = final; fi.prompt = "Responder a El Retrato";
            var dir = sala.AddComponent<TeatroDirector>();
            dir.juicio = juicio; dir.tuVerso = verso; dir.final = final;

            DoradoDecor(t, new Vector3(-5f, 1f, 9.5f), 0, "VITA SOMNIUM");
            var zone = AddZone("Teatro", "teatro_intro", "notif_teatro", "teatro");
            zone.tasks = new ZoneTask[] { p7, juicio, verso, final };
            AddWorld(sala);
            return SaveScene("Teatro");
        }
    }
}
