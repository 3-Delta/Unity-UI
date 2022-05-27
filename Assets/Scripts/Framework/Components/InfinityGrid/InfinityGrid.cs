using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

[Serializable]
public class InfinityGridCell {
    public int index = -1;
    public RectTransform bindGo;
    // 参数
    public object userData;

    public bool bDirty { get; private set; }
    
    internal void SetDirty(bool v) {
        this.bDirty = v;
    }

    internal void SetIndex(int index) {
        if (this.index != index) {
            this.bDirty = true;
            this.index = index;
#if UNITY_EDITOR
            this.bindGo.name = index.ToString();
#endif
        }
    }

    public void BindGameObject(GameObject go) {
        this.bindGo = go.transform as RectTransform;
    }

    public void BindUserData(object userDta) {
        this.userData = userDta;
    }
}

[RequireComponent(typeof(ScrollRect))]
[DisallowMultipleComponent]
public class InfinityGrid : MonoBehaviour {
    public ScrollRect _scrollview = null;
    public RectTransform _content = null;
    public GameObject _proto = null;
    public bool setCellPosition = true;

    [SerializeField] private Axis _startAxis = Axis.Horizontal;
    [SerializeField] private RectOffset _padding = default;
    [SerializeField] private Vector2 _cellsize = new Vector2(100, 100);
    [SerializeField] private Vector2 _spacing = new Vector2(1, 1);

    [SerializeField] private int _axisLimit = 1;

    private int _cellCount;
    private int _indexStart = -1;
    private int _indexEnd = -1;
    private int _showCount = 0;
    private int _xCellCount = 0;
    private int _yCellCount = 0;

    private bool _isLayoutDirty = true;
    private bool _isSettingDirty = true;

    private bool _hasCellChanged = true;

    private Queue<InfinityGridCell> cellPool = new Queue<InfinityGridCell>();
    private List<InfinityGridCell> cells = new List<InfinityGridCell>();

    public Action<InfinityGridCell> onCellCreated;
    public Action<InfinityGridCell, int> onCellChanged;

    public Vector2 ContentSize {
        get { return this.Content.sizeDelta; }
    }

    public RectTransform Content {
        get {
            if (this._content == null) {
                this._content = this.ScrollView.content;
            }

            return this._content;
        }
    }

    public ScrollRect ScrollView {
        get {
            if (this._scrollview == null) {
                this._scrollview = this.GetComponent<ScrollRect>();
            }

            return this._scrollview;
        }
    }

    public Vector2 NormalizedPosition {
        get { return this.ScrollView.normalizedPosition; }
        set {
            if (this.ScrollView.normalizedPosition != value) {
                this.ScrollView.normalizedPosition = value;
                this._isLayoutDirty = true;
            }
        }
    }

    public Axis StartAxis {
        get { return this._startAxis; }
        set {
            if (this._startAxis != value) {
                this._startAxis = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int AxisLimit {
        get { return this._axisLimit; }
        set {
            if (this._axisLimit != value) {
                this._axisLimit = value;
                if (this._axisLimit < 1) {
                    this._axisLimit = 1;
                }

                this._isSettingDirty = true;
            }
        }
    }

    public Vector2 CellSize {
        get { return this._cellsize; }
        set {
            if (this._cellsize != value) {
                this._cellsize = value;
                if (this._cellsize.x < 1 || this._cellsize.y < 1) {
                    this._cellsize = new Vector2(Mathf.Max(1, this._cellsize.x), Mathf.Max(1, this._cellsize.y));
                }

                this._isSettingDirty = true;
            }
        }
    }

    public Vector2 Spacing {
        get { return this._spacing; }
        set {
            if (this._spacing != value) {
                this._spacing = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int CellCount {
        get { return this._cellCount; }
        set {
            if (this._cellCount != value) {
                this._cellCount = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int Left {
        get { return this._padding.left; }
        set {
            if (this._padding.left != value) {
                this._padding.left = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int Right {
        get { return this._padding.right; }
        set {
            if (this._padding.right != value) {
                this._padding.right = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int Top {
        get { return this._padding.top; }
        set {
            if (this._padding.top != value) {
                this._padding.top = value;
                this._isSettingDirty = true;
            }
        }
    }

    public int Bottom {
        get { return this._padding.bottom; }
        set {
            if (this._padding.bottom != value) {
                this._padding.bottom = value;
                this._isSettingDirty = true;
            }
        }
    }

    private int InstanceCount {
        get { return this.cellPool.Count + this.cells.Count; }
    }

    public void ApplyLayout() {
        this._isLayoutDirty = false;

        // 逐个加载
        int realShowCount = Mathf.Min(this.InstanceCount + 1, this._showCount);

#region 计算新的开头结尾
        int newStartIndex = 0, newEndIndex = 0;
        if (this.StartAxis == Axis.Horizontal) {
            newStartIndex = (int)((this.Content.anchoredPosition.y - this.Top) / (this._cellsize.y + this.Spacing.y)) * this._xCellCount;
        }
        else {
            newStartIndex = (int)((this.Content.anchoredPosition.x - this.Left) / (this._cellsize.x + this.Spacing.x)) * this._yCellCount;
        }

        if (newStartIndex > this._cellCount - realShowCount) {
            newStartIndex = this._cellCount - realShowCount;
        }
        else if (newStartIndex < 0) {
            newStartIndex = 0;
        }

        newEndIndex = realShowCount + newStartIndex - 1;
#endregion

        int head = newStartIndex - this._indexStart;
        int tail = this._indexEnd - newEndIndex;

        this._indexStart = newStartIndex;
        this._indexEnd = newEndIndex;

#region 去除多余的开头结尾
        if (head > 0) {
            int removeCount = Mathf.Min(head, this.cells.Count);
            if (removeCount > 0) {
                for (int i = 0; i < removeCount; ++i) {
                    this.Collect(this.cells[i]);
                }

                this.cells.RemoveRange(0, removeCount);
            }
        }

        if (tail > 0) {
            int removeCount = Mathf.Min(tail, this.cells.Count);
            if (removeCount > 0) {
                for (int i = this.cells.Count - 1; i >= this.cells.Count - removeCount; --i) {
                    this.Collect(this.cells[i]);
                }

                this.cells.RemoveRange(this.cells.Count - removeCount, removeCount);
            }
        }
#endregion

#region 添加不够的开头结尾
        if (head < 0) {
            int addCount = Mathf.Min(-head, realShowCount);
            for (int i = 0; i < addCount; ++i) {
                this.cells.Insert(0, this.BuildOne());
            }
        }

        if (tail < 0) {
            int addCount = Mathf.Min(-tail, realShowCount);
            for (int i = 0; i < addCount; ++i) {
                this.cells.Add(this.BuildOne());
            }
        }
#endregion

#region 刷新列表index
        int x = 0, y = 0;
        for (int i = 0; i < this.cells.Count; ++i) {
            InfinityGridCell cell = this.cells[i];
            int oldIndex = cell.index;
            cell.SetIndex(i + this._indexStart);

            if (this.StartAxis == Axis.Horizontal) {
                y = cell.index / this._xCellCount;
                x = cell.index - y * this._xCellCount;
            }
            else {
                x = cell.index / this._yCellCount;
                y = cell.index - x * this._yCellCount;
            }

            if (this.setCellPosition) {
                var xx = x * (this._cellsize.x + this.Spacing.x) + this.Left;
                var yy = -y * (this._cellsize.y + this.Spacing.y) - this.Top;
                cell.bindGo.localPosition = new Vector3(xx, yy, 0);
            }

            if (cell.index != oldIndex) {
                this._hasCellChanged = true;
            }
        }
#endregion

        // 逐个加载
        if (this.InstanceCount < this._showCount) {
            this._isLayoutDirty = true;
        }
    }

    public void ApplySetting() {
        this._isSettingDirty = false;

        if (this.ScrollView == null) {
            return;
        }

        if (this.StartAxis == Axis.Horizontal) {
            this._xCellCount = this._axisLimit;
            this._yCellCount = Mathf.CeilToInt((float)this._cellCount / this._axisLimit);
            this._showCount = (int)((this.ScrollView.viewport.rect.height / this._cellsize.y) + 2) * this._xCellCount;
        }
        else {
            this._yCellCount = this._axisLimit;
            this._xCellCount = Mathf.CeilToInt((float)this._cellCount / this._axisLimit);
            this._showCount = (int)((this.ScrollView.viewport.rect.width / this._cellsize.x) + 2) * this._yCellCount;
        }

        var x = this._xCellCount * this._cellsize.x + (this._xCellCount - 1) * this.Spacing.x + this.Left + this.Right;
        var y = this._yCellCount * this._cellsize.y + (this._yCellCount - 1) * this.Spacing.y + this.Top + this.Bottom;
        this.Content.sizeDelta = new Vector2(x, y);

        this._showCount = Mathf.Min(this._showCount, this._cellCount);
        this._isLayoutDirty = true;
    }

    public void Clear() {
        this.CellCount = 0;

        for (int i = 0; i < this.cells.Count; ++i) {
            InfinityGridCell cell = this.cells[i];
            if (cell != null && cell.bindGo != null) {
                cell.BindUserData(null);
                DestroyImmediate(cell.bindGo.gameObject);
            }
        }

        this.cells.Clear();

        while (this.cellPool.Count > 0) {
            InfinityGridCell cell = this.cellPool.Dequeue();
            if (cell != null && cell.bindGo != null) {
                cell.BindUserData(null);
                DestroyImmediate(cell.bindGo.gameObject);
            }
        }

        this._indexStart = -1;
        this._indexEnd = -1;
    }

    private InfinityGridCell BuildOne() {
        InfinityGridCell cell = null;
        if (this.cellPool.Count > 0) {
            cell = this.cellPool.Dequeue();
        }
        else {
            GameObject go = Instantiate(this._proto);
            cell = new InfinityGridCell();
            cell.SetIndex(-1);
            cell.BindGameObject(go);

            cell.bindGo.SetParent(this.Content, false);
            cell.bindGo.localScale = Vector3.one;
            cell.bindGo.pivot = new Vector2(0, 1);

            this.onCellCreated?.Invoke(cell);
        }

        cell.bindGo.anchoredPosition3D = Vector3.zero;
        cell.bindGo.gameObject.SetActive(true);
        return cell;
    }

    private void Collect(InfinityGridCell cell) {
        this.cellPool.Enqueue(cell);
        cell.SetIndex(-1);
        cell.bindGo.gameObject.SetActive(false);
    }

    private void OnValueChanged(Vector2 v) {
        this._isLayoutDirty = true;
        if (this.Content != this.ScrollView.content) {
            this.Content.anchoredPosition = this.ScrollView.content.anchoredPosition;
        }
    }

    private void Awake() {
        if (this.ScrollView != null) {
            this._scrollview.onValueChanged.AddListener(this.OnValueChanged);
            this.Content.anchorMin = new Vector2(0, 1);
            this.Content.anchorMax = new Vector2(0, 1);
            this.Content.pivot = new Vector2(0, 1);
        }

        this._proto.SetActive(false);
    }

    private void Update() {
        if (this._isSettingDirty) {
            this.ApplySetting();
        }

        if (this._isLayoutDirty) {
            this.ApplyLayout();
        }

        if (this._hasCellChanged && this.onCellChanged != null) {
            for (int i = 0; i < this.cells.Count; ++i) {
                InfinityGridCell cell = this.cells[i];
                if (cell.bDirty) {
                    cell.SetDirty(false);
                    this.onCellChanged.Invoke(cell, cell.index);
                }
            }

            this._hasCellChanged = false;
        }
    }

    public void ForceRefreshActiveCell() {
        for (int i = 0; i < this.cells.Count; ++i) {
            InfinityGridCell cell = this.cells[i];
            cell.SetDirty(true);
        }

        this.StopMovement();
        this._isSettingDirty = true;
        this._hasCellChanged = true;
    }

    public void StopMovement() {
        this.ScrollView.StopMovement();
    }

    public InfinityGridCell GetCellByIndex(int index) {
        if (this._indexStart < 0 || this._indexEnd < 0) {
            return null;
        }

        if (index < this._indexStart || index > this._indexEnd) {
            return null;
        }

        for (int i = 0; i < this.cells.Count; ++i) {
            InfinityGridCell cell = this.cells[i];
            if (cell.index == index) {
                return cell;
            }
        }

        return null;
    }

    public void MoveIndexToTop(int index) {
        if (index < 0 || index >= this.CellCount) {
            return;
        }

        int l = index / this._axisLimit;
        if (l == 0) {
            this.NormalizedPosition = Vector2.zero;
            return;
        }

        float x = 0, y = 0;
        if (this.StartAxis == Axis.Horizontal) {
            y = l * (this.Spacing.y + this.CellSize.y) / (this._scrollview.content.sizeDelta.y - this._scrollview.viewport.sizeDelta.y);
        }
        else {
            x = l * (this.Spacing.x + this.CellSize.x) / (this._scrollview.content.sizeDelta.x - this._scrollview.viewport.sizeDelta.x);
        }

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);
        this.NormalizedPosition = new Vector2(x, y);
    }

    public void MoveToIndex(int index) {
        if (index < 0 || index >= this.CellCount) {
            return;
        }

        int l = index / this._axisLimit;
        if (l == 0) {
            this._content.anchoredPosition = Vector2.zero;
            return;
        }

        float x = 0, y = 0;
        if (this.StartAxis == Axis.Horizontal) {
            y = ((l - 1) * (this.Spacing.y + this.CellSize.y) + this.Top + this.CellSize.y * 0.5f - this._scrollview.viewport.sizeDelta.y * 0.5f);
        }
        else {
            x = ((l - 1) * (this.Spacing.x + this.CellSize.x) + this.Top + this.CellSize.x * 0.5f - this._scrollview.viewport.sizeDelta.x * 0.5f);
        }

        this._content.anchoredPosition = new Vector2(x, y);
    }

    public void MoveToMiddle(int index) {
        if (index < 0 || index >= this.CellCount) {
            return;
        }

        int l = index / this._axisLimit;
        if (l == 0) {
            this.NormalizedPosition = Vector2.zero;
            return;
        }

        float x = 0, y = 0;
        if (this.StartAxis == Axis.Horizontal) {
            y = ((l - 1) * (this.Spacing.y + this.CellSize.y) + this.Top + this.CellSize.y * 0.5f - this._scrollview.viewport.sizeDelta.y * 0.5f) / (this._scrollview.content.sizeDelta.y - this._scrollview.viewport.sizeDelta.y);
        }
        else {
            x = ((l - 1) * (this.Spacing.x + this.CellSize.x) + this.Top + this.CellSize.x * 0.5f - this._scrollview.viewport.sizeDelta.x * 0.5f) / (this._scrollview.content.sizeDelta.x - this._scrollview.viewport.sizeDelta.x);
        }

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);
        this.NormalizedPosition = new Vector2(x, y);
    }
}
