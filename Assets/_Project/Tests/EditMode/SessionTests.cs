using System.Collections.Generic;
using EnganoColorido.Assessment;
using EnganoColorido.Data;
using EnganoColorido.Save;
using EnganoColorido.Session;
using NUnit.Framework;

namespace EnganoColorido.Tests
{
    public class SessionTests
    {
        static ContentDatabase DB => TestData.DB;

        [Test] public void Modo45_EnviaAlTeatroEnElMinuto29()
        {
            var p = new SessionPacer(DB.sesion, 45);
            p.EnterZone("galeria");
            p.Tick(28.9f * 60);
            Assert.IsFalse(p.ShouldForceTheater);
            p.Tick(0.2f * 60);
            Assert.IsTrue(p.ShouldForceTheater);
            p.EnterZone("teatro");
            Assert.IsFalse(p.ShouldForceTheater, "en el teatro ya no se envía");
        }

        [Test] public void Modo90_EnviaAlTeatroEnElMinuto47()
        {
            var p = new SessionPacer(DB.sesion, 90);
            p.EnterZone("torre");
            p.Tick(46.9f * 60);
            Assert.IsFalse(p.ShouldForceTheater);
            p.Tick(0.2f * 60);
            Assert.IsTrue(p.ShouldForceTheater);
        }

        [Test] public void PresupuestosSumanLoDelGDD()
        {
            Assert.AreEqual(40, new SessionPacer(DB.sesion, 45).PlannedTotalMinutes, 0.01f);
            Assert.AreEqual(60, new SessionPacer(DB.sesion, 90).PlannedTotalMinutes, 0.01f);
        }

        [Test] public void PistasAl100_GuiadoAl130()
        {
            var p = new SessionPacer(DB.sesion, 45);
            p.EnterZone("patio"); // 7 min
            p.Tick(6.9f * 60);
            Assert.IsFalse(p.HintsAdvanced);
            p.Tick(0.2f * 60);
            Assert.IsTrue(p.HintsAdvanced);
            Assert.IsFalse(p.GuidedOffered);
            p.Tick(2.1f * 60); // 9,2 min > 9,1 (130 %)
            Assert.IsTrue(p.GuidedOffered);
        }

        [Test] public void Modo45_SaltaCapillaYTorre()
        {
            var p = new SessionPacer(DB.sesion, 45);
            var skipped = new List<string>();
            Assert.AreEqual("galeria", p.NextZone("radios", skipped));
            CollectionAssert.Contains(skipped, "capilla");
            skipped.Clear();
            Assert.AreEqual("teatro", p.NextZone("biblioteca", skipped));
            CollectionAssert.Contains(skipped, "torre");
        }

        [Test] public void Modo90_SaltaCapillaSoloSiVaAtrasado()
        {
            var p = new SessionPacer(DB.sesion, 90);
            p.Tick(15 * 60); // plan: la capilla empieza en el minuto 17
            Assert.AreEqual("capilla", p.NextZone("radios"));
            p.Tick(5 * 60);  // 20' > 17 + 2 de tolerancia
            Assert.AreEqual("galeria", p.NextZone("radios"));
        }

        [Test] public void SalidaForzadaYRutaFinal()
        {
            var p = new SessionPacer(DB.sesion, 45);
            p.EnterZone("teatro");
            p.Tick(37 * 60);
            Assert.IsTrue(p.ShouldForceExitTest);
            Assert.AreEqual("salida", p.NextZone("teatro"));
        }
    }

    public class EvaluacionTests
    {
        static SaveData Sesion()
        {
            var s = new SaveData { alias = "Kiro;\"x\"", curso = "2°A", modo = 45, fecha = "2026-10-16 10:00" };
            var ev = TestData.DB.evaluacion;
            for (int i = 0; i < 5; i++) s.diagnostico.Add(new Respuesta { id = ev.diagnostico[i].id, correcta = i < 2 }); // 2/5
            for (int i = 0; i < 8; i++) s.salida.Add(new Respuesta { id = ev.salida[i].id, correcta = i != 4 && i != 7 }); // 4/5 y 2/3
            s.puzzles.Add(new PuzzleLog { id = "P1", intentos = 3, pistas = 1, segundos = 120.4f, resuelto = true });
            s.verso.AddRange(new[] { "=HYPERLINK()", "No quiero un rostro pintado" });
            s.elapsed = 39.5f * 60;
            return s;
        }

        [Test] public void PuntajeYGanancia()
        {
            var s = Sesion();
            Assert.AreEqual(2, Scoring.Diagnostico(s));
            Assert.AreEqual(4, Scoring.Salida15(s));
            Assert.AreEqual(2, Scoring.Transferencia(s));
            Assert.AreEqual(2, Scoring.Ganancia(s));
        }

        [Test] public void CsvTieneMismasColumnasQueEncabezado()
        {
            var csv = Exporter.Csv(Sesion());
            var lines = csv.Split(new[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(2, lines.Length);
            int headerCols = lines[0].Split(';').Length;
            StringAssert.StartsWith("\"Kiro;\"\"x\"\"\";2°A;45;", lines[1]); // alias entre comillas por el ';'
            Assert.AreEqual(headerCols, CountCsvCells(lines[1]));
            StringAssert.Contains("'=HYPERLINK()", lines[1]);                 // sin fórmulas
            StringAssert.Contains(";39.5", lines[1]);                        // decimales con punto
        }

        [Test] public void ExportEscribeCsvConBomYJson()
        {
            var dir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ec_test_" + System.Guid.NewGuid().ToString("N"));
            var path = Exporter.Export(Sesion(), dir);
            var bytes = System.IO.File.ReadAllBytes(path);
            Assert.AreEqual(0xEF, bytes[0]); Assert.AreEqual(0xBB, bytes[1]); Assert.AreEqual(0xBF, bytes[2]);
            Assert.IsTrue(System.IO.File.Exists(System.IO.Path.ChangeExtension(path, ".json")));
            StringAssert.Contains("Kiro_", System.IO.Path.GetFileName(path));
            StringAssert.Contains("1 sesiones", CourseSummary.Build(dir));
            System.IO.Directory.Delete(dir, true);
        }

        [Test] public void GuardarYCargarConservaLaSesion()
        {
            var s = Sesion();
            s.inventario.Add("rosa"); s.SetFlag("camarin_notas"); s.zona = "galeria";
            var back = SaveSystem.FromJson(SaveSystem.ToJson(s));
            Assert.AreEqual(s.alias, back.alias);
            Assert.AreEqual("galeria", back.zona);
            CollectionAssert.Contains(back.inventario, "rosa");
            Assert.IsTrue(back.Flag("camarin_notas"));
            Assert.AreEqual(3, back.puzzles[0].intentos);
            Assert.AreEqual(s.elapsed, back.elapsed, 0.01f);
        }

        [Test] public void AlternativasSeBarajanSinPerderNinguna()
        {
            var q = TestData.DB.evaluacion.diagnostico[0];
            var opts = AssessmentRunner.Options(q);
            var sh = AssessmentRunner.Shuffle(opts, new System.Random(3));
            CollectionAssert.AreEquivalent(opts, sh);
            Assert.IsTrue(AssessmentRunner.Answer(q, q.correcta).correcta);
            Assert.IsFalse(AssessmentRunner.Answer(q, q.distractores[0]).correcta);
        }

        static int CountCsvCells(string line)
        {
            int n = 1; bool q = false;
            foreach (var c in line) { if (c == '"') q = !q; else if (c == ';' && !q) n++; }
            return n;
        }
    }

    public static class TestData
    {
        static ContentDatabase _db;
        public static ContentDatabase DB => _db ??= ContentLoader.Load(System.IO.Path.Combine(UnityEngine.Application.dataPath, "_Project/Data"));
    }
}
