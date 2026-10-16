# AVANCE — Engaño Colorido (Entrega 1)

## M0 Setup ✅
- Proyecto Unity 6000.3.11f1 + URP 17.3, Input System 1.19, TextMeshPro (ugui 2.0), AI Navigation 2.0.
- Estructura `Assets/_Project/...`, asmdefs (runtime, editor, tests EditMode).
- Menús `Tools/Engaño Colorido/` → Setup Project, Import Content, Build Greybox Scenes, Build Windows.
- `BuildScript.BuildWindows` (batch, `-hito <carpeta>`), escena de arranque generada por código.
- Build de humo de Windows OK (no se guarda; las builds por hito parten en M1).

## M1 Núcleo ✅ — `Builds/M1_Nucleo/`
- `PlayerController`: caminar/correr/agacharse, mirar con mouse, linterna (F), interactuar (E, raycast), celular (Tab), pausa (Esc).
- Interactuables: `Inspectable` (texto/ficha/poema → Cuaderno), `Pickup` (inventario), `Door` (salida de zona), `FichaBoard` (fichas de la profe Inés).
- Celular (`PhoneUI`): Cuaderno (fichas y versos), Mapa (√ / ? / X), Inventario, Stories, Pensar (pistas en 3 niveles), Ajustes (sensibilidad, FOV, tamaño de texto, volumen).
- Autoguardado al terminar cada zona (`SaveSystem`, JSON local en `persistentDataPath`).
- Escena Camarín generada por `Build Greybox Scenes`.
- Probar: abrir la build → "Recorrido libre: Camarín".

## M2 Sesión y evaluación ✅ — `Builds/M2_Sesion/`
- Menú docente con PIN (`session.json` → `pinDocente`, por defecto 1234): curso, modo (90 bloqueado "Próximamente"), dificultad de acertijos, carpeta de export, resumen del curso por concepto.
- Inicio con alias (sin nombres completos), diagnóstico D1–D5 y prueba de salida S1–S8 con alternativas barajadas; S6–S8 muestran el soneto de Quevedo (solo ahí).
- `SessionManager` + `SessionPacer`: reloj de sesión, presupuestos por zona (GDD 4.1), pistas adelantadas al 100 %, Don Ruperto ofrece modo guiado al 130 %, salto de zonas opcionales, envío forzado al Teatro (min 29 / 47) y a la prueba de salida (min 37 / 56).
- Export CSV (UTF-8 con BOM, `;`, apto para Excel en Chile, sin fórmulas) + JSON con todo el registro.
- Tests EditMode (12): ritmo, puntaje y ganancia, CSV/JSON, guardado/carga, barajado.
- Verificación automática: `EnganoColorido.exe -autoplay -exportdir <carpeta>` juega la sesión y exporta.

## M3 Datos ✅ — `Builds/M3_Datos/`
- `Assets/_Project/Data/`: `fichas.json` (16 fichas ≤ 80 palabras, con página del libro), `poems.json` (corpus 2.6 textual, sin Quevedo), `topicos.json`, `puzzles.json` (P1, H, P3, P4, P5, P7, Juicio, Tu verso: solución, distractores, 3 pistas, conceptTags), `assessment.json` (D1–D5, S1–S8 y el soneto de Quevedo solo aquí), `dialogue.json` (líneas lineales), `session.json` (PIN, zonas, presupuestos por modo).
- `Tools/Engaño Colorido/Import Content` → `Resources/ContentDatabase.asset` + `ContentValidator` (avisa si una corrección rompe un puzzle).
- `PuzzleValidation` (puro): asignación, estante por repisa, preguntas, abba de Tu verso, Juicio con feedback por categoría de conector.
- Tests EditMode (28): + contenido (Quevedo exclusivo de la salida, fichas ≤ 80 palabras, 3 pistas, Clotaldo), soluciones válidas y cada distractor rechazado en cada puzzle, abba, Juicio.
- Menú docente: "Revisar contenidos".
