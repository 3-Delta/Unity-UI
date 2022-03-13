using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
public class AssemblyReload : IAssembly {
    private Assembly fixedAssembly = null;
    private Assembly hotReloadAssembly = null;

    private Type[] allTypes;

    private Dictionary<string, Type> fixedTypes = new Dictionary<string, Type>();
    private Dictionary<string, Type> hotReloadTypes = new Dictionary<string, Type>();

    public object CreateInstance(string fullName) {
        // 不知道能否？
        // fixedAssembly.CreateInstance(fullName) ?? hotReloadAssembly.CreateInstance(fullName);
        object rlt = null;
        try {
            if (fixedTypes.TryGetValue(fullName, out _)) {
                rlt = fixedAssembly.CreateInstance(fullName);
            }
            else if (hotReloadTypes.TryGetValue(fullName, out _)) {
                rlt = hotReloadAssembly.CreateInstance(fullName);
            }

            return rlt;
        }
        catch {
            return null;
        }
    }

    public InstanceMethod CreateInstanceMethod(string typeNameIncludeNamespace, string methodName, ref object refInstance, int argCount) {
        return new MonoInstanceMethod(fixedAssembly, typeNameIncludeNamespace, methodName, ref refInstance);
    }

    public StaticMethod CreateStaticMethod(string typeNameIncludeNamespace, string methodName, int argCount) {
        return new MonoStaticMethod(fixedAssembly, typeNameIncludeNamespace, methodName);
    }

    public Type[] GetTypes() {
        return allTypes;
    }

    public void Clear() {
        fixedAssembly = null;
        hotReloadAssembly = null;
        allTypes = null;
    }

    public void Load() {
        LoadHotfixFixed();
        LoadHotReload();
    }

    public void LoadHotfixFixed() {
        string dllFullPath = Path.Combine(HotfixSettings.HotfixHotReloadRelativePath, HotfixSettings.HotfixFixedDLLNameWithExt);
        byte[] dllBytes = PathService.GetFileBytes(dllFullPath);

        try {
#if UNITY_EDITOR
            // editor模式下，加载pdb for debug
            string pdbFullPath = Path.Combine(HotfixSettings.HotfixHotReloadRelativePath, HotfixSettings.HotfixFixedPdbNameWithExt);
            byte[] pdbBytes = PathService.GetFileBytes(pdbFullPath);
            fixedAssembly = Assembly.Load(dllBytes, pdbBytes);
#else
            fixedAssembly = Assembly.Load(dllBytes);
#endif
            fixedTypes.Clear();
            foreach (var one in fixedAssembly.GetTypes()) {
                fixedTypes.Add(one.FullName, one);
            }
        }
        catch {
            Debug.LogError("请先编译生成 fixed dll");
        }
    }

    public void LoadHotReload() {
        string[] files = Directory.GetFiles(HotfixSettings.HotfixHotReloadRelativePath, HotfixSettings.HotReloadDLLName);
        if (files.Length != 1) {
            throw new Exception("HotReload dll count != 1");
        }

        string nameWithoutExtension = Path.GetFileNameWithoutExtension(files[0]);
        string dllPath = Path.Combine(Application.dataPath, HotfixSettings.HotfixHotReloadRelativePath, nameWithoutExtension + ".dll");
        byte[] dllBytes = PathService.GetFileBytes(dllPath);

        try {
#if UNITY_EDITOR
            // editor模式下，加载pdb for debug
            string pdbPath = Path.Combine(Application.dataPath, HotfixSettings.HotfixHotReloadRelativePath, nameWithoutExtension + ".pdb");
            byte[] pdbBytes = PathService.GetFileBytes(pdbPath);
            hotReloadAssembly = Assembly.Load(dllBytes, pdbBytes);
#else
            hotReloadAssembly = Assembly.Load(dllBytes);
#endif
            hotReloadTypes.Clear();
            foreach (var one in hotReloadAssembly.GetTypes()) {
                hotReloadTypes.Add(one.FullName, one);
            }

            List<Type> listType = new List<Type>();
            foreach (var kvp in fixedTypes) {
                listType.Add(kvp.Value);
            }

            foreach (var kvp in hotReloadTypes) {
                listType.Add(kvp.Value);
            }

            allTypes = listType.ToArray();
        }
        catch {
            Debug.LogError("请先编译生成 hotreload dll");
        }
    }
}
#endif
