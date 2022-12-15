using System;
using UnityEngine;

// #if UNITY_EDITOR
using UnityEditor;

// https://github.com/upscalebaby/generic-serializable-dictionary/blob/master/Assets/Editor/GenericDictionaryPropertyDrawer.cs
[CustomPropertyDrawer(typeof(DictionaryPropertyAttribute))]
public class DictionaryPropertyDrawer : PropertyDrawer {
    private static float lineHeight = EditorGUIUtility.singleLineHeight;
    private static float vertSpace = EditorGUIUtility.standardVerticalSpacing;
    private const float warningBoxHeight = 1.5f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Draw list of key/value pairs.
        var list = property.FindPropertyRelative("list");
        EditorGUI.PropertyField(position, list, label, true);

        // Draw key collision warning.
        var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision) {
            position.y += EditorGUI.GetPropertyHeight(list, true);
            if (!list.isExpanded) {
                position.y += vertSpace;
            }

            position.height = lineHeight * warningBoxHeight;
            position = EditorGUI.IndentedRect(position);
            EditorGUI.HelpBox(position, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        // Height of KeyValue list.
        float height = 0f;
        var list = property.FindPropertyRelative("list");
        height += EditorGUI.GetPropertyHeight(list, true);

        // Height of key collision warning.
        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision) {
            height += warningBoxHeight * lineHeight;
            if (!list.isExpanded) {
                height += vertSpace;
            }
        }

        return height;
    }
}
// #endif

// 字典的序列化
[AttributeUsage(AttributeTargets.Field)]
public class DictionaryPropertyAttribute : PropertyAttribute { }
