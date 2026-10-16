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
