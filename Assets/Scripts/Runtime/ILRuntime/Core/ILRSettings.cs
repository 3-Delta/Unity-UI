using System;
using System.IO;

using UnityEngine;

public static class ILRSettings {
    public const string HotfixDLLRelativePath = "../../Hotfix/ILRuntime/bin/Debug/Logic.Hotfix.dll";
    public const string HotfixPdbRelativePath = "../../Hotfix/ILRuntime/bin/Debug/Logic.Hotfix.pdb";

    /// <summary>
    /// 设定的自动生成的绑定脚本文件夹路径
    /// </summary>
    public const string BindingAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Bindings/Analysis";

    /// <summary>
    /// 设定的自动生成的delegate脚本文件夹路径
    /// </summary>
    public const string DelegateAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Delegates/Analysis";

    /// <summary>
    /// 设定的自动生成的适配脚本文件夹路径
    /// </summary>
    public const string AdaptorAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Adapters/Analysis";

    public static string HotfixDllFullPath {
        get {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, HotfixDLLRelativePath);
#else 
            return null;
#endif
        }
    }
    public static string HotfixPdbFullPath {
        get {
#if UNITY_EDITOR
            return Path.Combine(Application.dataPath, HotfixPdbRelativePath);
#else
            return null;
#endif
        }
    }
}
