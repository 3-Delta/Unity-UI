using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class UnityMarco {
    public static List<string> GetMarcos() {
        return GetMarcos(EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static List<string> GetMarcos(BuildTargetGroup group) {
        List<string> marcos = new List<string>();
        string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        if (!string.IsNullOrEmpty(groups)) {
            return groups.Split(';').ToList<string>();
        }

        return marcos;
    }

    public static void ClearMarcos() {
        ClearMarcos(EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static void ClearMarcos(BuildTargetGroup group) {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, "");
    }

    public static void AddMarco(string marco) {
        AddMarco(marco, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static void AddMarco(string marco, BuildTargetGroup group) {
        if (!string.IsNullOrEmpty(marco)) {
            string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (!groups.Contains(marco)) {
                groups = groups + ";" + marco;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groups);
            }
        }
    }

    public static void RemoveMarco(string marco) {
        RemoveMarco(marco, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static void RemoveMarco(string marco, BuildTargetGroup group) {
        if (!string.IsNullOrEmpty(marco)) {
            string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            if (groups.Contains(marco)) {
                groups = groups.Replace(marco + ";", "");
                groups = groups.Replace(marco, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groups);
            }
        }
    }
}
