using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

// 可以参考Rider的实现
// https://docs.microsoft.com/zh-cn/visualstudio/cross-platform/customize-project-files-created-by-vstu?view=vs-2019
// 如何在工程变化的时候，插入热更独立工程的csproject
public class ScriptAssetPostprocessor : AssetPostprocessor {
    public static string OnGeneratedSlnSolution(string path, string content) {
        return content;
    }

    public static string OnGeneratedCSProject(string path, string content) {
        return content;
    }
}
