using System;
using System.IO;
using UnityEngine;

public static class HotfixSettings {
    /// Unity编译的DLL存储目录
    public const string ScriptAssembliesDir = "Library/ScriptAssemblies";

    /// 设定的DLL存储目录
    public const string AssemblyFolderPath = "Assets/ABSource/DLL";

    /// 设定的热更DLL文件名称
    public const string HotfixDLLFileName = "Logic.Hotfix.dll";

    /// 设定的热更PDB文件名称
    public const string HotfixPDBFileName = "Logic.Hotfix.pdb";

    public const string HotfixFixedDLLName = "Logic.Hotfix.Fixed";
    public const string HotfixFixedPdbName = "Logic.Hotfix.Fixed";
    
    public const string HotfixFixedDLLNameWithExt = "Logic.Hotfix.Fixed.dll";
    public const string HotfixFixedPdbNameWithExt = "Logic.Hotfix.Fixed.pdb";

    // 参考ET，因为Unity如果不进行Refresh,就会始终返回缓存，所以命名后缀添加随机数
    public const string HotReloadDLLName = "Logic.Hotfix.HotReload_*";

    /// 设定的手动生成的绑定脚本文件夹路径
    public const string BindingManualFolderPath = "Assets/Scripts/Runtime/ILRuntime/ILRBindings/Manual";

    /// 设定的自动生成的适配脚本文件夹路径
    public const string AdaptorAnalysisFilePath = "Assets/Scripts/Runtime/ILRuntime/Adapters";

    public const string HotfixDLLRelativePath = "ABSource/DLL/" + HotfixDLLFileName;
    public const string HotfixPdbRelativePath = "ABSource/DLL/" + HotfixPDBFileName;

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

    public const string HotfixHotReloadRelativePath = "Temp/bin";

    /// 设定的自动生成的绑定脚本文件夹路径
    public const string BindingAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Bindings/Analysis";

    /// 设定的自动生成的delegate脚本文件夹路径
    public const string DelegateAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Delegates/Analysis";

    /// 设定的自动生成的适配脚本文件夹路径
    public const string AdaptorAnalysisFolderPath = "Assets/Scripts/Runtime/ILRuntime/Adapters/Analysis";
}
