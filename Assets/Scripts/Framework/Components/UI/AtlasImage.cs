using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

// https://github.com/SylarLi/AtlaS
// https://github.com/mob-sakai/AtlasImage
[CanEditMultipleObjects]
[CustomEditor(typeof(AtlasImage))]
public class AtlasImageEditor : ImageEditor {
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
public class AtlasImage : Image {
    public SpriteAtlas atlas;

    public bool Contains() {
        return atlas != null && sprite != null && atlas.GetSprite(sprite.name);
    }
}
