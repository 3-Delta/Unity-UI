using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

#if UNITY_EDITOR
using UnityEditor;

// 要求枚举数值必须从0开始，并且+1的关系
public class EnumComponentListInspector<T_CP, T_Enum> : Editor where T_CP : Component where T_Enum : Enum {
    private EnumComponentList<T_CP, T_Enum> node;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        node = target as EnumComponentList<T_CP, T_Enum>;
        T_CP[] cps = node.nodes;

        if (GUILayout.Button("FindAll")) {
            node.FindAll();
        }

        if (DrawHeader("Array", "nodes", false, false)) {
            for (int i = 0; i < cps.Length; i++) {
                SerializedProperty p = serializedObject.FindProperty(string.Format("{0}.Array.data[{1}]", "nodes", i));
                string key = Enum.GetName(typeof(T_Enum), i);

                EditorGUILayout.PropertyField(p, new GUIContent(key));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static bool DrawHeader(string text, string key, bool forceOn, bool minimalistic) {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) {
            GUILayout.Space(3f);
        }

        if (!forceOn && !state) {
            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        }

        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic) {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) {
                state = !state;
            }

            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
                state = !state;
            }
        }

        if (GUI.changed) {
            EditorPrefs.SetBool(key, state);
        }

        if (!minimalistic) {
            GUILayout.Space(2f);
        }

        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) {
            GUILayout.Space(3f);
        }

        return state;
    }
}
#endif

[DisallowMultipleComponent]
public class EnumComponentList<T_CP, T_Enum> : MonoBehaviour where T_CP : Component where T_Enum : Enum {
    // 要求枚举数值必须从0开始，并且+1的关系
    public T_CP[] nodes = new T_CP[Enum.GetNames(typeof(T_Enum)).Length];

    public T_CP Get(T_Enum target) {
        // enum的hashcode就是具体数值
        // enum.getvalues其实返回的是升序的结构，因为我们一般定义枚举就是升序定义，所以内部使用插入排序进行升序排序，效率高一些
        return this.nodes[(target as Enum).GetHashCode()];
    }

    public virtual void FindAll() {
        // 编辑器下一键根据名字查找
    }
}
