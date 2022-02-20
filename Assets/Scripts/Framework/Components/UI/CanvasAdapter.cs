using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class CanvasAdapter : MonoBehaviour {
    private Canvas canvas;
    private GraphicRaycaster raycaster;
    
    private void Awake() {
        if (canvas == null) {
            canvas = GetComponent<Canvas>();
        }

        if (raycaster == null) {
            raycaster = GetComponent<GraphicRaycaster>();
        }

        if (!TryGetComponent<ResolutionAdjuster>(out ResolutionAdjuster _)) {
            gameObject.AddComponent<ResolutionAdjuster>();
        }

        if (!TryGetComponent<SafeAreaAdjuster>(out SafeAreaAdjuster _)) {
            gameObject.AddComponent<SafeAreaAdjuster>();
        }
    }

    private void Start() {
        if (canvas != null) {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = CameraService.Camera2d;
        }
    }

    public void SetOrder(int order) {
        if (canvas != null) {
            canvas.sortingOrder = order;
        }
    }

    public void BlockRaycaster(bool toBlock) {
        raycaster.enabled = toBlock;
    }
}
