using UnityEngine;
using UnityEditor;
using System.IO;

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
}
