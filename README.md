# Engaño Colorido

Serious game en primera persona para **2° medio** (Lengua y Literatura): una sesión de un bloque de clase sobre el Siglo de Oro — hablante lírico, connotación, símbolos barrocos, soneto, tópicos, *La vida es sueño* y argumentación — con horror psicológico suave. Unity 6 (URP), greybox con atmósfera.

## Descargar y jugar

Cada fase del proyecto tiene su build de Windows en **[Releases](https://github.com/KIROCULTURE/engano-colorido/releases)**. Para presentar, usa la última:

| Fase | Qué se ve | Duración | Descarga |
|---|---|---|---|
| **M6 · Final (Entrega 1)** | **Modo 45 completo**: diagnóstico → Camarín → Patio → Radios → Galería → Biblioteca → Teatro (Juicio, Tu verso, finales) → prueba de salida → CSV | ~35–40 min | [hito-M6](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M6) |
| M5 · Zonas | Todo lo anterior menos el Teatro | 25–35 min | [hito-M5](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M5) |
| M4 · Mundo | Sesión con Camarín + Mundo de Niebla / Mundo Dorado | 6–8 min | [hito-M4](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M4) |
| M3 · Datos | Sesión con Camarín + revisión de contenidos (menú docente) | 6–8 min | [hito-M3](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M3) |
| M2 · Sesión | Menú docente, diagnóstico, prueba de salida y export | 6–8 min | [hito-M2](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M2) |
| M1 · Núcleo | Recorrido libre del Camarín (controles y celular) | 3–5 min | [hito-M1](https://github.com/KIROCULTURE/engano-colorido/releases/tag/hito-M1) |

1. En la página del release, en **Assets**, descarga `EnganoColorido_<fase>.zip`.
2. Descomprime en una **ruta corta** (Escritorio o pendrive) y abre `EnganoColorido.exe`. Windows 10/11, funciona sin internet.
3. Menú → **Nueva sesión** → alias. Menú docente: PIN `1234`. Los resultados (CSV + JSON) quedan en `Documentos\EnganoColorido`.

Controles: WASD moverse · mouse mirar · E interactuar · F linterna · C agacharse · Shift correr · Tab celular (pistas en *Pensar*) · Esc pausa.

## Proyecto

- Unity **6000.3.11f1**. Abrir con Unity Hub y usar *Tools → Engaño Colorido → Setup Project / Import Content / Build Greybox Scenes*.
- Contenido editable sin programar: `Assets/_Project/Data/*.json`.
- Detalle de cada fase, cómo generar escenas y builds, y lo pendiente: [AVANCE.md](AVANCE.md). Diseño: [GDD_Engano_Colorido.md](GDD_Engano_Colorido.md).
