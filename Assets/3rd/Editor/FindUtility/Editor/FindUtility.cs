using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

public class FindUtility : EditorWindow {
    [MenuItem("Tools/FindUtility")]
    public static void ShowWindow() {
        var window = GetWindow(typeof(FindUtility));
        window.minSize = new Vector2(400f, 400f);
        window.Show();
    }

    private void OnGUI() {
        GUILayout.Box("Asset");
        _assetPathText = EditorGUILayout.TextArea(_assetPathText, GUILayout.MinHeight(40f));
        if (GUILayout.Button("Convert to GUID")) {
            string[] assetPaths = _assetPathText.Split(StringSeparators, StringSplitOptions.RemoveEmptyEntries);
            OutputAssetPaths(assetPaths, null, null);
        }

        GUILayout.Space(20f);
        GUILayout.Box("GUID");
        _guidText = EditorGUILayout.TextArea(_guidText, GUILayout.MinHeight(40f));
        if (GUILayout.Button("Convert to AssetPath")) {
            string[] guids = _guidText.Split(StringSeparators, StringSplitOptions.RemoveEmptyEntries);
            string[] assetPaths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++) {
                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            }

            OutputAssetPaths(assetPaths, null, null);
        }

        GUILayout.Space(20f);
        GUILayout.Box("Shader");
        _shaderText = EditorGUILayout.TextArea(_shaderText, GUILayout.MinHeight(40f));
        if (GUILayout.Button("Find")) {
            string[] shaders = _shaderText.Split(StringSeparators, StringSplitOptions.RemoveEmptyEntries);
            string[] assetPaths = new string[shaders.Length];
            for (int i = 0; i < shaders.Length; i++) {
                var shader = Shader.Find(shaders[i]);
                if (shader != null) {
                    assetPaths[i] = AssetDatabase.GetAssetPath(shader);
                }
                else {
                    assetPaths[i] = null;
                }
            }

            OutputAssetPaths(assetPaths, shaders, "Shader");
        }
    }

    private void OutputAssetPaths(string[] assetPaths, string[] extraInfos, string extraHeader) {
        var selected = new List<Object>(assetPaths.Length);
        for (int i = 0; i < assetPaths.Length; i++) {
            string assetPath = assetPaths[i];
            if (string.IsNullOrEmpty(assetPath)) {
                continue;
            }

            if (assetPath.EndsWith(MetaExtension)) {
                assetPath = assetPath.Substring(0, assetPath.Length - MetaExtension.Length);
            }

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            if (string.IsNullOrEmpty(guid)) {
                Debug.LogError(string.Format("{0} didn't exist", assetPath));
                continue;
            }

            var asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            selected.Add(asset);
            if (string.IsNullOrEmpty(extraHeader)) {
                Debug.Log(string.Format("{0} | GUID: {1}", assetPath, guid), asset);
            }
            else {
                Debug.Log(string.Format("{0} | GUID: {1} | {2}: {3}", assetPath, guid, extraHeader, extraInfos[i]),
                    asset);
            }
        }

        Selection.objects = selected.ToArray();
    }

    private const string MetaExtension = ".meta";
    private readonly char[] StringSeparators = { '\r', '\n' };
    private string _assetPathText;
    private string _guidText;
    private string _shaderText;
}