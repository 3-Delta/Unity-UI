using System;
using System.IO;
using UnityEngine;

public static class HotfixSettings {
    /// <summary>
    /// Unity编译的DLL存储目录
    /// </summary>
    public const string ScriptAssembliesDir = "Library/ScriptAssemblies";

    /// <summary>
    /// 设定的DLL存储目录
    /// </summary>
    public const string AssemblyFolderPath = "Assets/ABSource/DLL";

    /// <summary>
    /// 设定的热更DLL文件名称
    /// </summary>
    public const string HotfixDLLFileName = "Logic.Hotfix.dll";

    /// <summary>
    /// 设定的热更PDB文件名称
    /// </summary>
    public const string HotfixPDBFileName = "Logic.Hotfix.pdb";

    /// <summary>
    /// 设定的手动生成的绑定脚本文件夹路径
    /// </summary>
    public const string BindingManualFolderPath = "Assets/Scripts/Runtime/ILRuntime/ILRBindings/Manual";

    /// <summary>
    /// 设定的自动生成的适配脚本文件夹路径
    /// </summary>
    public const string AdaptorAnalysisFilePath = "Assets/Scripts/Runtime/ILRuntime/Adapters";

    public const string HotfixDLLRelativePath = "ABSource/DLL/Logic.Hotfix.dll";
    public const string HotfixPdbRelativePath = "ABSource/DLL/Logic.Hotfix.pdb";

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
