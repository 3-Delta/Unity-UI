using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(TagPropertyAttribute))]
public class TagPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.String) {
            Debug.LogWarning("TagPropertyAttribute can only be applied on string properties/fields");
            return;
        }

        property.stringValue = EditorGUI.TagField(position, property.name, property.stringValue);
    }
}
#endif

// string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
// public string tag;
[AttributeUsage(AttributeTargets.Field)]
public class TagPropertyAttribute : PropertyAttribute {
}
