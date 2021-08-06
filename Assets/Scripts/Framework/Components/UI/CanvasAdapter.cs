using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class CanvasAdapter : MonoBehaviour {
    public Canvas canvas;
    public CanvasScaler canvasScaler;

    private RectTransform rectTrasnform;

    private void Awake() {
        if (canvas == null) {
            canvas = GetComponent<Canvas>();
        }

        if (canvasScaler == null) {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        if (!TryGetComponent<ResolutionAdjuster>(out ResolutionAdjuster _)) {
            gameObject.AddComponent<ResolutionAdjuster>();
        }

        if (!TryGetComponent<SafeAreaAdjuster>(out SafeAreaAdjuster _)) {
            gameObject.AddComponent<SafeAreaAdjuster>();
        }

        rectTrasnform = transform as RectTransform;
    }

    private void Start() {
        if (canvas != null) {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = CameraService.Camera2d;
        }

        if (canvasScaler != null) {
            canvasScaler.referenceResolution =
                new Vector2(GlobalSetting.ResolutionWidth, GlobalSetting.ResolutionHeight);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
        }

        rectTrasnform.anchoredPosition3D = Vector3.zero;
    }

    public void SetOrder(int order) {
        if (canvas != null) {
            canvas.sortingOrder = order;
        }
    }
}