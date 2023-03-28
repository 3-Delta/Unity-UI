using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

/*
using DG.Tweening;

[DisallowMultipleComponent]
public class UICenterOnChild : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
    public Action<Transform> onCenter;

    // hOrV, index, transform, toMiddleRate
    public Action<bool, int, Transform, float, Vector3> onTransform;

    public float smoothTime = 0.5f;
    public ScrollRect scrollRect; // 卷轴矩阵(设置Pivot(0.5f,0.5f))
    public GridLayoutGroup contentGridLayout;
    public Transform midTransform;

    public readonly List<Transform> childrenGos = new List<Transform>();
    public readonly List<float> childrenPos = new List<float>();

#if UNITY_EDITOR
    [SerializeField] private int currentPageIndex;
#else
    public int currentPageIndex { get; private set; }
#endif

    public int pageCount { get; private set; }

    public Vector3 MidPosition {
        get {
            if (this.midTransform == null) {
                return this.scrollRect.viewport.position;
            }

            return this.midTransform.position;
        }
    }

    public Vector3 MidOffset {
        get {
            // midTransform必须为viewPort的子节点
            if (this.midTransform != null) {
                return midTransform.localPosition;
            }

            return Vector3.zero;
        }
    }

    public float MaxDistance {
        get {
            if (this.childrenPos.Count <= 0) {
                return 0f;
            }

            return Mathf.Abs(this.childrenPos[0] - this.childrenPos[this.childrenPos.Count - 1]);
        }
    }

    private float targetPagePosition = 0f; // 目标页签坐标
    private Tweener tweener; // Dotween动画

    private void Awake() {
        contentGridLayout = scrollRect.content.GetComponent<GridLayoutGroup>();

        // 限制惯性
        scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        scrollRect.inertia = false;
    }

    private void Update() {
        UpdateScrollView();
    }

    // 外部构建所有子节点之后调用居中
    [ContextMenu(nameof(Rebuild))]
    public void Rebuild() {
        childrenGos.Clear();
        childrenPos.Clear();

        // 重建Layout, 方便计算位置
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        foreach (Transform item in scrollRect.content) {
            if (item.gameObject.activeSelf && !childrenGos.Contains(item)) {
                childrenGos.Add(item);
            }
        }

        pageCount = childrenGos.Count;
        if (scrollRect.horizontal) {
            float childPos = scrollRect.content.rect.width * 0.5f - contentGridLayout.cellSize.x * 0.5f;
            childPos += MidOffset.x;
            childrenPos.Add(childPos);
            for (int i = 1; i < pageCount; i++) {
                childPos -= contentGridLayout.cellSize.x + contentGridLayout.spacing.x;
                childrenPos.Add(childPos);
            }
        }
        else if (scrollRect.vertical) {
            float childPos = scrollRect.content.rect.height * 0.5f - contentGridLayout.cellSize.y * 0.5f;
            childPos += MidOffset.y;
            childrenPos.Add(childPos);
            for (int i = 0; i < pageCount; i++) {
                childPos -= contentGridLayout.cellSize.y + contentGridLayout.spacing.y;
                childrenPos.Add(childPos);
            }
        }
    }

    private void UpdateScrollView() {
        var midPos = MidPosition;
        var m = this.scrollRect.content.InverseTransformPoint(midPos);
        if (scrollRect.horizontal) {
            for (int i = 0; i < childrenGos.Count; i++) {
                Transform trans = childrenGos[i].transform;
                var ch = this.scrollRect.content.InverseTransformPoint(trans.position);
                float toMiddleRate = Mathf.Abs(ch.x - m.x) / MaxDistance;
                UpdateItem(true, i, trans, toMiddleRate);
            }
        }
        else if (scrollRect.vertical) {
            for (int i = 0; i < childrenGos.Count; i++) {
                Transform trans = childrenGos[i].transform;
                var ch = this.scrollRect.content.InverseTransformPoint(trans.position);
                float toMiddleRate = Mathf.Abs(ch.y - m.y) / MaxDistance;
                UpdateItem(false, i, trans, toMiddleRate);
            }
        }
    }

    private void UpdateItem(bool hOrV, int index, Transform tr, float toMiddleRate) {
        var srCenterOnCentent = scrollRect.viewport.InverseTransformPoint(scrollRect.transform.position);
        onTransform?.Invoke(hOrV, index, tr, toMiddleRate, srCenterOnCentent);
    }

#if UNITY_EDITOR
    [SerializeField] private int closestIndex;
#endif
    private int FindClosestPos(float currentPos) {
        int childIndex = 0;
        float closest = 0;
        float distance = Mathf.Infinity;
        for (int i = 0; i < childrenPos.Count; i++) {
            float p = childrenPos[i];
            float d = Mathf.Abs(p - currentPos);
            if (d < distance) {
                distance = d;
                closest = p;
                childIndex = i;
            }
        }

#if UNITY_EDITOR
        closestIndex = childIndex;
#endif
        return childIndex;
    }

    public void LerpTweenToTarget(float targetValue, bool needTween = false) {
        Vector3 v = scrollRect.content.anchoredPosition3D;
        if (scrollRect.horizontal) {
            v.x = targetValue;
        }
        else if (scrollRect.vertical) {
            v.y = targetValue;
        }

        if (!needTween) {
            scrollRect.content.anchoredPosition3D = v;
        }
        else {
            tweener = scrollRect.content.DOLocalMove(v, smoothTime, true);
        }
    }

#region 响应事件
    public void OnBeginDrag(PointerEventData eventData) {
        tweener?.Pause();
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (childrenGos.Count <= 0) {
            return;
        }

        int index = 0;
        if (scrollRect.horizontal) {
            index = FindClosestPos(scrollRect.content.anchoredPosition3D.x);
        }
        else if (scrollRect.vertical) {
            index = FindClosestPos(scrollRect.content.anchoredPosition3D.y);
        }

        targetPagePosition = this.childrenPos[index];
        this.CenterOn(index, true);
    }
#endregion

#region 功能提供
    [ContextMenu(nameof(ToLeft))]
    public void ToLeft(bool needTween = false) {
        if (childrenGos.Count <= 0) {
            return;
        }

        if (currentPageIndex > 0) {
            currentPageIndex = currentPageIndex - 1;
            targetPagePosition = childrenPos[currentPageIndex];
            LerpTweenToTarget(targetPagePosition, needTween);
        }
    }

    [ContextMenu(nameof(ToRight))]
    public void ToRight(bool needTween = false) {
        if (childrenGos.Count <= 0) {
            return;
        }

        if (currentPageIndex < pageCount - 1) {
            currentPageIndex = currentPageIndex + 1;
            targetPagePosition = childrenPos[currentPageIndex];
            LerpTweenToTarget(targetPagePosition, needTween);
        }
    }

    [ContextMenu(nameof(CenterOn))]
    public void CenterOn() {
        CenterOn(currentPageIndex, true);
    }

    public void CenterOn(Transform target, bool needTween = false) {
        int index = childrenGos.IndexOf(target);
        if (index != -1) {
            CenterOn(index, needTween);
        }
    }

    public void CenterOn(int index, bool needTween = false) {
        if (childrenGos.Count <= 0) {
            return;
        }

        currentPageIndex = index;
        targetPagePosition = childrenPos[currentPageIndex];

        var centerChild = childrenGos[currentPageIndex];
        onCenter?.Invoke(centerChild);
        LerpTweenToTarget(targetPagePosition, needTween);
    }
#endregion
}
*/

public class CenterOnAdjustItem {
    public bool useScale;
    public float scaleMultiply = 1f;

    public bool usePosition;
    public float positionMultiply = 1f;
    public float positionOffset = 0f;

    protected virtual void OnAdjust(bool hOrV, int index, Transform t, float toMiddleRate, Vector3 scCenterOnContent) {
        if (this.useScale) {
            float scale = Mathf.Clamp01(1 - toMiddleRate) * scaleMultiply;
            t.localScale = new Vector3(scale, scale, scale);
        }

        if (this.usePosition) {
            var oldPos = t.localPosition;
            if (hOrV) {
                t.localPosition = new Vector3(oldPos.x, scCenterOnContent.y + toMiddleRate * positionMultiply + this.positionOffset, oldPos.z);
            }
            else {
                t.localPosition = new Vector3(scCenterOnContent.x + toMiddleRate * positionMultiply + this.positionOffset, oldPos.y, oldPos.z);
            }
        }
    }
}
