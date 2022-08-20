#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

// 根据平台来决定当前gameobject是否会被打包进去
// 比如希望android把合格go打进去，ios不进去
// https://answer.uwa4d.com/question/62f4b97a343a54268c830e11
[DisallowMultipleComponent]
public class PlatformEditorOnly : MonoBehaviour, IPreprocessBuildWithReport {
    public List<BuildTarget> editorOnly = new List<BuildTarget>();

    // 本脚本只在Editor下使用
    private void Reset() {
        this.hideFlags = HideFlags.DontSaveInBuild;
    }

    // 打包构建之前调用
    public void Set(BuildTarget target) {
        var cur = target; // EditorUserBuildSettings.activeBuildTarget;
        foreach (var one in editorOnly) {
            if (one == cur) {
                var o = gameObject;
                o.tag = "EditorOnly";
                EditorUtility.SetDirty(o);
            }
        }
    }

    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report) {
        Set(report.summary.platform);
    }
}
#endif