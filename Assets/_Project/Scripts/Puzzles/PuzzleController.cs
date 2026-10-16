using System;
using System.Collections.Generic;
using EnganoColorido.Core;
using EnganoColorido.Data;
using EnganoColorido.Save;
using EnganoColorido.UI;
using EnganoColorido.World;
using UnityEngine;

namespace EnganoColorido.Puzzles
{
    /// Puzzle definido en puzzles.json: ficha previa → placa → pasos (paneles) → [parte en el mundo] → resuelto.
    /// Registra intentos, pistas, tiempo y si se resolvió con ayuda, con sus conceptTags.
    public class PuzzleController : ZoneTask, IPuzzle, IHintable
    {
        public string puzzleId;
        [TextArea] public string placa;          // texto de la placa (opcional)
        public string[] poemasPlaca;             // poemas que se muestran en la placa y quedan en el Cuaderno
        public string poemaContexto;             // poema que acompaña a las preguntas
        public string[] poemasAlResolver;        // poemas que quedan en el Cuaderno al resolver
        public string[] flagsRequeridos;         // p. ej. radios sintonizadas
        [TextArea] public string mensajeBloqueado = "Todavía no.";
        public bool requiereMundo;               // P1: después de los paneles falta cortar la rosa
        public string objetivoMundo;
        public string dialogoPaneles;            // al terminar los paneles (antes de la parte en el mundo)
        public string dialogoResuelto;
        public GameObject[] ocultarAlTerminarPaneles;
        public GameObject[] mostrarAlResolver;
        public GameObject[] ocultarAlResolver;
        public BellCycle campana;                // P1: suena mientras falta la parte en el mundo
        public string[] objetosGuiado;           // objetos que el modo guiado entrega (P3, P1)
        public PuzzleController requierePuzzle;  // Juicio: después del libreto
        public string dialogoAlAbrir;            // se dice una vez antes de la ficha (Juicio)
        public bool mundoDoradoAlAbrir;          // el Juicio ocurre en el Mundo Dorado
        public bool tuVerso;                     // Tu verso: elegir entre armar (abba) o escribir

        public event Action OnSolved;
        public string Id => puzzleId;
        Puzzle def;
        int hintsThisStep;
        float openedAt = -1;

        Puzzle Def => def ??= ContentDatabase.Instance.Puzzle(puzzleId);
        SaveData S => Game.State;
        PuzzleLog Log => S.Log(puzzleId, Def?.conceptTags);
        string StepFlag(int i) => $"{puzzleId}:paso{i}";
        string PanelsFlag => puzzleId + ":paneles";
        string WorldFlag => puzzleId + ":mundo";

        public int CurrentStep
        {
            get { int i = 0; while (Def != null && i < Def.pasos.Length && S.Flag(StepFlag(i))) i++; return i; }
        }
        public bool PanelsDone => S != null && S.Flag(PanelsFlag);
        public override bool Done => S != null && S.Solved(puzzleId);

        void Start()
        {
            if (S == null) return;
            if (Done) ApplySolvedVisuals();
            else if (PanelsDone) AfterPanels(false);
        }

        public override string Objective
        {
            get
            {
                if (PanelsDone && requiereMundo && !string.IsNullOrEmpty(objetivoMundo)) return objetivoMundo;
                return objetivo;
            }
        }

        // ------------------------------------------------------------ abrir
        public void Open()
        {
            if (Def == null) { UIManager.I.Toast("Falta el puzzle " + puzzleId + " en puzzles.json"); return; }
            if (Done) { UIManager.I.Toast($"«{Def.titulo}» ya está resuelto."); return; }
            if (PanelsDone) { UIManager.I.Toast(Objective); return; }
            if (requierePuzzle != null && !requierePuzzle.Done) { UIManager.I.Toast(mensajeBloqueado, 4f); return; }
            if (flagsRequeridos != null)
                foreach (var f in flagsRequeridos)
                    if (!S.Flag(f)) { UIManager.I.Toast(mensajeBloqueado, 4f); return; }
            if (openedAt < 0) openedAt = Time.time;
            if (mundoDoradoAlAbrir && WorldManager.I != null && !WorldManager.I.Dorado) WorldManager.I.RingBell(true, 3f);
            string said = puzzleId + ":dicho";
            if (!string.IsNullOrEmpty(dialogoAlAbrir) && !S.Flag(said))
            {
                S.SetFlag(said);
                UIManager.I.PlayDialogue(dialogoAlAbrir, () => Game.ShowFichas(Def.fichas, ShowPlaca));
                return;
            }
            Game.ShowFichas(Def.fichas, ShowPlaca);
        }

        void ShowPlaca()
        {
            if (tuVerso && CurrentStep == 0)
            {
                UIManager.I.ShowChoice(Def.titulo,
                    "Escribe una estrofa de cuatro versos sobre cómo percibes la vida (actividad 7, p. 85). Aparecerá en los créditos.",
                    new[] { "Armar un cuarteto con rima abba (banco de versos)", "Escribir mis propios cuatro versos" },
                    i => { if (i == 0) ShowStep(); else TuVersoWrite.Show(this); });
                return;
            }
            bool hasPoems = poemasPlaca != null && poemasPlaca.Length > 0;
            if (CurrentStep > 0 || (string.IsNullOrEmpty(placa) && !hasPoems)) { ShowStep(); return; }
            var sb = new System.Text.StringBuilder(placa ?? "");
            if (hasPoems)
                foreach (var id in poemasPlaca)
                {
                    var p = Game.DB.Poema(id);
                    if (p == null) continue;
                    S.AddUnique(S.poemas, id);
                    sb.Append("\n\n").Append(ContentDatabase.PoemaTexto(p));
                }
            UIManager.I.ShowText(Def.titulo, sb.ToString().Trim(), "Resolver", ShowStep);
        }

        void ShowStep()
        {
            int i = CurrentStep;
            if (i >= Def.pasos.Length) { FinishPanels(); return; }
            var paso = Def.pasos[i];
            if (paso.tipo == "pregunta")
            {
                var opts = new List<string> { paso.correcta };
                opts.AddRange(paso.distractores);
                opts = Assessment.AssessmentRunner.Shuffle(opts, new System.Random(puzzleId.GetHashCode() + i + Log.intentos));
                string ctx = string.IsNullOrEmpty(poemaContexto) ? null : ContentDatabase.PoemaTexto(Game.DB.Poema(poemaContexto));
                UIManager.I.ShowQuestion($"{Def.titulo} · {paso.titulo}", paso.enunciado, opts, ctx, a =>
                {
                    if (Check(i, new List<string> { a }, out var fb)) Next(i);
                    else { UIManager.I.Toast("No es esa. " + (fb ?? "Puedes pedir una pista en Pensar (celular)."), 4f); ShowStep(); }
                }, "Pistas: celular → Pensar.");
            }
            else AssignmentPanel.Show(this, paso, i);
        }

        /// Valida un intento del paso i y lo registra.
        public bool Check(int i, IList<string> answers, out string feedback)
        {
            var paso = Def.pasos[i];
            bool ok = PuzzleValidation.Validate(paso, answers, out feedback);
            var l = Log;
            l.intentos++;
            l.registro.Add(new Intento { paso = paso.id, correcto = ok, t = Time.time - Mathf.Max(0, openedAt) });
            if (paso.tipo == "abba" && ok) { S.verso.Clear(); S.verso.AddRange(answers); S.versoModo = "armado"; }
            if (!ok && paso.tipo == "juicio") feedback = JuicioPenalty(feedback);
            return ok;
        }

        int juicioErrors;

        /// Juicio: un error cuesta Tiempo y El Retrato contraataca; cada 2 errores llega una pista.
        /// En cero, Ale despierta junto a la última vela (sin game over, nunca bloquea).
        string JuicioPenalty(string feedback)
        {
            juicioErrors++;
            S.tiempo = Mathf.Max(0, S.tiempo - 10);
            var contra = Game.DB.Dialogo("juicio_contra");
            if (contra != null && contra.lineas.Length > 0)
                feedback = $"<color=#EBC261>El Retrato:</color> «{contra.lineas[juicioErrors % contra.lineas.Length].texto}»\n{feedback}";
            if (juicioErrors % 2 == 0)
            {
                var h = RequestHint();
                if (h != null) feedback += "\n<color=#A09A90>Pista: " + h + "</color>";
            }
            if (S.tiempo <= 0)
            {
                S.tiempo = 100;
                UIManager.I.Toast("La arena del reloj se acabó… Ale despierta junto a la última vela roja, en el mismo escenario. Sigue.", 6f);
            }
            return feedback;
        }

        public void Next(int i)
        {
            S.SetFlag(StepFlag(i));
            hintsThisStep = 0;
            if (i + 1 < Def.pasos.Length) ShowStep();
            else FinishPanels();
        }

        void FinishPanels()
        {
            S.SetFlag(PanelsFlag);
            if (requiereMundo && !S.Flag(WorldFlag)) AfterPanels(true);
            else Solve(false);
        }

        void AfterPanels(bool fresh)
        {
            foreach (var go in ocultarAlTerminarPaneles ?? new GameObject[0]) if (go) go.SetActive(false);
            if (campana) campana.Activate(true);
            if (fresh && !string.IsNullOrEmpty(dialogoPaneles)) UIManager.I.PlayDialogue(dialogoPaneles);
        }

        /// La parte en el mundo (cortar la rosa) terminó.
        public void CompleteWorld()
        {
            S.SetFlag(WorldFlag);
            if (PanelsDone) Solve(false);
        }

        void Solve(bool guided)
        {
            var l = Log;
            l.resuelto = true;
            l.conAyuda |= guided;
            if (openedAt >= 0) l.segundos += Time.time - openedAt;
            openedAt = -1;
            if (campana) campana.Activate(false);
            foreach (var p in poemasAlResolver ?? new string[0]) S.AddUnique(S.poemas, p);
            ApplySolvedVisuals();
            if (!string.IsNullOrEmpty(dialogoResuelto)) UIManager.I.PlayDialogue(dialogoResuelto, () => OnSolved?.Invoke());
            else { UIManager.I.Toast($"Resuelto: {Def.titulo}", 3f); OnSolved?.Invoke(); }
        }

        /// Tu verso escrito libremente: se guarda para la profesora, sin validación.
        public void CompleteWritten(string[] lines)
        {
            S.verso.Clear();
            S.verso.AddRange(lines);
            S.versoModo = "escrito";
            for (int i = 0; i < Def.pasos.Length; i++) S.SetFlag(StepFlag(i));
            S.SetFlag(PanelsFlag);
            Solve(false);
        }

        void ApplySolvedVisuals()
        {
            foreach (var go in ocultarAlTerminarPaneles ?? new GameObject[0]) if (go) go.SetActive(false);
            foreach (var go in ocultarAlResolver ?? new GameObject[0]) if (go) go.SetActive(false);
            foreach (var go in mostrarAlResolver ?? new GameObject[0]) if (go) go.SetActive(true);
        }

        // ------------------------------------------------------------ IPuzzle / guiado
        public bool Validate(IList<string> answers, out string feedback)
        {
            int i = Mathf.Min(CurrentStep, Def.pasos.Length - 1);
            return PuzzleValidation.Validate(Def.pasos[i], answers, out feedback);
        }

        public string GetHint(int level) => Hint(level);

        /// Modo guiado: Don Ruperto muestra el camino; queda registrado como "resuelto con ayuda".
        public void SolveGuided()
        {
            if (Def == null || Done) return;
            UIManager.I.CloseAllModals();
            if (openedAt < 0) openedAt = Time.time;
            foreach (var f in Def.fichas ?? new string[0]) S.AddUnique(S.fichas, f);
            foreach (var p in poemasPlaca ?? new string[0]) S.AddUnique(S.poemas, p);
            foreach (var o in objetosGuiado ?? new string[0]) S.AddUnique(S.inventario, o);
            for (int i = 0; i < Def.pasos.Length; i++)
            {
                if (Def.pasos[i].tipo == "abba" && S.verso.Count == 0) { S.verso.AddRange(PuzzleValidation.Solution(Def.pasos[i])); S.versoModo = "guiado"; }
                S.SetFlag(StepFlag(i));
            }
            S.SetFlag(PanelsFlag);
            S.SetFlag(WorldFlag);
            Solve(true);
        }

        public override void ForceComplete() => SolveGuided();

        // ------------------------------------------------------------ pistas
        public string HintTitle => Def != null ? Def.titulo : puzzleId;
        public int HintsShown => hintsThisStep;

        string[] StepHints
        {
            get
            {
                if (Def == null) return new string[0];
                int i = CurrentStep;
                if (i < Def.pasos.Length && Def.pasos[i].pistas != null && Def.pasos[i].pistas.Length > 0) return Def.pasos[i].pistas;
                return Def.pistas ?? new string[0];
            }
        }

        public string Hint(int level)
        {
            var h = StepHints;
            return level >= 1 && level <= h.Length ? h[level - 1] : "";
        }

        public string RequestHint()
        {
            if (hintsThisStep >= Mathf.Min(3, StepHints.Length)) return null;
            hintsThisStep++;
            Log.pistas++;
            return Hint(hintsThisStep);
        }
    }
}
