using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class AssemblyProxy {
    private static IAssembly assembly = null;

    private static bool hasInited = false;

    public static bool TryInit() {
        if (hasInited) {
            return false;
        }

#if __NATIVE__
    assembly = null;
#elif __REFL_RELOAD__ && UNITY_EDITOR
    assembly = new AssemblyReload();
#elif __REFL__
    assembly = new AssemblyReflection();
#elif __ILR__
    assembly = new AssemblyILRuntime();
#endif

        assembly?.Load();
        hasInited = true;
        FBridge.Init();
        return true;
    }

    public static void Clear() {
        assembly?.Clear();
        assembly = null;
        hasInited = false;
    }

    public static Type[] GetTypes() {
        return assembly?.GetTypes();
    }
    
    public static object CreateInstance(string typeNameWithNamespace) {
        return assembly?.CreateInstance(typeNameWithNamespace);
    }

    public static StaticMethod CreateStaticMethod(string typeNameWithNamespace, string methodName, int argCount) {
        return assembly?.CreateStaticMethod(typeNameWithNamespace, methodName, argCount);
    }

    public static InstanceMethod CreateInstanceMethod(string typeNameWithNamespace, string methodName, ref object refInstance, int argCount) {
        return assembly?.CreateInstanceMethod(typeNameWithNamespace, methodName, ref refInstance, argCount);
    }

#if __REFL_RELOAD__ && UNITY_EDITOR
    [MenuItem("Assembly/HotReloadInject")]
    private static void HotReload() {
        AssemblyReload ass = assembly as AssemblyReload;
        ass?.LoadHotReload();
        
        // 重新执行UI配置
        StaticMethod method = ass?.CreateStaticMethod("Logic.Hotfix.HotfixBridge", "UIInject", 0);
        method?.Exec();
        
        UnityEngine.Debug.LogError("HotReload Inject Success");
    }
#endif
}
