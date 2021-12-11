using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

// https://github.com/SylarLi/AtlaS
// https://github.com/mob-sakai/AtlasImage
// ugui的image不能明确的知道sprite是否来自atlas, 可以仿照ngui实现一套
[CanEditMultipleObjects]
[CustomEditor(typeof(ImageAtlas))]
public class ImageAtlasEditor : ImageEditor {
    private SerializedProperty atlasProperty;

    private GUIContent atlasContent = new GUIContent("Atlas");

    protected override void OnEnable() {
        base.OnEnable();

        atlasProperty = serializedObject.FindProperty("atlas");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(atlasProperty, atlasContent);
        atlasProperty.serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

[DisallowMultipleComponent]
public class ImageAtlas : Image {
    public SpriteAtlas atlas;

    public bool Contains() {
        return atlas != null && sprite != null && atlas.GetSprite(sprite.name);
    }
}
