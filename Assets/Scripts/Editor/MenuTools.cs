using UnityEditor;
using UnityEngine;

public class MenuTools : MonoBehaviour {
    [MenuItem("Tools/Path/PersistentPath", false, 1003)]
    public static void ApplicationPersistentPath() {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }

    [MenuItem("Tools/Path/DataPath", false, 1003)]
    public static void ApplicationDataPath() {
        EditorUtility.RevealInFinder(Application.dataPath);
    }

    [MenuItem("Tools/Path/StreamingAssetsPath", false, 1003)]
    public static void ApplicationStreamingAssetsPath() {
        EditorUtility.RevealInFinder(Application.streamingAssetsPath);
    }

    [MenuItem("Tools/Path/TemporaryCachePath", false, 1003)]
    public static void ApplicationTemporaryCachePath() {
        EditorUtility.RevealInFinder(Application.temporaryCachePath);
    }

    [MenuItem("Tools/Path/ProjectPath", false, 1003)]
    public static void ApplicationProjectPath() {
        EditorUtility.RevealInFinder(Application.dataPath + "/../");
    }

    [MenuItem("Tools/Path/CrashPath", false, 1003)]
    public static void ApplicationCrashPath() {
        string rootFolderPath = System.Environment.ExpandEnvironmentVariables("%localappdata%");
        string unityFolder = System.IO.Path.Combine(rootFolderPath, "Unity");
        EditorUtility.RevealInFinder(System.IO.Path.Combine(unityFolder, "Editor"));
    }

    [MenuItem("Tools/Path/LogPath", false, 1003)]
    public static void ApplicationLogPath() {
        EditorUtility.RevealInFinder(Application.consoleLogPath);
    }
}
