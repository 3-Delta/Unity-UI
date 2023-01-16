using System;

using UnityEditor;

using UnityEngine;

// 可以参考Rider的实现
// https://answer.uwa4d.com/question/5cc1190f5dad710abee80429#5cc13cfd5dad710abee80431
// https://docs.microsoft.com/zh-cn/visualstudio/cross-platform/customize-project-files-created-by-vstu?view=vs-2019
// https://answer.uwa4d.com/question/637db649f9f21132efff180e
// 如何在工程变化的时候，插入热更独立工程的csproject
public class ScriptAssetPostprocessor : AssetPostprocessor
{
    // 编译成果回调
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnCompiled()
    {
        Debug.LogError("编译成功");
    }

    // 参考：
    // 设置启动项目 https://www.cnblogs.com/dudu/p/set-default-startup-project-in-visual-studio.html
    // https://qiita.com/toRisouP/items/6b0fc5eb97b0d7ce1499
    // 7. https://blog.csdn.net/u010019717/article/details/89737747
    public static string OnGeneratedSlnSolution(string path, string content)
    {
        string slnGUID = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
        string prjGUID = "9AA23530-C9A6-478D-90A4-C897C7147769";
        string prjName = "Logic.Hotfix";
        string prjPath = @"Hotfix\Logic.Hotfix.csproj";

        if (content.Contains(prjName))
        {
            return content;
        }

        string append = $"Project(\"{slnGUID}\") = \"{prjName}\", \"{ prjPath}\", \"{prjGUID}\", \"{Environment.NewLine}\"EndProject";
        string newContent = content.Replace($"EndProject{Environment.NewLine}Global",
            $"EndProject{Environment.NewLine}{append}{Environment.NewLine}Global");
        return newContent;
    }

    public static string OnGeneratedCSProject(string path, string content)
    {
        return content;
    }
}
