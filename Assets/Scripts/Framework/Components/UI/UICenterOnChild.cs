using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine.Serialization;

// Scrollrect,Viewport,Content中心点都是(0.5, 0.5)而且必须无anchor
[DisallowMultipleComponent]
public class UICenterOnChild : MonoBehaviour, IBeginDragHandler, IEndDragHandler {
    public Action<Transform> onCenter;

    // hOrV, index, transform, toMiddle
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
            if (midTransform == null) {
                return scrollRect.viewport.position;
            }

            return midTransform.position;
        }
    }

    public Vector3 MidOffset {
        get {
            // midTransform必须为viewPort的子节点
            if (midTransform != null) {
                return midTransform.localPosition;
            }

            return Vector3.zero;
        }
    }

    public float MaxDistance {
        get {
            if (childrenPos.Count <= 0) {
                return 0f;
            }
            else if (childrenPos.Count > 1) {
                return Mathf.Abs(childrenPos[0] - childrenPos[childrenPos.Count - 1]);
            }
            else {
                return 1f;
            }
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
        // 重建Layout, 方便后面位置计算
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
            childrenPos.Add(-childPos);
            for (int i = 1; i < pageCount; i++) {
                childPos -= contentGridLayout.cellSize.y + contentGridLayout.spacing.y;
                childrenPos.Add(-childPos);
            }
        }
    }

    private void UpdateScrollView() {
        var midPos = MidPosition;
        var m = scrollRect.content.InverseTransformPoint(midPos);
        if (scrollRect.horizontal) {
            for (int i = 0; i < childrenGos.Count; i++) {
                Transform trans = childrenGos[i].transform;
                var ch = scrollRect.content.InverseTransformPoint(trans.position);
                float toMiddle = Mathf.Abs(ch.x - m.x) / MaxDistance;
                UpdateItem(true, i, trans, toMiddle);
            }
        }
        else if (scrollRect.vertical) {
            for (int i = 0; i < childrenGos.Count; i++) {
                Transform trans = childrenGos[i].transform;
                var ch = scrollRect.content.InverseTransformPoint(trans.position);
                float toMiddle = Mathf.Abs(ch.y - m.y) / MaxDistance;
                UpdateItem(false, i, trans, toMiddle);
            }
        }
    }

    private void UpdateItem(bool hOrV, int index, Transform tr, float toMiddle) {
        var srCenterOnCentent = scrollRect.content.InverseTransformPoint(scrollRect.viewport.position);
        onTransform?.Invoke(hOrV, index, tr, toMiddle, srCenterOnCentent);
    }

#if UNITY_EDITOR
    [FormerlySerializedAs("closestIndex")] [SerializeField] private int dragingClosestIndex;
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
        this.dragingClosestIndex = childIndex;
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

        targetPagePosition = childrenPos[index];
        CenterOn(index, true);
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
            Debug.LogError("target: " + target);
            CenterOn(index, needTween);
        }
    }

    public void CenterOn(int index, bool needTween = false) {
        Debug.LogError("index: " + index);
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
