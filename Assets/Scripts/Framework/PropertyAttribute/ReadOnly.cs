using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyPropertyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer {
    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    //     return EditorGUI.GetPropertyHeight(property, label, true);
    // }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyPropertyAttribute : PropertyAttribute { }
