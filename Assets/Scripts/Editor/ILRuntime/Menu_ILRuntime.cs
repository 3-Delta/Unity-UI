using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ILRuntime.Runtime.Intepreter;
using UnityEditor;

public static class Menu_ILRuntime {
    // Todo : 将来修改为自动获取所有继承自主工程的类，然后获取adapter
    [MenuItem("Tools/ILRuntime/Generate CLR Adaptor Code by Analysis")]
    private static void GenerateCLRAdaptorByAnalysis() {
        var types = new List<Type>();
        //types.Add((typeof(UnityEngine.ScriptableObject)));
        //types.Add((typeof(System.Exception)));
        //types.Add(typeof(System.Collections.IEnumerable));

        AdapterGenerater.Generate(types);
    }

    // Todo
    // 自动分析所有的可能使用到的类和接口以及方法， GenerateCLRBindingByAnalysis只能分析到代码中使用的，一些没有使用的会被漏掉
    // 比如Debug.Log如果不调用，就不能被分析掉，这里主要是全量分析
    [MenuItem("Tools/ILRuntime/Generate CLR Binding Code by Manual")]
    private static void GenerateCLRBindingByManual() { }

    [MenuItem("Tools/ILRuntime/Generate CLR Binding Code by Analysis")]
    private static void GenerateCLRBindingByAnalysis() {
        // 先删除旧代码
        AssetDatabase.DeleteAsset(HotfixSettings.BindingAnalysisFolderPath);
        Directory.CreateDirectory(HotfixSettings.BindingAnalysisFolderPath);

        // 分析热更DLL来生成绑定代码
        string dllFilePath = $"{HotfixSettings.AssemblyFolderPath}/{HotfixSettings.HotfixDLLFileName}.bytes";
        using (FileStream fs = new FileStream(dllFilePath, FileMode.Open, FileAccess.Read)) {
            ILRuntime.Runtime.Enviorment.AppDomain appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            appDomain.LoadAssembly(fs);

            // Crossbind Adapter is needed to generate the correct binding code

            ILRService.Init(appDomain);
            // Instead of
            // ManualAdapterRegister.Register(appDomain);
            // AnalysisAdapterRegister.Register(appDomain);

            // Todo 生成原理？？？
            // 生成所有绑定脚本
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appDomain, HotfixSettings.BindingAnalysisFolderPath);
        }

        AssetDatabase.Refresh();
    }
}
