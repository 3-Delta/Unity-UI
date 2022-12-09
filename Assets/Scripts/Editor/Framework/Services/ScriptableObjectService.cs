using UnityEditor;
using System.IO;

using UnityEngine;

public class ScriptableObjectService {
    public static T LoadAsset<T>(string path) where T : ScriptableObject {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null) {
            asset = CreateAsset<T>(path);
        }

        return asset;
    }

    public static T CreateAsset<T>(string path) where T : ScriptableObject {
        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.Refresh();

        return asset;
    }

    public static T LoadAsset<T>() where T : ScriptableObject {
        var guilds = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var guild in guilds) {
            var assetPath = AssetDatabase.GUIDToAssetPath(guild);
            if (string.IsNullOrEmpty(assetPath)) {
                continue;
            }

            var t = LoadAsset<T>(assetPath);
            if (t == null) {
                continue;
            }

            return t;
        }

        return null;
    }
}
