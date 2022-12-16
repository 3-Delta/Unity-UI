using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(FlagsEnumPropertyAttribute))]
public class FlagsEnumPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        
        // 另外可以参考：https://github.com/dbrizov/NaughtyAttributes
    }
}
#endif

// 在枚举的field是附属，需要定义枚举为[System.Flags]
// 即第一个枚举一定要是1，而不是0
[AttributeUsage(AttributeTargets.Field)]
public class FlagsEnumPropertyAttribute : PropertyAttribute {
}
