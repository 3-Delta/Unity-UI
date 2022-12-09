using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Random = UnityEngine.Random;

public class AssemBuilder {
    public static List<string> GetFiles(string[] codeDirectorys, string fileExtension = "*.cs") {
        List<string> scripts = new List<string>();
        for (int i = 0; i < codeDirectorys.Length; i++) {
            DirectoryInfo dti = new DirectoryInfo(codeDirectorys[i]);
            FileInfo[] fileInfos = dti.GetFiles(fileExtension, SearchOption.AllDirectories);
            for (int j = 0; j < fileInfos.Length; j++) {
                scripts.Add(fileInfos[j].FullName);
            }
        }

        return scripts;
    }

    public static Assembly GetAssembly(string targetAssemblyNameWithExtension) {
        Assembly rlt = null;
        foreach (var ass in CompilationPipeline.GetAssemblies()) {
            bool isEqual = string.Equals(ass.name, Path.GetFileNameWithoutExtension(targetAssemblyNameWithExtension), StringComparison.Ordinal);
            if (isEqual) {
                rlt = ass;
                break;
            }
        }

        return rlt;
    }

    public static void Build(string absolutePath, string assemblyName, Assembly targetAssembly, string[] codeDirectorys) {
        List<string> scripts = GetFiles(codeDirectorys, "*.cs");
        string dllPath = Path.Combine(absolutePath, $"{assemblyName}.dll");
        string pdbPath = Path.Combine(absolutePath, $"{assemblyName}.pdb");
        if (File.Exists(dllPath)) {
            File.Delete(dllPath);
        }

        if (File.Exists(pdbPath)) {
            File.Delete(pdbPath);
        }

        Directory.CreateDirectory(absolutePath);

        AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());

        //启用UnSafe
        //assemblyBuilder.compilerOptions.AllowUnsafeCode = true;

        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        assemblyBuilder.buildTargetGroup = buildTargetGroup;
        assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
        assemblyBuilder.compilerOptions = targetAssembly.compilerOptions;
        assemblyBuilder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup);
        // assemblyBuilder.compilerOptions.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_4_6;

        // 带后缀的绝对路径
        assemblyBuilder.excludeReferences = null;

        assemblyBuilder.additionalReferences = targetAssembly.compiledAssemblyReferences;

        assemblyBuilder.flags = AssemblyBuilderFlags.EditorAssembly;
        //AssemblyBuilderFlags.None                 正常发布
        //AssemblyBuilderFlags.DevelopmentBuild     开发模式打包
        //AssemblyBuilderFlags.EditorAssembly       编辑器状态
        assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;
    }

    public static void Build(string absolutePath, string assemblyName, string[] codeDirectorys, string[] additionalReferences, string[] excludeReferences, Action<string> onBuildStart = null, Action<string, CompilerMessage[]> onBuildEnd = null) {
        List<string> scripts = GetFiles(codeDirectorys, "*.cs");

        string dllPath = Path.Combine(absolutePath, $"{assemblyName}.dll");
        string pdbPath = Path.Combine(absolutePath, $"{assemblyName}.pdb");
        if (File.Exists(dllPath)) {
            File.Delete(dllPath);
        }

        if (File.Exists(pdbPath)) {
            File.Delete(pdbPath);
        }

        if (!Directory.Exists(absolutePath)) {
            Directory.CreateDirectory(absolutePath);
        }

        AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());

        //启用UnSafe
        //assemblyBuilder.compilerOptions.AllowUnsafeCode = true;

        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        assemblyBuilder.buildTargetGroup = buildTargetGroup;
        assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
        assemblyBuilder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup);
        // assemblyBuilder.compilerOptions.ApiCompatibilityLevel = ApiCompatibilityLevel.NET_4_6;

        // 带后缀的绝对路径
        assemblyBuilder.excludeReferences = excludeReferences;

        assemblyBuilder.additionalReferences = additionalReferences;

        assemblyBuilder.flags = AssemblyBuilderFlags.EditorAssembly;
        //AssemblyBuilderFlags.None                 正常发布
        //AssemblyBuilderFlags.DevelopmentBuild     开发模式打包
        //AssemblyBuilderFlags.EditorAssembly       编辑器状态
        assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;

        assemblyBuilder.buildStarted += (assemblyPath) => {
            onBuildStart?.Invoke(assemblyPath);
            OnBuildStarted(assemblyPath);
        };
        assemblyBuilder.buildFinished += (assemblyPath, compilerMessages) => {
            onBuildEnd?.Invoke(assemblyPath, compilerMessages);
            OnBuildFinished(assemblyPath, compilerMessages);
        };

        try {
            float progress = 0f;
            EditorUtility.DisplayCancelableProgressBar("AssemblyBuilder.Build", "...", progress);
            //开始构建
            if (!assemblyBuilder.Build()) {
                Debug.LogErrorFormat("Build Assembly Fail：" + assemblyBuilder.assemblyPath);
            }
            else {
                while (assemblyBuilder.status != AssemblyBuilderStatus.Finished) {
                    progress += 0.1f;
                    EditorUtility.DisplayCancelableProgressBar("AssemblyBuilder.Build", "...", progress);
                }
                Debug.LogErrorFormat("Build Assembly Success：" + assemblyBuilder.assemblyPath);
            }
        }
        finally {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void OnBuildStarted(string assemblyPath) {
        Debug.LogFormat("Build Assembly Start：" + assemblyPath);
    }

    private static void OnBuildFinished(string assemblyPath, CompilerMessage[] compilerMessages) {
        int errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
        int warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

        Debug.LogFormat("Warnings: {0} - Errors: {1}", warningCount.ToString(), errorCount.ToString());

        if (warningCount > 0) {
            Debug.LogFormat("有{0}个Warning", warningCount.ToString());
        }

        if (errorCount > 0) {
            for (int i = 0; i < compilerMessages.Length; i++) {
                if (compilerMessages[i].type == CompilerMessageType.Error) {
                    Debug.LogError(compilerMessages[i].message);
                }
            }
        }
    }
}

public class HotReloadBuilder {
    [MenuItem("Assembly/Build/HotfixAll(Fixed + HotReload)")]
    public static void BuildHotfixAll() {
        AssemBuilder.Build(HotfixSettings.HotfixHotReloadRelativePath, "Logic.Hotfix", new[] {
            "Hotfix/Fixed/",
            "Hotfix/HotReload/",
        }, Array.Empty<string>(), null);
    }
    
    [MenuItem("Assembly/Build/HotfixSeparate(Fixed + HotReload)")]
    public static void BuildHotfixSeparate() {
        BuildFixed();
        BuildHotReload();
    }

    [MenuItem("Assembly/Build/Fixed")]
    public static void BuildFixed() {
        AssemBuilder.Build(HotfixSettings.HotfixHotReloadRelativePath, "Logic.Hotfix.Fixed", new[] {
            "Hotfix/Fixed/",
        }, Array.Empty<string>(), null);
    }

    [MenuItem("Assembly/Build/HotReload")]
    // https://github.com/egametang/ET/blob/33a7d0dd8425eda22334b325ca5cf6d40711ab05/Unity/Assets/Editor/BuildEditor/BuildAssemblieEditor.cs#L56
    public static void BuildHotReload() {
        string[] files = Directory.GetFiles(HotfixSettings.HotfixHotReloadRelativePath, HotfixSettings.HotReloadDLLName);
        foreach (string oneFile in files) {
            File.Delete(oneFile);
        }

        int random = Random.Range(1, int.MaxValue);
        string fileName = $"Logic.Hotfix.HotReload_{random.ToString()}";

        AssemBuilder.Build(HotfixSettings.HotfixHotReloadRelativePath, fileName, new[] {
            "Hotfix/HotReload/",
        }, new[] {
            Path.Combine(HotfixSettings.HotfixHotReloadRelativePath, HotfixSettings.HotfixFixedDLLName + ".dll"),
        }, new string[] {
            Path.Combine(Application.dataPath, "ABSource/DLL/Logic.Hotfix.dll"),
        });
    }
}
