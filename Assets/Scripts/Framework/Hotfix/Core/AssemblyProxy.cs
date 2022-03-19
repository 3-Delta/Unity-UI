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

    public static object CreateInstance(string fullName) {
        return assembly?.CreateInstance(fullName);
    }

    public static StaticMethod CreateStaticMethod(string typeNameIncludeNamespace, string methodName, int argCount) {
        return assembly?.CreateStaticMethod(typeNameIncludeNamespace, methodName, argCount);
    }

    public static InstanceMethod CreateInstanceMethod(string typeNameIncludeNamespace, string methodName, ref object refInstance, int argCount) {
        return assembly?.CreateInstanceMethod(typeNameIncludeNamespace, methodName, ref refInstance, argCount);
    }

#if __REFL_RELOAD__ && UNITY_EDITOR
    [MenuItem("Assembly/HotReload")]
    private static void Reload() {
        (assembly as AssemblyReload)?.LoadHotReload();
        UnityEngine.Debug.LogError("HotReload Success");
    }
#endif
}
