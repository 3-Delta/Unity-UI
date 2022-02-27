using System;
using System.IO;
using System.Linq;

using ILRuntime.Runtime.Enviorment;

using UnityEngine;

public class AssemblyILRuntime : IAssembly {
    private ILRuntime.Runtime.Enviorment.AppDomain appDomain = null;
    private MemoryStream dllMemory;
    private MemoryStream pdbMemory;

    public object CreateInstance(string fullName) {
        try {
            return appDomain.Instantiate(fullName);
        }
        catch {
            return null;
        }
    }
    public InstanceMethod CreateInstanceMethod(string typeNameIncludeNamespace, string methodName, ref object refInstance, int argCount) {
        return new ILRInstanceMethod(appDomain, typeNameIncludeNamespace, methodName, ref refInstance, argCount);
    }
    public StaticMethod CreateStaticMethod(string typeNameIncludeNamespace, string methodName, int argCount) {
        return new ILRStaticMethod(appDomain, typeNameIncludeNamespace, methodName, argCount);
    }
    public Type[] GetTypes() {
        return appDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
    }
    public void Clear() {
        // 清理Jit子线程
        appDomain.Dispose();

        // 新版本dllde stream只有再最终释放的时候才去释放，而不是LoadAssembly的时候就去释放
        // 如果忘记释放，或者unity还引用dll的时候，热更工程重新进行了编译，那么就会共同持有dll，导致问题
        dllMemory?.Close();
        pdbMemory?.Close();
    }

    public void Load() {
        appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

        string dllFullPath = ILRSettings.HotfixDllFullPath;
        byte[] dllBytes = IOService.GetFileBytes(dllFullPath);
        dllMemory = new MemoryStream(dllBytes);
        try {
#if UNITY_EDITOR
            string pdbFullPath = ILRSettings.HotfixPdbFullPath;
            byte[] pdbBytes = IOService.GetFileBytes(pdbFullPath);
            pdbMemory = new MemoryStream(pdbBytes);
            appDomain.LoadAssembly(dllMemory, pdbMemory, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
#else
            appDomain.LoadAssembly(dllMemory);
#endif
            ILRService.Init(appDomain);

            // 未捕获的异常都走这里
            // AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            // {
            //     Debug.Log($"UnhandledException: {ex.ExceptionObject}");
            // };
        }
        catch {
            Debug.LogError("请先编译生成dll");
        }
    }
}
