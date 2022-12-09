using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

public static class Menu_ILRuntime {
    [MenuItem("ILRuntime/安装VS调试插件")]
    static void InstallDebugger() {
        EditorUtility.OpenWithDefaultApp("Packages/com.ourpalm.ilruntime@2.0.2/Samples~/Basic Demo/Debugger~/ILRuntimeDebuggerLauncher.vsix");
    }

    [MenuItem("ILRuntime/打开ILRuntime中文文档")]
    static void OpenDocumentation() {
        Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
    }

    [MenuItem("ILRuntime/打开ILRuntime Github项目")]
    static void OpenGithub() {
        Application.OpenURL("https://github.com/Ourpalm/ILRuntime");
    }

    [MenuItem("ILRuntime/打开ILRuntime视频教程")]
    static void OpenTutorial() {
        Application.OpenURL("https://learn.u3d.cn/tutorial/ilruntime");
    }

    // 參考ET
    // [MenuItem("ILRuntime/Generate CLR Adaptor Code by Analysis")]
    // private static void GenerateCLRAdaptorByAnalysis() {
    //     //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
    //     //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
    //     using (StreamWriter sw = new StreamWriter("Assets/Mono/ILRuntime/ICriticalNotifyCompletionAdapter.cs")) {
    //         sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(
    //             typeof(ICriticalNotifyCompletion), "Logic.Runtime"));
    //     }
    //
    //     AssetDatabase.Refresh();
    //
    //     Debug.LogError("生成适配器完成");
    // }

    [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    public static void GenerateCLRBindingByAnalysis() {
        // 先删除旧代码
        AssetDatabase.DeleteAsset(HotfixSettings.BindingAnalysisFolderPath);
        Directory.CreateDirectory(HotfixSettings.BindingAnalysisFolderPath);

        // 分析热更DLL来生成绑定代码
        string dllFilePath = $"{HotfixSettings.HotfixHotReloadRelativePath}/{HotfixSettings.HotfixDLLFileName}.bytes";
        using (FileStream fs = new FileStream(dllFilePath, FileMode.Open, FileAccess.Read)) {
            ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            appDomain.LoadAssembly(fs);

            ILRService.Init(appDomain);
            // 生成所有绑定脚本
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appDomain, HotfixSettings.BindingAnalysisFolderPath);
        }

        AssetDatabase.Refresh();
        Debug.LogError("生成CLR绑定文件完成");
    }
}
