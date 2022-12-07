using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

// 要求文件夹名字和平台名字对应
public class PlatformSwitcher : IActiveBuildTargetChanged {
    [System.Serializable]
    public class Item {
        public string path;
        public bool meta;
    }

    public readonly List<Item> Paths = new List<Item>() {
        new Item() {
            path = $"{Application.dataPath}/../OffLinePackages/Platform",
            meta = false 
        },
    };

    public int callbackOrder { get; }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget) {
        Debug.LogError("OnActiveBuildTargetChanged:  " + previousTarget + " -> " + newTarget);

        if (previousTarget == newTarget) {
            return;
        }

        // https://docs.unity3d.com/Manual/SpecialFolders.html
        // 文件或者文件夹以.开头 或者 以~结尾，都会被unity忽略，也就是说不会生成meta文件，也就意味着AssetDataBase系列API不能使用
        foreach (var one in Paths) {
            string oldPath = $"{one.path}/{previousTarget}";
            string newPath = $"{one.path}/.{previousTarget}";
            if (Directory.Exists(oldPath)) {
                Directory.Move(oldPath, newPath);
                if(one.meta) {
                    File.Move(oldPath + ".meta", newPath + ".meta");
                }
            }

            oldPath = $"{one.path}/.{newTarget}";
            newPath = $"{one.path}/{newTarget}";
            if (Directory.Exists(oldPath)) {
                Directory.Move(oldPath, newPath);
                if(one.meta) {
                    File.Move(oldPath + ".meta", newPath + ".meta");
                }
            }
        }

        AssetDatabase.Refresh();
    }
}