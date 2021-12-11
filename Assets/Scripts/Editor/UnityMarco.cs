using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class UnityMarco {
    public static List<string> GetMarcos() {
        List<string> marcos = new List<string>();
        string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!string.IsNullOrEmpty(groups)) {
            return groups.Split(';').ToList<string>();
        }

        return marcos;
    }

    public static void ClearMarcos() {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, "");
    }

    public static void AddMarco(string marco) {
        if (!string.IsNullOrEmpty(marco)) {
            string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!groups.Contains(marco)) {
                groups = groups + ";" + marco;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, groups);
            }
        }
    }

    public static void RemoveMarco(string marco) {
        if (!string.IsNullOrEmpty(marco)) {
            string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (groups.Contains(marco)) {
                groups = groups.Replace(marco + ";", "");
                groups = groups.Replace(marco, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, groups);
            }
        }
    }
}
