# CLAUDE.md — Engaño Colorido

Serious game FPS para 2° medio: una experiencia educativa de **un bloque de clase** sobre la unidad del Siglo de Oro del texto escolar de Lengua y Literatura (Mineduc). Horror psicológico suave con puzzles inspirados en Silent Hill.
**Lee `GDD_Engano_Colorido.md` antes de cualquier tarea.** Es la fuente de verdad de contenido y diseño.

## Prioridades (en caso de conflicto, gana la de arriba)
1. Que se aprenda el contenido de la unidad (GDD secciones 1, 2 y 4).
2. Que la sesión calce en el bloque: modo 90 (~60' de juego) y modo 45 (~40'), con control de ritmo (GDD 4.3).
3. Narrativa y atmósfera.

## Alcance: serious game de bajo presupuesto
Esto **no es un juego comercial**. Es un serious game de estudiantes que tiene que enseñar la unidad y caber en un bloque de clase. El GDD describe la visión completa: implementa siempre **la versión más simple que enseñe lo mismo**.
- Regla de oro: entre "más bonito" y "más simple", elige más simple. Entre "más simple" y "enseña mejor", elige enseña mejor.
- Todo con primitivas de Unity (cubos, planos, cilindros) y materiales de color. Sin modelos, sin animaciones, sin rigs. Don Ruperto y El Retrato son siluetas o planos con texto; el esqueleto puede ser un modelo de primitivas.
- Sin cinemáticas: las escenas se cuentan con texto en pantalla, cambios de luz y un sonido.
- Sin shaders propios ni VFX Graph: URP por defecto, niebla de `RenderSettings` y un solo volumen de postproceso por mundo.
- Sin sistemas genéricos (crafting, árboles de diálogo, misiones, localización, logros): solo lo que piden los puzzles. Los diálogos son listas lineales de líneas.
- UI con uGUI simple y una fuente. El mapa del celular puede ser una lista de zonas con su estado (✓ / ? / X).
- No agregues nada que no esté en el GDD. Ningún paquete externo fuera del stack.
- Si algo del GDD sale caro, haz la versión mínima y anótalo en `AVANCE.md`.

## Reglas de contenido (no negociables)
- Todo el texto del juego en español de Chile. Ale tiene género neutro: nunca adjetivos con género para Ale.
- El contenido académico sale del libro (GDD sección 2). Definiciones parafraseadas, nunca copiadas del libro.
- Solo se citan textualmente los textos de dominio público de GDD 2.6. Lo de 2.8 no se reproduce.
- El poema de Quevedo (2.6) es exclusivo de la prueba de salida: no puede aparecer antes en el juego.
- Silent Hill = patrones de diseño (GDD 5). Cero nombres, personajes, criaturas, música, sonidos, logos o assets de la saga.
- Público menor de edad: sin sangre, gore, armas ni combate; la muerte es tema literario, nunca algo deseable; ningún final mata a Ale.
- Datos de estudiantes: solo alias, solo en local, sin red (GDD 8).

## Stack
- Unity 6 LTS (o 2022.3 LTS si el proyecto ya existe), URP, Input System nuevo, TextMeshPro, AI Navigation.
- Sin assets de pago. Audio y texturas propios o CC0. Estática procedural (`AudioClip.Create`).
- Target: Windows, offline, 60 fps a 1080p en gráfica integrada.

## Cómo trabajar sin abrir el editor
- Todo lo que normalmente se arma a mano se genera con menús de Editor:
  - `Tools/Engaño Colorido/Build Greybox Scenes` → escenas (camarín, patio hub, capilla, galería, biblioteca, torre, teatro) con primitivas, colliders, luces, niebla, NavMesh y spawn points.
  - `Tools/Engaño Colorido/Import Content` → convierte `Assets/_Project/Data/*.json` en ScriptableObjects.
- El juego debe poder jugarse de punta a punta en greybox.

## Estructura
```
Assets/_Project/
  Scripts/{Core,Player,Interaction,Puzzles,World,AI,UI,Save,Session,Assessment,Data}
  Editor/
  Data/{poems,fichas,topicos,puzzles,assessment,dialogue,session}.json
  Scenes/ Prefabs/ Materials/ Audio/
docs/   (opcional: PDF del texto escolar; la unidad está en pp. 64–119)
```

## Código
- Código mínimo y limpio, sin sobre-ingeniería.
- Contenido fuera del código: fichas, poemas, preguntas, pistas, soluciones, presupuestos de tiempo y banco de evaluación en JSON. Una docente debe poder corregir una pista o una pregunta sin tocar C#.
- Puzzles comparten una interfaz simple (`Validate`, `GetHint(level)`, `OnSolved`, `SolveGuided`) y registran cada intento con `conceptTags`.
- `SessionManager`: modo 45/90, reloj de sesión, presupuestos por zona, salto de zonas opcionales y envío forzado al Teatro (GDD 4.3).
- Tests EditMode: validadores de cada puzzle, Juicio (conector + evidencia), esquema abba de Tu verso, puntaje y ganancia pre/post, export CSV/JSON, guardado/carga, lógica de ritmo.

## Plan de entregas
El proyecto se entrega en dos fases. **La tarea actual es la Entrega 1 (avance, penúltima fase).** La Entrega 2 se hace la semana siguiente.

Trabaja de corrido hasta completar la Entrega 1: al cerrar cada hito deja el proyecto compilando, haz commit y anota en `AVANCE.md` qué quedó y cómo probarlo. No esperes aprobación entre hitos, salvo que algo bloquee o haya que decidir algo de diseño que el GDD no resuelve.

### Build por hito (para elegir qué fase presentar)
Desde M1 en adelante, al cerrar **cada** hito genera una build de Windows jugable y guárdala aparte:
- Carpeta `Builds/<número>_<nombre>/` (ej. `Builds/M1_Nucleo/`) con el `.exe` y un `LEEME.txt` de 5–10 líneas: qué incluye, cómo se juega, qué falta y cuánto dura.
- Build por línea de comandos con `BuildScript.BuildWindows` en modo batch (`Unity.exe -batchmode -quit -projectPath . -executeMethod BuildScript.BuildWindows -logFile build.log`), y también desde el menú `Tools/Engaño Colorido/Build Windows`. Si no encuentras la ruta de Unity en el equipo, pregúntala una vez.
- Cada build debe **partir en un estado presentable**: menú principal con lo que existe; lo que aún no está hecho aparece bloqueado con "Próximamente", nunca roto ni con errores.
- Etiqueta git por hito (`hito-M1`, `hito-M2`, …) para poder regenerar cualquier build después.
- `Builds/` va en `.gitignore`, excepto los `LEEME.txt`.
- Al final, `AVANCE.md` incluye una tabla de builds: hito, qué se ve al jugarla, duración aproximada y si sirve para presentar.

### Entrega 1 — Avance (penúltima fase): el **modo 45 completo y jugable** de inicio a fin
1. **M0 Setup:** proyecto, paquetes, carpetas, menús de Editor, escena de arranque.
2. **M1 Núcleo:** controlador FPS, linterna, interactuar/inspeccionar, celular (Cuaderno, Mapa, Inventario, Pensar, Ajustes básicos), autoguardado por zona.
3. **M2 Sesión y evaluación:** menú docente (alias, curso, modo, PIN), diagnóstico, prueba de salida, `SessionManager` con presupuestos y envío forzado al Teatro, export CSV/JSON.
4. **M3 Datos:** JSON desde el GDD + importador (fichas, poemas, puzzles, evaluación, tiempos).
5. **M4 Mundo básico:** niebla + postproceso, cambio Niebla/Dorado con campana (cambio de materiales, luces y postproceso; nada más), estática del celular. **Las Horas no van en la Entrega 1.**
6. **M5 Zonas del modo 45:** Prólogo, Patio (P1), Radios (H), Galería (P3, P4 con la escena del esqueleto), Biblioteca (P5).
7. **M6 Final:** Teatro (P7), Juicio, Tu verso, los dos finales, prueba de salida y export.

**Criterio de la Entrega 1:** una persona puede jugar el modo 45 completo en greybox, en ~40 minutos, del diagnóstico al CSV exportado, sin errores que bloqueen. Todos los puzzles del modo 45 validan bien y tienen ficha y pistas. El arte puede ser greybox con atmósfera (niebla, luz, color por mundo).

**Si el tiempo no alcanza, prioriza en este orden:** (1) el loop completo diagnóstico → zonas → Juicio → prueba de salida → CSV; (2) P1, P3/P4 y P5; (3) Radios y P7; (4) Las Horas y el cambio de mundo. Lo que quede afuera se anota en `AVANCE.md` como pendiente para la Entrega 2.

### Entrega 2 — Fase final (semana siguiente)
- Zonas del modo 90: Capilla (P2) y Torre (P6), y el modo 90 completo.
- Las Horas con IA completa y modo Calma.
- Audio (ambiente, campana, TTS de poemas), accesibilidad completa, pulido visual de los dos mundos.
- Prueba cronometrada de una sesión completa en cada modo y build de Windows.

## Criterios de "listo" por puzzle
- Se resuelve con la solución del GDD y rechaza los distractores.
- Tiene ficha previa, 3 pistas y modo guiado al pasar su presupuesto de tiempo.
- Registra intentos, pistas, tiempo y si se resolvió con ayuda.
