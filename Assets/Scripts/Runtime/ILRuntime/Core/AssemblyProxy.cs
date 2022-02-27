using System;

public enum EAssemblyLoadType {
    // 原生C#模式
    ByNative,

    // ILRuntime模式
    ByILRuntime,

    // 反射模式
    ByReflection,
}

public static class AssemblyProxy {
    public static EAssemblyLoadType assemblyLoadType { get; set; } = EAssemblyLoadType.ByILRuntime;

    private static IAssembly assembly = null;

    public static void Init() {
        if (assemblyLoadType == EAssemblyLoadType.ByNative) {
            assembly = null;
        }
        else if (assemblyLoadType == EAssemblyLoadType.ByILRuntime) {
            assembly = new AssemblyILRuntime();
        }
        else if (assemblyLoadType == EAssemblyLoadType.ByReflection) {
            assembly = new AssemblyReflection();
        }

        assembly?.Load();
    }

    public static void Clear() {
        assembly?.Clear();
        assembly = null;
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
}
