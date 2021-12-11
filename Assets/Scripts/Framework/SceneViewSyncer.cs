using System;
using UnityEngine;

// https://blog.uwa4d.com/archives/USparkle_Continuous-optimization.html
// https://weibo.com/819881121?layerid=4631050959719366
#if UNITY_EDITOR
using UnityEditor;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class SceneViewSyncer : MonoBehaviour {
    public Camera camera;
    private SceneView view = null;

    private void Awake() {
        view = SceneView.lastActiveSceneView;
        TryGetComponent(out camera);
    }

    private void LateUpdate() {
        if (view != null) {
            view.cameraSettings.nearClip = camera.nearClipPlane;
            view.cameraSettings.farClip = camera.farClipPlane;
            view.cameraSettings.fieldOfView = camera.fieldOfView;

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
