using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;

public class BuildSetup
{
    [MenuItem("Build/Add All Scenes to Build Settings")]
    static void AddAllScenes()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            scenes[i] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = scenes;
        Debug.Log("Added " + guids.Length + " scenes to Build Settings.");
    }

    [MenuItem("Build/Build Android APK")]
    static void BuildAndroidAPK()
    {
        BuildAPK();
    }

    static void BuildAPK()
    {
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        string outputPath = Path.Combine(Application.dataPath, "../Builds/FNaF-Panorama.apk");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("APK built successfully: " + outputPath);
            EditorUtility.RevealInFinder(outputPath);
        }
        else
        {
            Debug.LogError("Android build failed: " + summary.totalErrors + " errors.");
            EditorApplication.Exit(1);
        }
    }
}
