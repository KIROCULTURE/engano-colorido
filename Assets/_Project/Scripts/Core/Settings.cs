using System;
using System.IO;
using UnityEngine;

namespace EnganoColorido.Core
{
    /// Ajustes locales del equipo (PlayerPrefs). Nada sale del computador.
    public static class Settings
    {
        public static event Action Changed;

        public static float Sensibilidad { get => PlayerPrefs.GetFloat("ec.sens", 1f); set { PlayerPrefs.SetFloat("ec.sens", value); Changed?.Invoke(); } }
        public static float FOV { get => PlayerPrefs.GetFloat("ec.fov", 70f); set { PlayerPrefs.SetFloat("ec.fov", value); Changed?.Invoke(); } }
        public static float TamanoTexto { get => PlayerPrefs.GetFloat("ec.texto", 1f); set { PlayerPrefs.SetFloat("ec.texto", value); Changed?.Invoke(); } }
        public static float Volumen { get => PlayerPrefs.GetFloat("ec.vol", 0.8f); set { PlayerPrefs.SetFloat("ec.vol", value); AudioListener.volume = value; Changed?.Invoke(); } }
        /// 0 Fácil (por defecto), 1 Normal, 2 Difícil
        public static int Dificultad { get => PlayerPrefs.GetInt("ec.dif", 0); set { PlayerPrefs.SetInt("ec.dif", value); Changed?.Invoke(); } }

        // Menú docente
        public static string Curso { get => PlayerPrefs.GetString("ec.curso", "2°A"); set => PlayerPrefs.SetString("ec.curso", value); }
        public static int Modo { get => PlayerPrefs.GetInt("ec.modo", 45); set => PlayerPrefs.SetInt("ec.modo", value); }
        public static string ExportOverride; // solo pruebas automáticas (-exportdir)
        public static string CarpetaExport
        {
            get
            {
                if (!string.IsNullOrEmpty(ExportOverride)) return ExportOverride;
                var def = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EnganoColorido");
                return PlayerPrefs.GetString("ec.export", def);
            }
            set => PlayerPrefs.SetString("ec.export", value);
        }

        public static readonly string[] Dificultades = { "Fácil", "Normal", "Difícil" };

        public static void Save() => PlayerPrefs.Save();
    }
}
