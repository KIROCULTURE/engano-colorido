using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// Build de Windows. Batch:
/// Unity.exe -batchmode -quit -projectPath . -executeMethod BuildScript.BuildWindows -hito M1_Nucleo -logFile build.log
public static class BuildScript
{
    public const string Exe = "EnganoColorido.exe";

    [MenuItem("Tools/Engaño Colorido/Build Windows", priority = 30)]
    public static void BuildWindowsMenu() => Build("Dev");

    public static void BuildWindows() => Build(Arg("-hito") ?? Environment.GetEnvironmentVariable("EC_HITO") ?? "Dev");

    static void Build(string hito)
    {
        var scenes = EditorBuildSettings.scenes.Where(s => s.enabled && File.Exists(s.path)).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
        {
            Debug.LogError("No hay escenas en Build Settings. Usa Tools/Engaño Colorido/Build Greybox Scenes.");
            if (Application.isBatchMode) EditorApplication.Exit(1);
            return;
        }
        string dir = Path.Combine("Builds", hito);
        Directory.CreateDirectory(dir);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone, ScriptingImplementation.Mono2x);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(dir, Exe),
            target = BuildTarget.StandaloneWindows64,
            targetGroup = BuildTargetGroup.Standalone,
            options = BuildOptions.None
        });
        var s = report.summary;
        Debug.Log($"[BuildScript] {hito}: {s.result} · {s.totalErrors} errores · {s.totalSize / (1024 * 1024)} MB · {s.totalTime}");
        // Carpeta que Burst deja junto al exe y no se distribuye
        foreach (var burst in Directory.GetDirectories(dir, "*_BurstDebugInformation_DoNotShip"))
            Directory.Delete(burst, true);
        if (Application.isBatchMode) EditorApplication.Exit(s.result == BuildResult.Succeeded ? 0 : 1);
    }

    static string Arg(string name)
    {
        var a = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(a, name);
        return i >= 0 && i + 1 < a.Length ? a[i + 1] : null;
    }
}
