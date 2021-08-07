using UnityEditor;

using UnityEngine;

public static class BuildMenuItem {
    #region Build
    [MenuItem("Assets/Build/EditorBundle")]
    public static void BuildEditorBundle() {
        ABBuilder.Build(true);
    }

    [MenuItem("Assets/Build/AssetBundle")]
    public static void BuildAssetBundle() {
        ABBuilder.Build(false);
    }

    [MenuItem("Assets/Build/Player")]
    public static void BuildPlayer() {
        PlayerBuilder.Build();
    }

    [MenuItem("Assets/Build/ClearProgressBar")]
    public static void ClearProgressBar() {
        EditorUtility.ClearProgressBar();
    }

    public static void DisplayProgressBar(string title, string content, int index, int max) {
        EditorUtility.DisplayProgressBar($"{title}({index}/{max}) ", content, index * 1f / max);
    }

    [MenuItem("Assets/Build/ClearABLabel")]
    public static void ClearABLabel() {
        var os = Selection.objects;
        foreach (var item in os) {
            string path = AssetDatabase.GetAssetPath(item);
            AssetImporter imper = AssetImporter.GetAtPath(path);
            imper.assetBundleName = null;

            EditorUtility.SetDirty(imper);
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
    }

    #endregion

    #region path
    [MenuItem("Assets/Path/ABOutputPath")]
    public static void OpenABOutputPath() {
        EditorUtility.RevealInFinder(BuildSetting.AppVersionOutputPath);
    }
    [MenuItem("Assets/Path/PersistentPath")]
    public static void ApplicationPersistentPath() {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
    [MenuItem("Assets/Path/DataPath")]
    public static void ApplicationDataPath() {
        EditorUtility.RevealInFinder(Application.dataPath);
    }
    [MenuItem("Assets/Path/StreamingAssetsPath")]
    public static void ApplicationStreamingAssetsPath() {
        EditorUtility.RevealInFinder(Application.streamingAssetsPath);
    }
    [MenuItem("Assets/Path/CachePath")]
    public static void ApplicationTemporaryCachePath() {
        EditorUtility.RevealInFinder(Application.temporaryCachePath);
    }
    [MenuItem("Assets/Path/CrashPath")]
    public static void ApplicationCrashPath() {
        string rootFolderPath = System.Environment.ExpandEnvironmentVariables("%localappdata%");
        string unityFolder = System.IO.Path.Combine(rootFolderPath, "Unity");
        EditorUtility.RevealInFinder(System.IO.Path.Combine(unityFolder, "Editor"));
    }
    #endregion
}
