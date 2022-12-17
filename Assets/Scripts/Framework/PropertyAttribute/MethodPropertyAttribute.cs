using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;

// DefaultMethodDrawer.cs
// 本来是想参考Odin的实现，但是太复杂，而且odin是dll的形式
// 知道Odin有对于Method的特性去序列化方法，就是[Button],F12点进去找到所属的dll位置，用DnSpy打开，然后DnSpy导出为一个C#工程。Dnspy中会自动处理关联
// 的其他程序集，比如UnityEngine但是导出的vs工程没有这些程序集，需要手动引用。
[CustomPropertyDrawer(typeof(MethodPropertyAttribute))]
public class ButtonPropertyDrawer : PropertyDrawer {
    private float buttonHeight = 20f;
    private GUIStyle style = EditorStyles.miniButton;

    private bool drawParameters = true;
    private bool expanded = false;

    private MethodInfo methodInfo;
    private bool hasInvokedOnce;
    // private ActionResolver buttonActionResolver;
    // private ValueResolver<object> buttonValueResolver;

    public bool hasInited = false;

    public void Init(Rect position, SerializedProperty property, GUIContent label) {
        
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (!this.hasInited) {
            this.Init(position, property, label);
            this.hasInited = true;
        }

        var a = attribute;
        var f = fieldInfo;
        
        // DrawPropertyLayout(position, property, label);
    }

    // public void DrawPropertyLayout(Rect position, SerializedProperty property, GUIContent label) {
    //     if (!this.drawParameters) {
    //         this.DrawNormalButton(position, property, label);
    //     }
    //     else {
    //         DrawCompactBoxButton(position, property, label);
    //     }
    // }
    //
    // private void DrawNormalButton(Rect position, SerializedProperty property, GUIContent label) {
    //     Rect btnRect = (this.buttonHeight > 0f) ? GUILayoutUtility.GetRect(GUIContent.none, this.style, GUILayout.Height(this.buttonHeight)) : GUILayoutUtility.GetRect(GUIContent.none, this.style);
    //     btnRect = EditorGUI.IndentedRect(btnRect);
    //
    //     Color tmp = GUI.color;
    //     if (GUI.Button(btnRect, label, this.style)) {
    //         this.InvokeButton(position, property, label);
    //     }
    //
    //     GUI.color = tmp;
    // }

    // private void DrawCompactBoxButton(Rect position, SerializedProperty property, GUIContent label) {
    //     SirenixEditorGUI.BeginBox(Array.Empty<GUILayoutOption>());
    //     Rect rect = SirenixEditorGUI.BeginToolbarBoxHeader(22f).AlignRight(70f).Padding(1f);
    //     rect.height -= 1f;
    //     if (GUI.Button(rect, "Invoke")) {
    //         this.InvokeButton(position, property, label);
    //     }
    //     
    //     if (this.expanded) {
    //         EditorGUILayout.LabelField(label, Array.Empty<GUILayoutOption>());
    //     }
    //     else {
    //         this.expanded = SirenixEditorGUI.Foldout(this.expanded, label, null);
    //     }
    //
    //     SirenixEditorGUI.EndToolbarBoxHeader();
    //     this.DrawParameters(position, property, label, false);
    //     SirenixEditorGUI.EndToolbarBox();
    // }
    //
    // private void DrawParameters(Rect position, SerializedProperty property, GUIContent label, bool appendButton) {
    //     if (SirenixEditorGUI.BeginFadeGroup(this, this.expanded)) {
    //         GUILayout.Space(0f);
    //         // childcount需要排除返回值，也就是数组中最后一个值
    //         for (int i = 0; i < base.Property.Children.Count; i++) {
    //             base.Property.Children[i].Draw();
    //         }
    //
    //         if (appendButton) {
    //             Rect rect = EditorGUILayout.BeginVertical(SirenixGUIStyles.BottomBoxPadding, Array.Empty<GUILayoutOption>()).Expand(3f);
    //             SirenixEditorGUI.DrawHorizontalLineSeperator(rect.x, rect.y, rect.width, 0.5f);
    //             this.DrawNormalButton(position, property, label);
    //             EditorGUILayout.EndVertical();
    //         }
    //     }
    //
    //     SirenixEditorGUI.EndFadeGroup();
    // }
    //
    // private void InvokeButton(Rect position, SerializedProperty property, GUIContent label) {
    //     try {
    //         GUIHelper.RemoveFocusControl();
    //         GUIHelper.RequestRepaint();
    //         if (((base.Property.Info.GetMemberInfo() as MethodInfo) ?? base.Property.Info.GetMethodDelegate().Method).IsGenericMethodDefinition) {
    //             Debug.LogError("Cannot invoke a generic method definition.");
    //         }
    //         else {
    //             if (this.attribute == null || this.attribute.DirtyOnClick) {
    //                 if (base.Property.ParentValueProperty != null) {
    //                     base.Property.ParentValueProperty.RecordForUndo("Clicked Button '" + base.Property.NiceName + "'", true);
    //                 }
    //
    //                 foreach (UnityEngine.Object target in base.Property.SerializationRoot.ValueEntry.WeakValues.OfType<UnityEngine.Object>()) {
    //                     InspectorUtilities.RegisterUnityObjectDirty(target);
    //                 }
    //             }
    //
    //             MethodInfo methodInfo = (MethodInfo)property..se.Info.GetMemberInfo();
    //             if (methodInfo != null) {
    //                 this.InvokeMethodInfo(position, property, label, methodInfo);
    //             }
    //         }
    //     }
    //     finally {
    //         GUIUtility.ExitGUI();
    //     }
    // }
    //
    // private void InvokeMethodInfo(Rect position, SerializedProperty property, GUIContent label, MethodInfo methodInfo) {
    //     InspectorProperty parentValueProperty = base.Property.ParentValueProperty;
    //     ImmutableList targets = base.Property.ParentValues;
    //     int argCount = this.hasReturnValue ? (base.Property.Children.Count - 1) : base.Property.Children.Count;
    //     for (int i = 0; i < targets.Count; i++) {
    //         object value = targets[i];
    //         if (value != null || methodInfo.IsStatic) {
    //             try {
    //                 object[] arguments = new object[argCount];
    //                 for (int j = 0; j < arguments.Length; j++) {
    //                     arguments[j] = base.Property.Children[j].ValueEntry.WeakSmartValue;
    //                 }
    //
    //                 object result;
    //                 if (methodInfo.IsStatic) {
    //                     result = methodInfo.Invoke(null, arguments);
    //                 }
    //                 else {
    //                     result = methodInfo.Invoke(value, arguments);
    //                 }
    //
    //                 for (int k = 0; k < arguments.Length; k++) {
    //                     base.Property.Children[k].ValueEntry.WeakSmartValue = arguments[k];
    //                 }
    //             }
    //             catch (ExitGUIException ex) {
    //                 Debug.LogException(ex);
    //             }
    //
    //             if (parentValueProperty != null && value.GetType().IsValueType) {
    //                 parentValueProperty.ValueEntry.WeakValues[i] = value;
    //             }
    //         }
    //     }
    // }
}
#endif

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class MethodPropertyAttribute : Attribute { }
