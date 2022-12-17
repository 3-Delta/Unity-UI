using System.IO;

#if UNITY_EDITOR
using UnityEditor;

// https://github.com/insthync/UnityEditorUtils/blob/f0cd0b05f2abcf24d3ec4cedba03b7caa64abae0/Editor/UnityScenePropertyDrawer.cs
// public SceneAsset sceneAsset;
public class SceneAssetUtils {
    public static SceneAsset GetSceneObject(string sceneName) {
        if (string.IsNullOrEmpty(sceneName)) {
            return null;
        }

        foreach (var editorScene in EditorBuildSettings.scenes) {
            var sceneNameWithoutExtension = Path.GetFileNameWithoutExtension(editorScene.path);
            if (sceneNameWithoutExtension == sceneName) {
                return AssetDatabase.LoadAssetAtPath(editorScene.path, typeof(SceneAsset)) as SceneAsset;
            }
        }

        return null;
    }
}
#endif
