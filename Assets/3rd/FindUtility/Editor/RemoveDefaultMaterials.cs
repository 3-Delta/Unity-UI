using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// https://networm.me/2019/09/08/unity-remove-default-material/
public class RemoveDefaultMaterials : Editor
{
    [MenuItem("Tools/Misc/Reimport All Model")]
    public static void ReimportAllModel()
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        Array.Sort(assetPaths);
        Debug.LogWarning(string.Format("Total assets count: {0}", assetPaths.Length));
        int processedCount = 0;

        foreach (string assetPath in assetPaths)
        {
            string normalizedAssetPath = assetPath.ToLower();
            if (!normalizedAssetPath.EndsWith(".fbx") &&
                !normalizedAssetPath.EndsWith(".obj") &&
                !normalizedAssetPath.EndsWith(".3ds"))
            {
                continue;
            }

            var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (modelImporter == null || modelImporter.importMaterials)
            {
                continue;
            }

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
            Debug.Log(assetPath, AssetDatabase.LoadMainAssetAtPath(assetPath));
            processedCount++;
        }

        Debug.LogWarning(string.Format("Total processed model count: {0}", processedCount));
    }

    private void OnPostprocessModel(AssetImporter assetImporter, GameObject model)
    {
        var modelImporter = assetImporter as ModelImporter;
        if (modelImporter == null || modelImporter.importMaterials)
        {
            return;
        }

        var renderers = model.GetComponentsInChildren<Renderer>();
        if (renderers == null)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            renderer.sharedMaterials = new Material[0];
        }
    }
}
