using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class UnityMarco {
    [MenuItem("Marco/Native")]
    private static void MarcoNative() {
        UnityMarcoDef.SetMarco(EMarcoType.Native);
    }

    [MenuItem("Marco/ReflReload")]
    private static void MarcoReflReload() {
        UnityMarcoDef.SetMarco(EMarcoType.ReflReload);
    }

    [MenuItem("Marco/Refl")]
    private static void MarcoRefl() {
        UnityMarcoDef.SetMarco(EMarcoType.Refl);
    }

    [MenuItem("Marco/Ilr")]
    private static void MarcoIlr() {
        UnityMarcoDef.SetMarco(EMarcoType.Ilr);
    }

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

    public static void ChangeMarco(IList<string> toRemove, IList<string> toAdd) {
        ChangeMarco(toRemove, toAdd, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static void ChangeMarco(IList<string> toRemove, IList<string> toAdd, BuildTargetGroup group) {
        string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        bool hasChanged = false;
        foreach (var marco in toRemove) {
            if (groups.Contains(marco)) {
                groups = groups.Replace(marco + ";", "");
                groups = groups.Replace(marco, "");

                hasChanged = true;
            }
        }

        foreach (var marco in toAdd) {
            if (!groups.Contains(marco)) {
                groups = groups + ";" + marco;

                hasChanged = true;
            }
        }

        if (hasChanged) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groups);
        }
    }

    public static void ChangeMarco(IList<string> toRemove, string toAdd) {
        ChangeMarco(toRemove, toAdd, EditorUserBuildSettings.selectedBuildTargetGroup);
    }

    public static void ChangeMarco(IList<string> toRemove, string toAdd, BuildTargetGroup group) {
        string groups = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        bool hasChanged = false;
        foreach (var marco in toRemove) {
            if (groups.Contains(marco)) {
                groups = groups.Replace(marco + ";", "");
                groups = groups.Replace(marco, "");

                hasChanged = true;
            }
        }

        if (!groups.Contains(toAdd)) {
            groups = groups + ";" + toAdd;

            hasChanged = true;
        }

        if (hasChanged) {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groups);
        }
    }
}
