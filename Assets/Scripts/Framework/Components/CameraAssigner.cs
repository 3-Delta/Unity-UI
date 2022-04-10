using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum ECameraType {
    Camera2d,
    Camera3d,
    Camera2dTop,
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class CameraAssigner : MonoBehaviour {
    public ECameraType cameraType = ECameraType.Camera2d;
    [SerializeField] private Camera targetCamera;

    private void Awake() {
        if (targetCamera == null) {
            targetCamera = GetComponent<Camera>();
        }

        if (cameraType == ECameraType.Camera2d) {
            CameraService.Camera2d = targetCamera;
        }
    }

    private void OnEnable() {
        if (cameraType == ECameraType.Camera3d) {
            CameraService.CurrentCamera3d = targetCamera;
        }
    }

    private void OnDisable() {
        if (cameraType == ECameraType.Camera3d) {
            CameraService.CurrentCamera3d = null;
        }
    }
}
