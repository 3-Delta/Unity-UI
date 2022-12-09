using System.IO;
using UnityEditor;
using UnityEngine;

// 编译之后立即拷贝dll
public class HotfixCopyer {
    // [InitializeOnLoadMethod]
    // private static void Copy() {
    //     if (!Directory.Exists(HotfixSettings.AssemblyFolderPath)) {
    //         Directory.CreateDirectory(HotfixSettings.AssemblyFolderPath);
    //     }
    //
    //     // Copy DLL
    //     string dllSource = Path.Combine(HotfixSettings.ScriptAssembliesDir, HotfixSettings.HotfixDLLFileName);
    //     string dllDest = Path.Combine(HotfixSettings.AssemblyFolderPath, $"{HotfixSettings.HotfixDLLFileName}.bytes");
    //     File.Copy(dllSource, dllDest, true);
    //
    //     // Copy PDB
    //     string pdbSource = Path.Combine(HotfixSettings.ScriptAssembliesDir, HotfixSettings.HotfixPDBFileName);
    //     string pdbDest = Path.Combine(HotfixSettings.AssemblyFolderPath, $"{HotfixSettings.HotfixPDBFileName}.bytes");
    //     File.Copy(pdbSource, pdbDest, true);
    //
    //     Debug.Log("Copy hotfix assembly files done.");
    //     AssetDatabase.Refresh();
    // }
}
