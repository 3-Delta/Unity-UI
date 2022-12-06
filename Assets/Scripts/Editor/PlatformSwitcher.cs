using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

public class PlatformSwitcher : IActiveBuildTargetChanged {
    public readonly List<BuildTarget> PLATFORMS = new List<BuildTarget>() {
        BuildTarget.StandaloneWindows64,
        BuildTarget.Android,
        BuildTarget.iOS,
        BuildTarget.StandaloneOSX,
    };

    public string Folder = "Platform";

    public void Reset() {
        BuildTarget current = EditorUserBuildSettings.activeBuildTarget;
    }

    public int callbackOrder { get; }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) {
        Debug.LogError("OnActiveBuildTargetChanged:" + previousTarget + " -> " + newTarget);

        if (previousTarget == newTarget) {
            return;
        }

        // https://docs.unity3d.com/Manual/SpecialFolders.html
        // 文件或者文件夹以.开头 或者 以~结尾，都会被unity忽略，也就是说不会生成meta文件，也就意味着AssetDataBase系列API不能使用
        string oldPath = $"{Application.dataPath}/{Folder}/{previousTarget}";
        string newPath = $"{Application.dataPath}/{Folder}/.{previousTarget}";
        if (Directory.Exists(oldPath)) {
            Directory.Move(oldPath, newPath);
            File.Move(oldPath + ".meta", newPath + ".meta");
        }

        oldPath = $"{Application.dataPath}/{Folder}/.{newTarget}";
        newPath = $"{Application.dataPath}/{Folder}/{newTarget}";
        if (Directory.Exists(oldPath)) {
            Directory.Move(oldPath, newPath);
            File.Move(oldPath + ".meta", newPath + ".meta");
        }

        AssetDatabase.Refresh();
    }
}