using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Player;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEngine;

public class PlayerBuilder : IPostprocessBuildWithReport, IPreprocessBuildWithReport {
    public int callbackOrder { get; }

    public static void Build() {
       
    }

    public static bool TryCompile(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup) {
        ScriptCompilationSettings settings = new ScriptCompilationSettings() {
            target = buildTarget,
            group = buildTargetGroup,
            options = ScriptCompilationOptions.DevelopmentBuild
        };

        string outputFolder = HotfixSettings.HotfixDLLRelativePath;
        ScriptCompilationResult result = PlayerBuildInterface.CompilePlayerScripts(settings, outputFolder);
        int count = result.assemblies.Count;
        if (count > 0) {
            Debug.LogError("生成的程序集 个数 " + count.ToString());
            return true;
        }

        AssemblyBuilder assemblyBuilder = null; // 参数是全路径
        assemblyBuilder.buildTarget = buildTarget;
        assemblyBuilder.buildTargetGroup = buildTargetGroup;
        // 反选那些程序集(全路径 + 后缀)
        assemblyBuilder.excludeReferences = new string[0];
        assemblyBuilder.buildStarted  += delegate(string assemblyPath) {  };
        assemblyBuilder.buildFinished += delegate(string assemblyPath, CompilerMessage[] messages) {
            foreach (var item in messages) {
                if (item.type == CompilerMessageType.Error) {
                    Debug.LogError(item.message);
                }
            }
        };

        if (assemblyBuilder.Build()) {
            while (assemblyBuilder.status != AssemblyBuilderStatus.Finished) {
                // 计算进度
            }
            
            // 拷贝dll 以便 生成ilr绑定
        }


        return false;
    }

    public void OnPreprocessBuild(BuildReport report) {
        // 清理console
        Debug.ClearDeveloperConsole();
        
        // 重新编译脚本
        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

        bool result = TryCompile(buildTarget, buildTargetGroup);
        if (!result) {
            Debug.LogError("编译代码报错");
            return;
        }
        
        // 重新生成ilr绑定代码
        Menu_ILRuntime.GenerateCLRBindingByAnalysis();
        
        // 生成pkg

        AssetDatabase.Refresh();
    }

    public void OnPostprocessBuild(BuildReport report) {
    }
}
