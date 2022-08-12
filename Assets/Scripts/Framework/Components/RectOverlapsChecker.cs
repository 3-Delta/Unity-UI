using System;
using UnityEngine;

// 可以用于检测：Mask是否与某个rect有重叠， 某些情况下，首次交叠的时候需要做一些初始化工作
[DisallowMultipleComponent]
public class RectOverlapsChecker : MonoBehaviour {
    public RectTransform left;
    public RectTransform right;
    public bool checkWhenStart = true;

    // true 表示接触， false 表示不接触
    public Action<bool> onOverlapChanged;

    private bool _defaultFlag = false;

    private void Start() {
        if (checkWhenStart) {
            DefaultCheck();
        }
    }

    private void Update() {
        bool newFlag = Check();
        if (newFlag != _defaultFlag) {
            _defaultFlag = newFlag;
            _OnOverlapChanged();
        }
    }

    public void DefaultCheck() {
        _defaultFlag = Check();
        _OnOverlapChanged();
    }

    public bool Check() {
        if (GetWorldRect(left, out Rect leftRect) && GetWorldRect(right, out Rect rightRect)) {
            return leftRect.Overlaps(rightRect);
        }
        else {
            return false;
        }
    }

    private void _OnOverlapChanged() {
        // Debug.LogError("_defaultFlag:" + _defaultFlag);
        onOverlapChanged?.Invoke(_defaultFlag);
    }

    private static readonly Vector3[] corners = new Vector3[4];

    public static bool GetWorldRect(RectTransform rt, out Rect rect) {
        if (rt != null) {
            rt.GetWorldCorners(corners);
            float x = corners[0].x;
            float y = corners[0].y;
            float w = corners[2].x - corners[0].x;
            float h = corners[1].y - corners[0].y;
            rect = new Rect(x, y, w, h);
            return true;
        }
        else {
            rect = default;
            return false;
        }
    }
}