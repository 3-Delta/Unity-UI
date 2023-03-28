using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
#endif

[DisallowMultipleComponent]
public class ActiveRecorder : MonoBehaviour {
#if UNITY_EDITOR
    private void Reset() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    public Dictionary<Transform, bool> status = new Dictionary<Transform, bool>();

    public void Record() {
        this.status.Clear();
        foreach (var ch in this.GetComponentsInChildren<Transform>(true)) {
            this.status[ch] = ch.gameObject.activeSelf;
        }
    }

    public void UnRecord() {
        foreach (var ch in this.GetComponentsInChildren<Transform>(true)) {
            if (this.status.TryGetValue(ch, out var flag)) {
                ch.gameObject.SetActive(flag);
            }
        }
    }

    public void Print() {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var kvp in status) {
            string path = GetPath(kvp.Key);
            stringBuilder.AppendFormat($"{path} -> {kvp.Value}");
            stringBuilder.AppendLine();
        }

        Debug.LogError(stringBuilder.ToString());
    }

    public static string GetPath(Component component, Component end = null) {
        if (component == null) {
            return null;
        }

        string totalPath = null;
        List<string> paths = new List<string>();
        if (end == null) {
            var cp = component;
            while (cp.transform.parent != null) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }
        else {
            var cp = component;
            while (cp.gameObject != end.gameObject) {
                paths.Add(cp.transform.name);
                cp = cp.transform.parent;
            }
        }

        if (paths.Count > 1) {
            paths.Reverse();
            totalPath = string.Join("/", paths);
        }
        else if (paths.Count > 0) {
            totalPath = paths[0];
        }

        return totalPath;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ActiveRecorder))]
public class ActiveRecorderInspector : Editor {
    private ActiveRecorder owner;

    private void OnEnable() {
        owner = target as ActiveRecorder;
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        if (GUILayout.Button("记录")) {
            owner.Record();
        }

        if (GUILayout.Button("还原")) {
            owner.UnRecord();
        }

        if (GUILayout.Button("打印")) {
            owner.Print();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
