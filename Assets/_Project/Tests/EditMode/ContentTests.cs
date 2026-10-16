using System.Collections.Generic;
using System.IO;
using EnganoColorido.Data;
using EnganoColorido.Puzzles;
using NUnit.Framework;

namespace EnganoColorido.Tests
{
    public class ContentTests
    {
        static ContentDatabase DB => TestData.DB;
        static string DataDir => Path.Combine(UnityEngine.Application.dataPath, "_Project/Data");

        [Test] public void ContenidoSinProblemas()
        {
            var problemas = ContentValidator.Check(DB);
            Assert.IsEmpty(problemas, string.Join("\n", problemas));
        }

        [Test] public void QuevedoSoloEnLaPruebaDeSalida()
        {
            var versos = new List<string>();
            foreach (var v in DB.evaluacion.poemaSalida.versos) if (v.Length > 12) versos.Add(v.Trim('¡', '!', ':', ',', ';', '.', ' '));
            foreach (var file in new[] { "poems", "fichas", "topicos", "puzzles", "dialogue", "session" })
            {
                var text = File.ReadAllText(Path.Combine(DataDir, file + ".json"));
                StringAssert.DoesNotContain("Quevedo", text, file);
                foreach (var v in versos) StringAssert.DoesNotContain(v, text, $"{file}.json contiene un verso de Quevedo");
            }
            foreach (var q in DB.evaluacion.diagnostico) Assert.AreNotEqual("quevedo_fue", q.contexto);
        }

        [Test] public void FichasMaximo80Palabras()
        {
            foreach (var f in DB.fichas.fichas)
                Assert.LessOrEqual(ContentValidator.Palabras(f.texto), 80, f.id);
        }

        [Test] public void ClotaldoDiceLoDelHacerBien()
        {
            var vida = DB.Poema("calderon_vida");
            int i = System.Array.FindIndex(vida.versos, v => v.Contains("que aun en sueños"));
            StringAssert.StartsWith("CLOTALDO", vida.versos[i]);
        }

        [Test] public void CadaPuzzleTieneFichaTresPistasYTags()
        {
            foreach (var id in ContentValidator.PuzzlesModo45)
            {
                var p = DB.Puzzle(id);
                Assert.IsNotNull(p, id);
                Assert.AreEqual(3, p.pistas.Length, id);
                Assert.IsNotEmpty(p.conceptTags, id);
                if (id != "TUVERSO") Assert.IsNotEmpty(p.fichas, id);
            }
        }
    }

    public class PuzzleTests
    {
        static ContentDatabase DB => TestData.DB;

        static IEnumerable<string> Ids => ContentValidator.PuzzlesModo45;

        [Test] public void SolucionValidaYDistractoresRechazados([ValueSource(nameof(Ids))] string id)
        {
            foreach (var paso in DB.Puzzle(id).pasos)
            {
                var sol = PuzzleValidation.Solution(paso);
                Assert.IsTrue(PuzzleValidation.Validate(paso, sol, out var fb), $"{id}/{paso.id}: la solución no valida ({fb})");
                if (paso.tipo == "pregunta")
                {
                    foreach (var d in paso.distractores)
                        Assert.IsFalse(PuzzleValidation.Validate(paso, new List<string> { d }, out _), $"{id}/{paso.id}: acepta «{d}»");
                    continue;
                }
                if (paso.tipo == "abba") continue;
                for (int i = 0; i < paso.slots.Length; i++)
                    foreach (var op in ContentDatabase.Opciones(paso, paso.slots[i]))
                    {
                        if (op == paso.slots[i].correcta) continue;
                        var wrong = new List<string>(sol) { [i] = op };
                        Assert.IsFalse(PuzzleValidation.Validate(paso, wrong, out _), $"{id}/{paso.id}: acepta «{op}» en «{paso.slots[i].etiqueta}»");
                    }
            }
        }

        [Test] public void EstanteValidaPorRepisa()
        {
            var paso = DB.Puzzle("P5").pasos[0];
            var sol = PuzzleValidation.Solution(paso);
            (sol[0], sol[3]) = (sol[3], sol[0]); // error en la repisa 1
            (sol[12], sol[13]) = (sol[13], sol[12]); // error en la repisa 4
            CollectionAssert.AreEquivalent(new[] { 1, 4 }, PuzzleValidation.WrongGroups(paso, sol));
        }

        [Test] public void JuicioNombraLaCategoriaDelConector()
        {
            var r1 = DB.Puzzle("JUICIO").pasos[0];
            var a = PuzzleValidation.Solution(r1);
            a[0] = "por lo tanto";
            Assert.IsFalse(PuzzleValidation.Juicio(r1, a, out var fb));
            StringAssert.Contains("consecuencia", fb);
            StringAssert.Contains("opone", fb);
            a = PuzzleValidation.Solution(r1);
            a[1] = "«hace a Dios ser mi cautivo»";
            Assert.IsFalse(PuzzleValidation.Juicio(r1, a, out fb));
            StringAssert.Contains("no responde a la tesis", fb);
        }

        [Test] public void TuVersoSoloAceptaAbba()
        {
            var p = DB.Puzzle("TUVERSO").pasos[0];
            string ado1 = "No quiero un rostro pintado", ado2 = "el reloj sigue a mi lado";
            string ida1 = "prefiero vivir mi vida", ida2 = "con la luz que no se olvida";
            Assert.IsTrue(PuzzleValidation.Abba(p, new[] { ado1, ida1, ida2, ado2 }, out _));
            Assert.IsTrue(PuzzleValidation.Abba(p, new[] { ida1, ado1, ado2, ida2 }, out _));
            Assert.IsFalse(PuzzleValidation.Abba(p, new[] { ado1, ado2, ida1, ida2 }, out _), "aabb");
            Assert.IsFalse(PuzzleValidation.Abba(p, new[] { ado1, ida1, ado2, ida2 }, out _), "abab");
            Assert.IsFalse(PuzzleValidation.Abba(p, new[] { ado1, ida1, ida1, ado2 }, out _), "repetido");
            Assert.IsFalse(PuzzleValidation.Abba(p, new[] { ado1, "todo lo cambia el tiempo", ida2, ado2 }, out _), "sin rima");
        }
    }
}
