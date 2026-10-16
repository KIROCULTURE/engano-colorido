using EnganoColorido.Assessment;
using EnganoColorido.Core;
using EnganoColorido.Save;
using TMPro;

namespace EnganoColorido.UI
{
    /// Pantalla final: resultados de la sesión, ruta del export y créditos con la estrofa de Tu verso.
    public static class ResultsScreen
    {
        public static void Show(SaveData s, string exportPath, string error)
        {
            if (DevTools.AutoPlay) { DevTools.AutoFinish(exportPath ?? ("ERROR " + error)); return; }
            var sb = new System.Text.StringBuilder();
            sb.Append($"Alias: <b>{UIKit.Escape(s.alias)}</b> · Curso {UIKit.Escape(s.curso)} · Modo {s.modo}\n\n");
            sb.Append($"Diagnóstico: <b>{Scoring.Diagnostico(s)} de 5</b>\n");
            sb.Append($"Prueba de salida: <b>{Scoring.Salida15(s)} de 5</b> + transferencia <b>{Scoring.Transferencia(s)} de 3</b>\n");
            sb.Append($"Ganancia (S1–S5 − D1–D5): <b>{Scoring.Ganancia(s):+0;-0;0}</b>\n");
            if (!string.IsNullOrEmpty(s.final)) sb.Append($"Final elegido: <b>{(s.final == "despertar" ? "Despertar" : s.final == "sonar" ? "Dulce soñar" : s.final)}</b>\n");
            sb.Append($"Duración: {s.elapsed / 60f:0} min\n\n");
            sb.Append(error == null
                ? $"<color=#73C780>Resultados guardados (CSV + JSON) en:</color>\n<size=80%>{exportPath}</size>\n\n"
                : $"<color=#E07366>No se pudo exportar: {error}</color>\nRevisa la carpeta en el menú docente.\n\n");
            if (s.verso.Count > 0)
            {
                sb.Append("<b>Tu verso</b>\n<i>");
                foreach (var v in s.verso) sb.Append(UIKit.Escape(v)).Append('\n');
                sb.Append("</i>\n");
            }
            sb.Append("<size=80%><color=#A09A90>Engaño Colorido · serious game de Lengua y Literatura, 2° medio.\n" +
                      "Poemas de dominio público (siglos XVI–XVII). Contenidos parafraseados del Texto del Estudiante (Mineduc).\n" +
                      "Para la tarea: ¿necesitamos hoy un «desengaño del mundo»? (ensayo, p. 106).</color></size>");
            UIManager.I.ShowText("Fin de la sesión", sb.ToString(), "Volver al menú", () => GameFlow.I.ReturnToMenu(), 1100, 900, Tema.App, "Repasa+ · resultados");
        }
    }
}
