# AVANCE — Engaño Colorido (Entrega 1)

**Estado:** Entrega 1 completa. El modo 45 se juega de punta a punta en greybox: diagnóstico → Camarín → Patio (P1) → Radios (H) → Galería (P3, P4 y el esqueleto) → Biblioteca (P5) → Teatro (P7, Juicio, Tu verso, final) → prueba de salida → CSV/JSON. Hitos M0–M6 con commit y etiqueta `hito-M0` … `hito-M6`. 28/28 tests EditMode en verde.

## 1. Tabla de builds

Todas en `Builds/<hito>/EnganoColorido.exe`, cada una con su `LEEME.txt`. **Antes de abrir una build, copia su carpeta a una ruta corta** (Escritorio, pendrive). Desde la carpeta de esta sesión Windows usa una ruta virtualizada muy larga y el ejecutable no carga.

| Hito | Qué se ve al jugarla | Duración aprox. | ¿Sirve para presentar? |
|---|---|---|---|
| `M1_Nucleo` | Menú y recorrido libre del Camarín: moverse, linterna, interactuar, celular (Cuaderno, Mapa, Inventario, Pensar, Ajustes), fichas del espejo, autoguardado | 3–5 min | Solo como demo de controles: no muestra el contenido curricular |
| `M2_Sesion` | Menú docente con PIN, alias, diagnóstico, Camarín, prueba de salida y export CSV/JSON (las zonas que faltan se omiten solas) | 6–8 min | Parcial: muestra el ciclo de evaluación completo |
| `M3_Datos` | Lo de M2 + todo el contenido en JSON y "Revisar contenidos" en el menú docente | 6–8 min | Parcial: bueno para revisar textos con la docente |
| `M4_Mundo` | Lo de M3 + Mundo de Niebla y Mundo Dorado con la campana, y estática del celular | 6–8 min | Parcial: muestra la atmósfera |
| `M5_Zonas` | Sesión con Patio (P1), Radios (H), Galería (P3, P4, esqueleto) y Biblioteca (P5). Sin Teatro: al salir de la Biblioteca va directo a la prueba de salida | 25–35 min | Sí, pero sin el cierre (Juicio y finales) |
| **`M6_Final`** | **Modo 45 completo:** lo de M5 + Teatro (P7), Juicio del Espejo, Tu verso, finales Despertar / Dulce soñar, prueba de salida y export | ~35–40 min (estimado; el juego fuerza el Teatro en el min 29 y la prueba en el 37) | **Sí: es la recomendada** |

La duración real con estudiantes no está medida (ver pendientes). El ritmo sí se verificó en la build con reloj acelerado: modo guiado ofrecido, Teatro en el minuto 29,0, prueba de salida en el 37,0 y CSV exportado.

## 2. Cómo abrir el proyecto, generar las escenas y jugar el modo 45

**Requisitos:** Unity **6000.3.11f1** con el módulo *Windows Build Support (Mono)*, que viene por defecto.

1. **Ubica el proyecto en una ruta corta**, por ejemplo `C:\Proyectos\EnganoColorido`. Unity no lee algunos archivos de paquetes de URP cuando la ruta completa pasa de 260 caracteres. Durante el desarrollo usé un *junction* (`C:\Users\kiroi\AppData\Local\Temp\ec-engano`) que apunta a la carpeta de esta sesión.
2. Abre Unity Hub → *Add project from disk* → elige la carpeta → ábrelo con 6000.3.11f1. La primera apertura descarga paquetes y tarda unos minutos.
3. Usa el menú **Tools → Engaño Colorido**, en este orden:
   - **Setup Project**: solo la primera vez. Importa los recursos de TextMeshPro y ajusta Player Settings.
   - **Import Content**: convierte `Assets/_Project/Data/*.json` en `Resources/ContentDatabase.asset` y avisa en la consola si una corrección rompe un puzzle.
   - **Build Greybox Scenes**: genera Arranque, Camarin, Patio, Galeria, Biblioteca y Teatro, y las deja en Build Settings. Las escenas se regeneran enteras: si hay que cambiar algo de una escena, se edita el generador (`Assets/_Project/Editor/Greybox*.cs`), no la escena.
4. **Para jugar:** abre `Assets/_Project/Scenes/Arranque.unity` → Play → **Nueva sesión** → escribe un alias → diagnóstico → zonas.
   - El modo, el curso, la dificultad y la carpeta de export se cambian en **Menú docente** (PIN `1234`, editable en `session.json`).
   - Por defecto el CSV y el JSON se guardan en `Documentos\EnganoColorido`.
5. **Controles:** WASD moverse · mouse mirar · E interactuar · F linterna · C agacharse · Shift correr · Tab celular (las pistas están en *Pensar*) · Esc pausa (*Guardar y volver al menú*; "Continuar" retoma la partida).
6. **Tests:** Window → General → Test Runner → EditMode → Run All (28 tests).
7. **Build de Windows:** Tools → Engaño Colorido → Build Windows (deja la build en `Builds/Dev/`). También por línea de comandos:
   ```
   Unity.exe -batchmode -quit -projectPath <ruta> -executeMethod BuildScript.BuildWindows -hito M6_Final -logFile build.log
   ```
   Todo en batch, sin abrir el editor: `-executeMethod EnganoColorido.EditorTools.EnganoSetup.BatchAll`.
8. **Para regenerar una build antigua:** `git checkout hito-M3`, luego Build Greybox Scenes y Build Windows.
9. **Verificación automática (solo desarrollo):**
   - `EnganoColorido.exe -autoplay -exportdir <carpeta>` juega toda la sesión con modo guiado y exporta.
   - Agrega `-devidle -devclock 40` para probar el ritmo con reloj acelerado, o `-devfinal sonar` para el otro final.
   - `-devscene Patio -devshot captura.png` saca una captura de una escena.

**Para corregir contenido sin tocar C#:** fichas, poemas, pistas, preguntas, diálogos, PIN y presupuestos están en `Assets/_Project/Data/*.json`. Después de editar, usa Import Content, corre los tests y haz la build.

## 3. Pendiente para la Entrega 2

- **Modo 90:** Capilla (P2, órgano del estribillo, Santa Teresa) y Torre del reloj (P6, las tres horas). Sus puertas en el Patio están cerradas con "solo en el modo 90" y el modo 90 aparece bloqueado en el menú docente. `SessionPacer` ya soporta el modo 90, incluido el salto de Capilla y Torre si la sesión va atrasada (está testeado). La ficha "Poesía mística" ya está escrita.
- **Las Horas:** patrulla con NavMesh, robo de Tiempo, perder el rastro al agacharse, y el modo Calma/Suave ("Presencias"). AI Navigation está instalado pero el generador todavía no hornea NavMesh. `StaticSource` ya está listo para que las Horas hagan sonar la estática del celular.
- **Tiempo (reloj de arena):** hoy solo lo consumen los errores del Juicio. Al llegar a cero, Ale despierta y sigue, sin game over. Las velas rojas son decorativas: el guardado es automático por zona y también manual desde la pausa.
- **Audio:** ambiente (zumbido, viento) y TTS de los poemas. Hoy solo hay campana, estática y un tono grave, todos procedurales. Los fragmentos de las radios van subtitulados, sin voz.
- **Accesibilidad completa:** ya están los subtítulos siempre activos, el tamaño de texto, la sensibilidad, el FOV y el juego no tiene destellos. Faltan TTS, opción explícita "sin sustos" (hoy no hay sustos bruscos) y el modo Calma.
- **Pulido visual** de los dos mundos y de las recreaciones de Pereda y Murillo (hoy son primitivas).
- **Jugar con mouse y teclado reales (sin verificar):** todas las pruebas de punta a punta usaron autoplay o modo guiado y capturas automáticas. Hay que jugar M6 a mano al menos una vez: escribir el alias, hacer clic en los paneles, cortar la rosa dentro de los 4 s, mover el dial de las radios y elegir el final.
- **Prueba cronometrada con estudiantes** en cada modo. La duración de ~40 min es una estimación: el ritmo está verificado, pero no se midió jugando de verdad.
- **Confirmar con la docente:**
  - la actitud *enunciativa* de la radio 3 (Calderón);
  - la respuesta de S7 (Quevedo, *carmínica*), que el libro deja abierta;
  - las páginas citadas, que son de la edición 2017. Si el curso usa la 2020, se cambian en los JSON.
- **Detalles menores:** la fuente de TMP no tiene ✓ ni emojis, así que el Mapa usa √ / ? / X y las stories no llevan emojis. Al escribir alias y curso se les quitan `;` y comillas para que el resumen del curso pueda leer los CSV.

---

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

## M4 Mundo básico ✅ — `Builds/M4_Mundo/`
- Niebla: fog exponencial² por escena + volumen `PP_Niebla` (grano, viñeta, desaturación, ACES).
- `WorldManager`: Niebla ↔ Dorado con la campana (`RingBell`): interpola color de materiales (`WorldVariant`, pan de oro), luces, niebla, luz ambiente y peso de los volúmenes `PP_Niebla`/`PP_Dorado`; `WorldOnly` para decorado exclusivo del Dorado (inscripciones latinas, velas, flores marchitas). Nada más (sin shaders propios).
- `BellCycle` (campana cada 20 s, para P1-B), `ProceduralAudio` (estática, campana y tono generados con `AudioClip.Create`), `PhoneStatic` + `StaticSource` (estática del celular cerca del espejo, el velo y las radios; en la Entrega 2 se suman Las Horas).
- Camarín: la intro ocurre en el Mundo Dorado y la campana devuelve a la Niebla al leer las notas.
- Las Horas no van en la Entrega 1.

## M5 Zonas del modo 45 ✅ — `Builds/M5_Zonas/`
- `PuzzleController` (datos de `puzzles.json`): ficha previa → placa (poemas al Cuaderno) → pasos (`AssignmentPanel` / preguntas) → parte en el mundo opcional → resuelto. Implementa `IPuzzle` (`Validate`, `GetHint`, `OnSolved`, `SolveGuided`) e `IHintable`; registra intentos, pistas, tiempo, con ayuda y `conceptTags`.
- `AssignmentPanel`: clic en hueco → clic en opción; orden de opciones barajado y estable; estante con rótulos/colores según dificultad (Fácil: colores por rima + letra; Normal: letra; Difícil: nada); validación por repisa.
- **Patio** (P1): candado de 4 diales + `RoseBush` (botón en Niebla, marchita en Dorado, florece 4 s en cada cambio; campana cada 20 s). **Radios** (H): `RadioTuner` con dial entre estática (`PhoneStatic`) → fragmento subtitulado → tablero de etiquetas. Puertas de Capilla y Torre cerradas ("modo 90").
- **Galería** (P3, P4, velo): objetos en Laboratorio, Sala de profesores y Bodega de utilería (+ distractores), recreaciones de Pereda y Murillo, retratos que se cuelgan al resolver, `VeiledFigure` (velo → esqueleto, cambio de luz, tono grave, ficha de *El mágico prodigioso*).
- **Biblioteca** (P5): estante 4/4/3/3, funciones de estrofa, preguntas de "dulce" y tópico; el soneto de Boscán entra al Cuaderno recién al resolver.
- Sub-zonas en una escena (Patio → Radios) con presupuestos separados.
- Verificado: `-autoplay` recorre Camarín → Patio/Radios → Galería → Biblioteca → prueba de salida → CSV sin excepciones.

## M6 Final ✅ — `Builds/M6_Final/`
- **Teatro**: P7 (libreto con acotación, aparte, diálogo y monólogo; máscaras de personajes tipo; pregunta del conflicto). El libreto deja en el Cuaderno el fragmento de *La vida es sueño*, que trae el verso de Clotaldo para el Juicio.
- **Juicio del Espejo** (en el Mundo Dorado): 3 rondas de conector + verso del Cuaderno + conector, con la frase armándose en vivo. Cada error cuesta Tiempo y El Retrato contraataca; cada 2 errores llega una pista. El feedback nombra la categoría ("introduce una consecuencia, pero aquí se necesita un conector que opone ideas").
- **Tu verso**: armar un cuarteto abba desde el banco (validado) o escribir cuatro versos propios (se guardan sin validar). La estrofa aparece en la pantalla final.
- **Finales**: *Despertar* (amanecer, monólogo, decisión de publicar o no la foto sin filtro) y *Dulce soñar* (Mundo Dorado, retrato nuevo, versos de Boscán), sin muerte. Ambos llevan a la prueba de salida y la elección queda en el CSV.
- `TeatroDirector` encadena Juicio → Tu verso → final. El modo guiado nunca elige el final por la persona.
- Verificado en la build: autoplay completo con cada final, ritmo con reloj acelerado (Teatro en el min 29, salida en el 37) y capturas de escenas y paneles.
