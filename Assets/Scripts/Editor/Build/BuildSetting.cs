using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildSetting {
    public static bool ChangeABVersion = true;
    public static readonly string ABOutputPath = $"{Application.dataPath}/{"../ABOutput"}";

    public static string AppVersionOutputPath {
        get {
            var platform = EditorUserBuildSettings.activeBuildTarget;
            string appVersion = GlobalSetting.AppVersion.ToString();
            return $"{ABOutputPath}/{platform}/AppVersion_{appVersion}";
        }
    }

    public static string GetABOutputPath(uint abVersion) {
        string appVersion = AppVersionOutputPath;
        return $"{appVersion}/ABVersion_{abVersion}";
    }

    public static string GetABOutputVersionPath(uint abVersion) {
        string appVersion = AppVersionOutputPath;
        return $"{appVersion}/ABVersion_{abVersion}/ab.version.json";
    }

    public static string CurrentAppVersionPath {
        get {
            var platform = EditorUserBuildSettings.activeBuildTarget;
            return $"{ABOutputPath}/{platform}/app.version.json";
        }
    }

    public static string CurrentABVersionPath {
        get {
            string appVersionPath = AppVersionOutputPath;
            return $"{appVersionPath}/ab.version.json";
        }
    }

    public static string ABBuildHistroyPath {
        get {
            string appVersionPath = AppVersionOutputPath;
            return $"{appVersionPath}/ab.histroy.json";
        }
    }

    public static BundleRecords CurrentABRecords {
        get {
            var manifest = new BundleRecords();
            var path = BuildSetting.CurrentABVersionPath;
            // 如果文件不存在,则版本号就是默认值0
            if (File.Exists(path)) {
                manifest.FromJson(path);
            }

            return manifest;
        }
    }

    public static BundleHistroy CurrentABHistroy {
        get {
            var manifest = new BundleHistroy();
            var path = BuildSetting.ABBuildHistroyPath;
            // 如果文件不存在,则版本号就是默认值0
            if (File.Exists(path)) {
                manifest.FromJson(path);
            }

            return manifest;
        }
    }
}