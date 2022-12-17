using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

// 需要参考Unity默认的数组是怎么序列化的
[CustomPropertyDrawer(typeof(EnumListPropertyAttribute))]
public class EnumListPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // EnumListPropertyAttribute enumList = (EnumListPropertyAttribute)this.attribute;
        // bool isArray = fieldInfo.FieldType.IsArray;
        //
        // if (DrawHeader("Array", "nodes", false, false)) {
        //     var enumNames = Enum.GetNames(enumList.enumType);
        //     for (int i = 0; i < enumNames.Length; i++) {
        //         string key = enumNames[i];
        //         SerializedProperty p = property.serializedObject.FindProperty(string.Format("{0}.Array.data[{1}]", "crr", i));
        //         EditorGUILayout.PropertyField(p, new GUIContent(key));
        //     }
        // }
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

[AttributeUsage(AttributeTargets.Field)]
public class EnumListPropertyAttribute : PropertyAttribute {
    public Type enumType;
    public Type valueType;

    public EnumListPropertyAttribute(Type enumType, Type valueType) {
        this.enumType = enumType;
        this.valueType = valueType;
    }
}
