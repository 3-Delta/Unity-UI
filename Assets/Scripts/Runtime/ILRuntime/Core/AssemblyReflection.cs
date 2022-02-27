using System;
using System.Reflection;

using UnityEngine;

public class AssemblyReflection : IAssembly {
    private Assembly assembly = null;

    public object CreateInstance(string fullName) {
        try {
            return assembly.CreateInstance(fullName);
        }
        catch {
            return null;
        }
    }
    public InstanceMethod CreateInstanceMethod(string typeNameIncludeNamespace, string methodName, ref object refInstance, int argCount) {
        return new MonoInstanceMethod(assembly, typeNameIncludeNamespace, methodName, ref refInstance);
    }
    public StaticMethod CreateStaticMethod(string typeNameIncludeNamespace, string methodName, int argCount) {
        return new MonoStaticMethod(assembly, typeNameIncludeNamespace, methodName);
    }
    public Type[] GetTypes() {
        return assembly.GetTypes();
    }
    public void Clear() {
        assembly = null;
    }

    public void Load() {
        string dllFullPath = ILRSettings.HotfixDllFullPath;
        byte[] dllBytes = IOService.GetFileBytes(dllFullPath);
        try {
#if UNITY_EDITOR
            // editor模式下，加载pdb for debug
            string pdbFullPath = ILRSettings.HotfixPdbFullPath;
            byte[] pdbBytes = IOService.GetFileBytes(pdbFullPath);
            assembly = Assembly.Load(dllBytes, pdbBytes);
#else
        assembly = Assembly.Load(dllBytes);
#endif
        }
        catch {
            Debug.LogError("请先编译生成dll");
        }
    }
}
