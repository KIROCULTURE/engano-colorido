using EnganoColorido.Interaction;
using EnganoColorido.Puzzles;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.EditorTools
{
    /// Patio (P1 + H), Galería (P3, P4 y el velo) y Biblioteca (P5).
    public static partial class GreyboxBuilder
    {
        public static PuzzleController MakePuzzle(GameObject host, string id, string zona, string objetivo)
        {
            var p = host.AddComponent<PuzzleController>();
            p.puzzleId = id; p.zona = zona; p.objetivo = objetivo;
            return p;
        }

        public static PuzzleTrigger Trigger(GameObject go, PuzzleController p, string prompt)
        {
            var t = go.AddComponent<PuzzleTrigger>();
            t.puzzle = p; t.prompt = prompt;
            return t;
        }

        static Pickup Item(string id, Transform parent, Vector3 pos, System.Action<Transform> shape)
        {
            var g = Group("Objeto_" + id, parent, pos);
            shape(g.transform);
            var col = g.AddComponent<BoxCollider>();
            col.size = new Vector3(0.5f, 0.5f, 0.5f); col.center = new Vector3(0, 0.2f, 0);
            var pk = g.AddComponent<Pickup>();
            pk.itemId = id;
            return pk;
        }

        static GameObject Lamp(Transform parent, Vector3 pos, float intensity = 4f, float range = 12f)
        {
            var g = Group("Farol", parent, pos);
            Prim(PrimitiveType.Cylinder, "Poste", g.transform, new Vector3(0, 1.5f, 0), new Vector3(0.1f, 1.5f, 0.1f), Metal);
            Prim(PrimitiveType.Sphere, "Ampolleta", g.transform, new Vector3(0, 3.1f, 0), Vector3.one * 0.3f, Llama, false);
            PointLight(g.transform, new Vector3(0, 2.9f, 0), new Color(1f, 0.75f, 0.45f), intensity, range);
            return g;
        }

        static GameObject Silhouette(string name, Transform parent, Vector3 pos, Material mat, float yaw = 0)
        {
            var g = Group(name, parent, pos);
            g.transform.localEulerAngles = new Vector3(0, yaw, 0);
            Prim(PrimitiveType.Capsule, "Cuerpo", g.transform, new Vector3(0, 0.85f, 0), new Vector3(0.6f, 0.85f, 0.45f), mat, false);
            Prim(PrimitiveType.Sphere, "Cabeza", g.transform, new Vector3(0, 1.85f, 0), Vector3.one * 0.32f, mat, false);
            var col = g.AddComponent<BoxCollider>();
            col.size = new Vector3(0.7f, 2f, 0.6f); col.center = new Vector3(0, 1f, 0);
            return g;
        }

        // ------------------------------------------------------------ 1 · Patio de las rosas + H · Radios
        static string BuildPatio()
        {
            NewScene(FogNiebla, 0.065f, new Color(0.16f, 0.17f, 0.20f));
            Sun(new Color(0.5f, 0.55f, 0.7f), 0.25f, new Vector3(50, 30, 0));
            var patio = Group("Patio");
            var t = patio.transform;
            Box("Suelo", t, new Vector3(0, -0.05f, 0), new Vector3(40, 0.1f, 40), Piso);
            Wall(t, "MuroN", new Vector3(0, 2.5f, 20), 40, 5, true, new[] { -2f, 12f }, Muro);
            Wall(t, "MuroS", new Vector3(0, 2.5f, -20), 40, 5, true, new[] { 0f }, Muro);
            Wall(t, "MuroE", new Vector3(20, 2.5f, 0), 40, 5, false, new float[0], Muro);
            Wall(t, "MuroO", new Vector3(-20, 2.5f, 0), 40, 5, false, new[] { 6f }, Muro);
            for (int i = -3; i <= 3; i++)
            {
                Prim(PrimitiveType.Cylinder, "Pilar", t, new Vector3(i * 5.5f, 2f, 19.4f), new Vector3(0.5f, 2f, 0.5f), Muro);
                Prim(PrimitiveType.Cylinder, "Pilar", t, new Vector3(19.4f, 2f, i * 5.5f), new Vector3(0.5f, 2f, 0.5f), Muro);
            }
            AddPlayer(new Vector3(0, 0.05f, -16f), 0);

            AddDoor(t, new Vector3(0, 0, -20f), 0, "Camarín", "Ya no queda nada en el camarín.", true);
            AddDoor(t, new Vector3(-20f, 0, 6f), 90, "Capilla", "Capilla: solo en el modo 90 (Entrega 2). En el modo 45 no se visita.", true);
            var salida = AddDoor(t, new Vector3(12f, 0, 20f), 0, "Galería", "La puerta de la galería no abre.");

            // Don Ruperto
            var ruperto = Silhouette("DonRuperto", t, new Vector3(2.5f, 0, -12f), Tela, 200);
            PointLight(ruperto.transform, new Vector3(0.4f, 1.2f, 0.4f), new Color(1f, 0.7f, 0.4f), 0.8f, 3f);
            var talk = ruperto.AddComponent<Talk>(); talk.prompt = "Hablar con Don Ruperto"; talk.dialogo = "patio_intro";

            // Torre con la campana
            var torre = Box("Torre", t, new Vector3(-2f, 4.5f, 18f), new Vector3(3.4f, 9f, 3.4f), Muro);
            Prim(PrimitiveType.Cylinder, "Campana", t, new Vector3(-2f, 9.6f, 16.2f), new Vector3(1f, 0.6f, 1f), Oro, false);
            var bell = torre.AddComponent<BellCycle>();
            AddDoor(t, new Vector3(-2f, 0, 16.25f), 0, "Torre del reloj", "Torre del reloj: solo en el modo 90 (Entrega 2). En el modo 45 no se visita.", true);
            Text3D("23:59", t, new Vector3(-2f, 7.2f, 16.2f), Vector3.zero, 6f, new Color(0.7f, 0.15f, 0.15f), 3f);

            // P1 · Candado de las rosas
            var candado = Box("Candado", t, new Vector3(-6f, 0.55f, -4f), new Vector3(1.4f, 1.1f, 0.6f), Metal);
            for (int i = 0; i < 4; i++)
                Prim(PrimitiveType.Cylinder, "Dial", t, new Vector3(-6.48f + i * 0.32f, 0.85f, -4.32f), new Vector3(0.22f, 0.03f, 0.22f), Oro, false, new Vector3(90, 0, 0));
            Text3D("A UNA ROSA · SONETO XXIII", t, new Vector3(-6f, 0.45f, -4.32f), Vector3.zero, 1.1f, new Color(0.9f, 0.85f, 0.7f), 1.4f);
            var p1 = MakePuzzle(candado, "P1", "patio", "Abre el candado de las rosas, junto al invernadero.");
            p1.placa = "Placa del candado: «A una rosa», de Luis de Góngora, y el primer terceto del Soneto XXIII, de Garcilaso de la Vega.";
            p1.poemasPlaca = new[] { "gongora_rosa", "garcilaso_xxiii" };
            p1.requiereMundo = true;
            p1.objetivoMundo = "Entra al invernadero y corta una rosa justo cuando suene la campana.";
            p1.dialogoPaneles = "patio_candado_ok";
            p1.dialogoResuelto = "patio_rosa_ok";
            p1.objetosGuiado = new[] { "rosa" };
            p1.campana = bell;
            Trigger(candado, p1, "Revisar el candado de las rosas");

            // Invernadero (se abre con el candado)
            var inv = RoomX("Invernadero", new Vector3(-11f, 0, 4f), new Vector3(6, 3, 6), new float[0], new float[0], new[] { 0f }, new float[0], Vidrio, Piso, true);
            var reja = Box("Reja", inv.transform, new Vector3(3f, 1.3f, 0), new Vector3(0.1f, 2.6f, 1.8f), Metal);
            p1.ocultarAlTerminarPaneles = new[] { reja };
            var nota = Box("Nota", inv.transform, new Vector3(-2.8f, 1.5f, 1.5f), new Vector3(0.05f, 0.5f, 0.4f), Papel);
            Inspect(nota, "Leer la nota del invernadero", "Nota en el invernadero", "Alguien escribió con plumón: «coged de vuestra alegre primavera / el dulce fruto, antes que el tiempo airado / cubra de nieve la hermosa cumbre». Debajo: «Las rosas abren solo un instante».", "garcilaso_xxiii");
            PointLight(inv.transform, new Vector3(0, 2.6f, 0), new Color(0.8f, 0.9f, 0.8f), 0.8f, 5f);
            var rosas = Group("Rosas");
            for (int i = 0; i < 3; i++)
            {
                var r = Group("Rosal", rosas.transform, new Vector3(-12.6f, 0, 2.4f + i * 1.6f));
                Prim(PrimitiveType.Cylinder, "Tallo", r.transform, new Vector3(0, 0.45f, 0), new Vector3(0.05f, 0.45f, 0.05f), Verde, false);
                Prim(PrimitiveType.Sphere, "Hojas", r.transform, new Vector3(0, 0.55f, 0), new Vector3(0.45f, 0.3f, 0.45f), Verde, false);
                var boton = Prim(PrimitiveType.Sphere, "Boton", r.transform, new Vector3(0, 0.98f, 0), new Vector3(0.1f, 0.14f, 0.1f), Boton, false);
                var flor = Prim(PrimitiveType.Sphere, "Flor", r.transform, new Vector3(0, 1.02f, 0), new Vector3(0.28f, 0.2f, 0.28f), Rosa, false);
                var marchita = Prim(PrimitiveType.Sphere, "Marchita", r.transform, new Vector3(0.08f, 0.88f, 0), new Vector3(0.18f, 0.1f, 0.18f), Marchita, false);
                var col = r.AddComponent<BoxCollider>(); col.size = new Vector3(0.7f, 1.3f, 0.7f); col.center = new Vector3(0, 0.65f, 0);
                var rb = r.AddComponent<RoseBush>();
                rb.puzzle = p1; rb.boton = boton; rb.flor = flor; rb.marchita = marchita;
            }

            // H · Kiosco con las tres radios
            var kiosco = Group("Kiosco", t, new Vector3(8f, 0, 2f));
            var k = kiosco.transform;
            Box("Techo", k, new Vector3(0, 3.1f, 0), new Vector3(4.4f, 0.2f, 4.4f), Madera);
            foreach (var c in new[] { new Vector3(-2, 1.5f, -2), new Vector3(2, 1.5f, -2), new Vector3(-2, 1.5f, 2), new Vector3(2, 1.5f, 2) })
                Prim(PrimitiveType.Cylinder, "Poste", k, c, new Vector3(0.15f, 1.5f, 0.15f), Madera);
            Box("Mesa", k, new Vector3(0, 0.45f, 0.6f), new Vector3(2.8f, 0.9f, 0.9f), Madera);
            PointLight(k, new Vector3(0, 2.6f, 0), new Color(1f, 0.72f, 0.45f), 1.1f, 6f);
            var tablero = Box("Tablero", k, new Vector3(0, 1.7f, 1.3f), new Vector3(2.6f, 1.2f, 0.08f), Papel);
            Text3D("CARMÍNICA · APOSTRÓFICA · ENUNCIATIVA", k, new Vector3(0, 2.15f, 1.25f), Vector3.zero, 1.1f, new Color(0.25f, 0.15f, 0.08f), 2.4f);
            var h = MakePuzzle(tablero, "H", "radios", "Sintoniza las tres radios del kiosco y pega sus etiquetas.");
            h.flagsRequeridos = new[] { "radio_0", "radio_1", "radio_2" };
            h.mensajeBloqueado = "Primero sintoniza las tres radios de la mesa.";
            h.dialogoResuelto = "radios_ok";
            h.poemasAlResolver = new[] { "calderon_estas" };
            Trigger(tablero, h, "Pegar las etiquetas de las radios");
            float[] estaciones = { 23f, 71f, 46f };
            string[] poemas = { null, "garcilaso_xxiii", "calderon_estas" }; // Boscán se completa en la Biblioteca
            for (int i = 0; i < 3; i++)
            {
                var radio = Box("Radio" + (i + 1), k, new Vector3(-0.9f + i * 0.9f, 1.08f, 0.6f), new Vector3(0.55f, 0.36f, 0.3f), i == 1 ? Tela : Madera);
                Prim(PrimitiveType.Cylinder, "Dial", k, new Vector3(-0.9f + i * 0.9f + 0.12f, 1.08f, 0.44f), new Vector3(0.12f, 0.02f, 0.12f), Oro, false, new Vector3(90, 0, 0));
                Text3D((i + 1).ToString(), k, new Vector3(-0.9f + i * 0.9f - 0.12f, 1.1f, 0.44f), Vector3.zero, 1.6f, new Color(1f, 0.85f, 0.5f), 0.3f);
                var rt = radio.AddComponent<RadioTuner>();
                rt.tablero = h; rt.indice = i; rt.estacion = estaciones[i]; rt.poemaId = poemas[i];
            }
            kiosco.AddComponent<StaticSource>().radio = 5f;

            // Ambiente
            Lamp(t, new Vector3(-4f, 0, -12f)); Lamp(t, new Vector3(5f, 0, -6f)); Lamp(t, new Vector3(-8f, 0, 10f)); Lamp(t, new Vector3(10f, 0, 12f));
            Box("Banca", t, new Vector3(-2f, 0.25f, -8f), new Vector3(2f, 0.5f, 0.5f), Madera);
            Box("Banca", t, new Vector3(4f, 0.25f, 8f), new Vector3(2f, 0.5f, 0.5f), Madera);
            DoradoDecor(t, new Vector3(-6f, 0, -1.5f), 180, "CARPE DIEM");

            var zone = AddZone("Patio de las rosas", "patio_intro", "notif_patio", "patio", "radios");
            zone.tasks = new ZoneTask[] { p1, h };
            salida.mensajeCerrada = "La puerta de la galería no abre.";
            AddWorld(patio, inv);
            return SaveScene("Patio");
        }

        // ------------------------------------------------------------ 3 · Galería
        static string BuildGaleria()
        {
            NewScene(new Color(0.30f, 0.30f, 0.33f), 0.04f, new Color(0.26f, 0.26f, 0.29f));
            var hall = RoomX("Galeria", Vector3.zero, new Vector3(30, 4, 10), new[] { 12f }, new[] { -8f, 0f, 8f }, new float[0], new[] { 0f });
            var t = hall.transform;
            AddPlayer(new Vector3(-13f, 0.05f, 0), 90);
            AddDoor(t, new Vector3(-15f, 0, 0), 90, "Patio", "Mejor seguir adelante.", true);
            var salida = AddDoor(t, new Vector3(12f, 0, 5f), 0, "Biblioteca", "Aún falta algo en la galería.");
            foreach (var x in new[] { -11f, -4f, 3f, 10f }) PointLight(t, new Vector3(x, 3.4f, 0), new Color(1f, 0.8f, 0.6f), 7f, 12f);

            // Salas laterales (al sur)
            var lab = RoomX("Laboratorio", new Vector3(-8f, 0, -8f), new Vector3(6, 3.5f, 6), new float[0], new float[0], new float[0], new float[0], null, null, true, "N");
            var sala = RoomX("SalaProfesores", new Vector3(0f, 0, -8f), new Vector3(6, 3.5f, 6), new float[0], new float[0], new float[0], new float[0], null, null, true, "N");
            var bodega = RoomX("BodegaUtileria", new Vector3(8f, 0, -8f), new Vector3(6, 3.5f, 6), new float[0], new float[0], new float[0], new float[0], null, null, true, "N");
            foreach (var (room, label) in new[] { (lab, "LABORATORIO"), (sala, "SALA DE PROFESORES"), (bodega, "BODEGA DE UTILERÍA") })
            {
                Text3D(label, t, new Vector3(room.transform.position.x, 3.1f, -4.8f), new Vector3(0, 180, 0), 1.6f, null, 3f);
                Box("Mesa", room.transform, new Vector3(0, 0.45f, -1.5f), new Vector3(2.4f, 0.9f, 1f), Madera);
                PointLight(room.transform, new Vector3(0, 3f, -1f), new Color(0.8f, 0.85f, 1f), 4.5f, 8f);
            }
            Item("calavera", lab.transform, new Vector3(-0.5f, 0.9f, -1.5f), g => { Prim(PrimitiveType.Sphere, "Craneo", g, new Vector3(0, 0.15f, 0), new Vector3(0.24f, 0.22f, 0.26f), Hueso, false); Box("Mandibula", g, new Vector3(0, 0.03f, 0.06f), new Vector3(0.14f, 0.05f, 0.12f), Hueso, false); });
            Item("ancla", lab.transform, new Vector3(0.6f, 0.9f, -1.5f), g => { Box("Cana", g, new Vector3(0, 0.2f, 0), new Vector3(0.05f, 0.4f, 0.05f), Metal, false); Box("Brazos", g, new Vector3(0, 0.03f, 0), new Vector3(0.3f, 0.05f, 0.05f), Metal, false); });
            Item("reloj", sala.transform, new Vector3(-0.5f, 0.9f, -1.5f), g => { Prim(PrimitiveType.Cylinder, "Base", g, new Vector3(0, 0.02f, 0), new Vector3(0.18f, 0.02f, 0.18f), Oro, false); Prim(PrimitiveType.Cylinder, "Vidrio", g, new Vector3(0, 0.17f, 0), new Vector3(0.12f, 0.13f, 0.12f), Vidrio, false); Prim(PrimitiveType.Cylinder, "Tapa", g, new Vector3(0, 0.32f, 0), new Vector3(0.18f, 0.02f, 0.18f), Oro, false); });
            Item("llave", sala.transform, new Vector3(0.6f, 0.9f, -1.5f), g => Box("Llave", g, new Vector3(0, 0.03f, 0), new Vector3(0.25f, 0.04f, 0.06f), Oro, false));
            Item("corona", bodega.transform, new Vector3(-0.5f, 0.9f, -1.5f), g => { Prim(PrimitiveType.Cylinder, "Aro", g, new Vector3(0, 0.08f, 0), new Vector3(0.28f, 0.08f, 0.28f), Oro, false); for (int i = 0; i < 5; i++) { float a = i * Mathf.PI * 2 / 5; Prim(PrimitiveType.Sphere, "Punta", g, new Vector3(Mathf.Cos(a) * 0.13f, 0.2f, Mathf.Sin(a) * 0.13f), Vector3.one * 0.05f, Rosa, false); } });
            Item("pelota", bodega.transform, new Vector3(0.6f, 0.9f, -1.5f), g => Prim(PrimitiveType.Sphere, "Pelota", g, new Vector3(0, 0.12f, 0), Vector3.one * 0.24f, Mat("Pelota", new Color(0.7f, 0.25f, 0.2f)), false));
            Box("Pedestal", t, new Vector3(-10f, 0.5f, 3.6f), new Vector3(0.6f, 1f, 0.6f), Muro);
            Item("paloma", t, new Vector3(-10f, 1f, 3.6f), g => { Prim(PrimitiveType.Sphere, "Cuerpo", g, new Vector3(0, 0.1f, 0), new Vector3(0.18f, 0.14f, 0.28f), Papel, false); Prim(PrimitiveType.Sphere, "Cabeza", g, new Vector3(0, 0.2f, 0.12f), Vector3.one * 0.09f, Papel, false); });

            // Recreaciones de vanitas (Pereda, Murillo)
            var sueno = Frame("SuenoCaballero", t, new Vector3(0, 2.2f, 4.82f), Vector3.zero, 2.4f, 1.6f, Tela);
            Prim(PrimitiveType.Sphere, "Calavera", sueno.transform, new Vector3(-0.6f, -0.4f, -0.05f), new Vector3(0.22f, 0.2f, 0.05f), Hueso, false);
            Box("Angel", sueno.transform, new Vector3(0.7f, 0.2f, -0.05f), new Vector3(0.3f, 0.7f, 0.03f), Papel, false);
            Inspect(sueno, "Mirar el cuadro", "El sueño del caballero (recreación)",
                "Recreación de la composición de Antonio de Pereda (s. XVII): un caballero duerme junto a una mesa con riquezas, una calavera, un reloj, flores y una corona. Un ángel sostiene una cinta: Aeterna pungit, cito volat et occidit. Todo lo valioso de la mesa es pasajero.");
            var omazur = Frame("NicolasOmazur", t, new Vector3(-4f, 2.1f, -4.82f), new Vector3(0, 180, 0), 1.1f, 1.4f, Tela);
            Prim(PrimitiveType.Sphere, "Calavera", omazur.transform, new Vector3(0, -0.3f, -0.05f), new Vector3(0.2f, 0.18f, 0.05f), Hueso, false);
            Inspect(omazur, "Mirar el retrato", "Nicolás Omazur (recreación)",
                "Recreación del retrato que pintó Murillo: un hombre elegante mira de frente mientras sostiene una calavera. Incluso un retrato de alguien importante recuerda que la vida termina.");

            // P3 · Bodegón del caballero
            var mesa = Box("Bodegon", t, new Vector3(0, 0.45f, 2.6f), new Vector3(3f, 0.9f, 1.3f), Madera);
            Box("Cinta", t, new Vector3(0, 2.85f, 4.6f), new Vector3(4.2f, 0.35f, 0.04f), Papel);
            Text3D("AETERNA PUNGIT, CITO VOLAT ET OCCIDIT", t, new Vector3(0, 2.85f, 4.55f), Vector3.zero, 1.5f, new Color(0.3f, 0.15f, 0.05f), 4f);
            for (int i = 0; i < 4; i++) Box("Hueco", t, new Vector3(-1.05f + i * 0.7f, 0.91f, 2.6f), new Vector3(0.45f, 0.01f, 0.45f), Oscuro, false);
            var colocados = Group("ObjetosColocados", t, new Vector3(0, 0.92f, 2.6f));
            Prim(PrimitiveType.Sphere, "Calavera", colocados.transform, new Vector3(-1.05f, 0.13f, 0), new Vector3(0.24f, 0.22f, 0.26f), Hueso, false);
            Prim(PrimitiveType.Cylinder, "Reloj", colocados.transform, new Vector3(-0.35f, 0.16f, 0), new Vector3(0.14f, 0.16f, 0.14f), Vidrio, false);
            Prim(PrimitiveType.Sphere, "Rosa", colocados.transform, new Vector3(0.35f, 0.1f, 0), new Vector3(0.22f, 0.15f, 0.22f), Rosa, false);
            Prim(PrimitiveType.Cylinder, "Corona", colocados.transform, new Vector3(1.05f, 0.08f, 0), new Vector3(0.28f, 0.08f, 0.28f), Oro, false);
            colocados.SetActive(false);
            var p3 = MakePuzzle(mesa, "P3", "galeria", "Completa la mesa del caballero con los objetos que hablan de lo pasajero.");
            p3.placa = "Una mesa armada como la del cuadro de Pereda, con cuatro huecos. En la cinta del ángel: Aeterna pungit, cito volat et occidit («eternamente hiere, rápido vuela y mata»).";
            p3.mostrarAlResolver = new[] { colocados };
            p3.objetosGuiado = new[] { "calavera", "reloj", "rosa", "corona" };
            Trigger(mesa, p3, "Revisar la mesa del caballero");

            // P4 · El retrato que miente
            var atril = Box("Atril", t, new Vector3(7f, 0.55f, 2.4f), new Vector3(0.6f, 1.1f, 0.4f), Madera);
            var p4 = MakePuzzle(atril, "P4", "galeria", "Lee el soneto del atril y cuelga los cuatro retratos en orden.");
            p4.placa = "En el atril, el soneto que Sor Juana Inés de la Cruz escribió sobre su propio retrato. Junto a la pared hay cuatro retratos de Ale, cada uno más deteriorado.";
            p4.poemasPlaca = new[] { "sorjuana_este" };
            p4.poemaContexto = "sorjuana_este";
            Trigger(atril, p4, "Leer el soneto del atril");
            var enElSuelo = Group("RetratosEnElSuelo", t, new Vector3(7f, 0, 3.9f));
            for (int i = 0; i < 4; i++)
                Box("Retrato", enElSuelo.transform, new Vector3(-1.2f + i * 0.8f, 0.5f, 0), new Vector3(0.7f, 0.9f, 0.06f), Tela, false, new Vector3(-15, 0, 0));
            var colgados = Group("RetratosColgados", t);
            Material[] caras = { Hueso, Marchita, Oscuro, Muro };
            for (int i = 0; i < 4; i++)
            {
                var f = Frame("Retrato" + (i + 1), colgados.transform, new Vector3(4f + i * 2f, 2.1f, 4.82f), Vector3.zero, 1f, 1.3f, i == 3 ? Muro : Tela);
                if (i < 3) Prim(PrimitiveType.Sphere, "Rostro", f.transform, new Vector3(0, 0.1f, -0.05f), new Vector3(0.45f, 0.55f, 0.05f), caras[i], false);
            }
            colgados.SetActive(false);
            for (int i = 0; i < 4; i++) Box("Gancho", t, new Vector3(4f + i * 2f, 2.9f, 4.82f), new Vector3(0.08f, 0.08f, 0.08f), Oro, false);
            p4.ocultarAlResolver = new[] { enElSuelo };
            p4.mostrarAlResolver = new[] { colgados };

            // P4-C · La figura velada (El mágico prodigioso)
            var figura = Group("FiguraVelada", t, new Vector3(13.6f, 0, 0));
            var ft = figura.transform;
            var velo = Group("Velo", ft);
            Prim(PrimitiveType.Cylinder, "Manto", velo.transform, new Vector3(0, 0.95f, 0), new Vector3(0.6f, 0.95f, 0.6f), Papel, false);
            Prim(PrimitiveType.Sphere, "Capucha", velo.transform, new Vector3(0, 1.95f, 0), Vector3.one * 0.45f, Papel, false);
            var esq = Group("Esqueleto", ft);
            Prim(PrimitiveType.Sphere, "Craneo", esq.transform, new Vector3(0, 1.85f, 0), new Vector3(0.26f, 0.3f, 0.28f), Hueso, false);
            for (int i = 0; i < 4; i++) Box("Costilla", esq.transform, new Vector3(0, 1.45f - i * 0.12f, 0), new Vector3(0.4f - i * 0.04f, 0.04f, 0.22f), Hueso, false);
            Box("Columna", esq.transform, new Vector3(0, 1.2f, -0.08f), new Vector3(0.06f, 0.9f, 0.06f), Hueso, false);
            Box("Cadera", esq.transform, new Vector3(0, 0.8f, 0), new Vector3(0.36f, 0.12f, 0.16f), Hueso, false);
            Prim(PrimitiveType.Cylinder, "PiernaI", esq.transform, new Vector3(-0.12f, 0.38f, 0), new Vector3(0.06f, 0.38f, 0.06f), Hueso, false);
            Prim(PrimitiveType.Cylinder, "PiernaD", esq.transform, new Vector3(0.12f, 0.38f, 0), new Vector3(0.06f, 0.38f, 0.06f), Hueso, false);
            Box("RopaDeAle", esq.transform, new Vector3(0, 1.3f, 0.02f), new Vector3(0.46f, 0.5f, 0.2f), Mat("RopaAle", new Color(0.2f, 0.28f, 0.45f)), false);
            var luzVelo = PointLight(ft, new Vector3(-0.8f, 2.6f, 0), new Color(1f, 0.6f, 0.4f), 0.9f, 5f);
            var fc = figura.AddComponent<BoxCollider>(); fc.size = new Vector3(1.2f, 2.3f, 1.2f); fc.center = new Vector3(0, 1.15f, 0);
            var vf = figura.AddComponent<VeiledFigure>();
            vf.zona = "galeria"; vf.objetivo = "Al fondo de la galería espera una figura con un velo.";
            vf.requiere = p4; vf.velo = velo; vf.esqueleto = esq; vf.luz = luzVelo;
            figura.AddComponent<StaticSource>().radio = 5f;

            DoradoDecor(t, new Vector3(-4f, 0, 4.4f), 0, "VANITAS VANITATUM");
            var zone = AddZone("Galería", "galeria_intro", "notif_galeria", "galeria");
            zone.tasks = new ZoneTask[] { p3, p4, vf };
            _ = salida;
            AddWorld(hall, lab, sala, bodega);
            return SaveScene("Galeria");
        }

        // ------------------------------------------------------------ 4 · Biblioteca
        static string BuildBiblioteca()
        {
            NewScene(new Color(0.28f, 0.27f, 0.25f), 0.04f, new Color(0.28f, 0.25f, 0.22f));
            Random.InitState(5); // escena determinista
            var room = RoomX("Biblioteca", Vector3.zero, new Vector3(16, 4.5f, 12), new float[0], new[] { 0f }, new float[0], new[] { 2f });
            var t = room.transform;
            AddPlayer(new Vector3(0, 0.05f, -4.6f), 0);
            AddDoor(t, new Vector3(0, 0, -6f), 0, "Galería", "Mejor seguir adelante.", true);
            AddDoor(t, new Vector3(-8f, 0, 2f), 90, "Teatro", "Primero el estante del soneto.");
            PointLight(t, new Vector3(0, 3.8f, -1f), new Color(1f, 0.78f, 0.5f), 8f, 14f);
            PointLight(t, new Vector3(0, 3.4f, 4.0f), new Color(1f, 0.78f, 0.5f), 5f, 7f);
            PointLight(t, new Vector3(0, 1.8f, 1f), new Color(1f, 0.7f, 0.4f), 3f, 5f);

            Material[] lomos = { Mat("LomoA", new Color(0.45f, 0.15f, 0.12f)), Mat("LomoB", new Color(0.15f, 0.22f, 0.40f)), Mat("LomoC", new Color(0.18f, 0.35f, 0.18f)), Mat("LomoD", new Color(0.35f, 0.2f, 0.38f)) };
            // estanterías decorativas
            foreach (var (x, z, rot) in new[] { (7.6f, -3f, 90f), (7.6f, 1f, 90f), (7.6f, 4.5f, 90f), (-7.6f, -3.5f, 90f), (-7.6f, 5f, 90f) })
            {
                var e = Group("Estanteria", t, new Vector3(x, 0, z));
                e.transform.localEulerAngles = new Vector3(0, rot, 0);
                Box("Mueble", e.transform, new Vector3(0, 1.75f, 0), new Vector3(2.4f, 3.5f, 0.5f), Madera);
                for (int f = 0; f < 4; f++)
                    for (int b = 0; b < 7; b++)
                        Box("Libro", e.transform, new Vector3(-1f + b * 0.32f, 0.5f + f * 0.85f, x > 0 ? -0.3f : 0.3f), new Vector3(0.22f, 0.55f, 0.12f), lomos[(b + f) % 4], false);
            }

            // P5 · estante del soneto (4 / 4 / 3 / 3)
            var estante = Group("EstanteSoneto", t, new Vector3(0, 0, 5.6f));
            var et = estante.transform;
            Box("Fondo", et, new Vector3(0, 1.6f, 0.15f), new Vector3(3.4f, 3.2f, 0.1f), Madera);
            Box("LadoI", et, new Vector3(-1.65f, 1.6f, 0), new Vector3(0.1f, 3.2f, 0.4f), Madera);
            Box("LadoD", et, new Vector3(1.65f, 1.6f, 0), new Vector3(0.1f, 3.2f, 0.4f), Madera);
            for (int r = 0; r < 4; r++)
            {
                Box("Repisa", et, new Vector3(0, 0.55f + r * 0.7f, 0), new Vector3(3.2f, 0.06f, 0.4f), Madera);
                Text3D("Repisa " + (r + 1), et, new Vector3(-1.15f, 0.47f + r * 0.7f, -0.22f), Vector3.zero, 1.3f, new Color(0.95f, 0.85f, 0.6f), 1.2f);
            }
            var col = estante.AddComponent<BoxCollider>(); col.size = new Vector3(3.4f, 3.2f, 0.6f); col.center = new Vector3(0, 1.6f, 0);
            var ordenados = Group("LibrosOrdenados", et);
            int[] porRepisa = { 4, 4, 3, 3 };
            string rimas = "ABBAABBACDCDCD";
            int n = 0;
            for (int r = 0; r < 4; r++)
                for (int b = 0; b < porRepisa[r]; b++, n++)
                    Box("Libro" + (n + 1), ordenados.transform, new Vector3(-0.6f + b * 0.4f, 0.85f + r * 0.7f, 0), new Vector3(0.3f, 0.55f, 0.3f), lomos[rimas[n] - 'A'], false);
            ordenados.SetActive(false);
            var mesa = Box("MesaLectura", t, new Vector3(0, 0.4f, 1f), new Vector3(2.6f, 0.8f, 1.4f), Madera);
            var pila = Group("LibrosTirados", t, new Vector3(0, 0.8f, 1f));
            for (int i = 0; i < 14; i++)
                Box("Libro", pila.transform, new Vector3(Random.Range(-1f, 1f), 0.05f + (i % 4) * 0.08f, Random.Range(-0.5f, 0.5f)), new Vector3(0.45f, 0.08f, 0.3f), lomos[i % 4], false, new Vector3(0, Random.Range(0, 180), 0));
            var p5 = MakePuzzle(estante, "P5", "biblioteca", "Ordena en el estante los 14 libros del soneto.");
            p5.placa = "Catorce libros tirados sobre la mesa. Cada lomo tiene un verso de un soneto de Juan Boscán. El estante tiene cuatro repisas: cuatro, cuatro, tres y tres libros.";
            p5.poemaContexto = "boscan_dulce";
            p5.poemasAlResolver = new[] { "boscan_dulce" };
            p5.ocultarAlResolver = new[] { pila };
            p5.mostrarAlResolver = new[] { ordenados };
            Trigger(estante, p5, "Ordenar el estante del soneto");
            Trigger(mesa, p5, "Revisar los libros tirados");

            DoradoDecor(t, new Vector3(-4f, 0, 5.4f), 0, "VITA SOMNIUM");
            var zone = AddZone("Biblioteca", "biblioteca_intro", "notif_biblioteca", "biblioteca");
            zone.tasks = new ZoneTask[] { p5 };
            AddWorld(room);
            return SaveScene("Biblioteca");
        }
    }
}
