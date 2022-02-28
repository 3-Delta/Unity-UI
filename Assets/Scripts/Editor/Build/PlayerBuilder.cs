using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PlayerBuilder : IPostprocessBuildWithReport, IPreprocessBuildWithReport {
    public static void Build() { }

    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report) {
        // UnityMarco.RemoveMarco(DefMarco.__DEV_MODE__);
        // // 重新编译代码
        //
        // // 重新生成ilr绑定代码
        // Menu_ILRuntime.GenerateCLRBindingByAnalysis();
        //
        // AssetDatabase.Refresh();
    }

    public void OnPostprocessBuild(BuildReport report) {
        // UnityMarco.AddMarco(DefMarco.__DEV_MODE__);
    }
}
