using UnityEditor;

using UnityEngine;

// https://blog.uwa4d.com/archives/USparkle_Continuous-optimization.html
#if UNITY_EDITOR
public class SceneViewSyncer : MonoBehaviour {
    private SceneView view = null;

    private void Awake() {
        view = SceneView.lastActiveSceneView;
    }

    private void LateUpdate() {
        if (view != null) {
            view.LookAt(transform.position, transform.rotation, 0f);
        }
    }

    private void OnDestroy() {
        if (view != null) {
            view.LookAt(transform.position, transform.rotation, 5f);
        }
    }
}
#endif
