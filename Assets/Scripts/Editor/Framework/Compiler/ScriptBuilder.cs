using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

public class ScriptBuilder {
    // 构建整个工程中最终出包会用到的代码，编辑器代码会被ignore。
    // 也就是说相比Library\ScriptAssemblies会少一些
    public static bool Build() {
        string assembliesOutputPath = Application.dataPath + "/../Temp/ScriptBuilder";
        return Build(assembliesOutputPath);
    }

    public static bool Build(string assembliesOutputPath) {
        return Build(assembliesOutputPath, EditorUserBuildSettings.activeBuildTarget);
    }

    public static bool Build(string assembliesOutputPath, BuildTarget target) {
        var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
        ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings() {
            target = target,
            group = targetGroup,
            options = ScriptCompilationOptions.None
        };

        // 创建临时输出程序集路径
        //string TempOutputFolder = Path.GetDirectoryName(Application.dataPath) + "/AppScriptDll";
        if (Directory.Exists(assembliesOutputPath)) {
            Directory.Delete(assembliesOutputPath, true);
        }

        Directory.CreateDirectory(assembliesOutputPath);

        // 编译所有脚本
        ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, assembliesOutputPath);
        if (scriptCompilationResult.assemblies.Count > 0) {
            Debug.LogFormat($"Success to CompilePlayerScripts, total assembly count is: {scriptCompilationResult.assemblies.Count.ToString()}, at: {assembliesOutputPath}");
            return true;
        }
        else {
            // unity内部有自己的报错提示
            return false;
        }
    }
}
