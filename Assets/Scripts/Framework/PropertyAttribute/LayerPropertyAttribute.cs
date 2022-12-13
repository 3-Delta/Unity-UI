using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(LayerPropertyAttribute))]
public class LayerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (property.propertyType != SerializedPropertyType.Integer) {
            Debug.LogWarning("LayerPropertyAttribute can only be applied on integer properties/fields");
            return;
        }

        property.intValue = EditorGUI.LayerField(position, property.name, property.intValue);
    }
}
#endif

// public int layer;
[AttributeUsage(AttributeTargets.Field)]
public class LayerPropertyAttribute : PropertyAttribute {
}