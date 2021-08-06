using System;
using UnityEngine;

// 动态调整分辨率导致分辨率或者安全区域发生变化，比如折叠屏
[DisallowMultipleComponent]
public class ResolutionTracer : MonoBehaviour {
    private int lastWidth = Screen.width;
    private int lastHeight = Screen.height;
    public Action<int, int> onResolutionChanged;

    private Rect lastSafeArea;
    public Action<Rect> onSafeAreaChanged;

    private void Awake() {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        lastSafeArea = Screen.safeArea;
    }

    private void Update() {
        var width = Screen.width;
        var height = Screen.height;

        if (width != lastWidth || height != lastHeight) {
            onResolutionChanged?.Invoke(width, height);

            lastWidth = width;
            lastHeight = height;
        }

        var cur = Screen.safeArea;
        if (!cur.Equals(lastSafeArea)) {
            onSafeAreaChanged?.Invoke(cur);

            lastSafeArea = cur;
        }
    }
}