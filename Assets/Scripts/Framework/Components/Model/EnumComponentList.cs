using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class EnumComponentListInspector<T_CP, T_Enum> : Editor
    where T_CP : Component
    where T_Enum : Enum {

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EnumComponentList<T_CP, T_Enum> node = target as EnumComponentList<T_CP, T_Enum>;
        T_CP[] cps = node.nodes;

        if (GUILayout.Button("FindAll")) {
            node.FindAll();
        }

        if (DrawHeader("Nodes", "Nodes", false, false)) {
            for (int i = 0; i < cps.Length; i++) {

                string key = Enum.GetName(typeof(T_Enum), i);
                SerializedProperty p = serializedObject.FindProperty(string.Format("{0}.Array.data[{1}]", "nodes", i));
                EditorGUILayout.PropertyField(p, new GUIContent(key));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic) {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic) {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }
}
#endif

[DisallowMultipleComponent]
public class EnumComponentList<T_CP, T_Enum> : MonoBehaviour
    where T_CP : Component
    where T_Enum : Enum {

    public T_CP[] nodes;

    public virtual void FindAll() {
    }
}