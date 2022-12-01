using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

#if UNITY_EDITOR
using UnityEditor;
using SC;
using UnityEngine;
using UnityEngine.UI;
namespace SC
{
    [CustomEditor(typeof(InfinityGrid))]
    public class ScrollRectGridInsprctor : Editor
    {
        InfinityGrid _grid = null;
        InfinityGrid grid
        {
            get
            {
                if (_grid == null)
                    _grid = target as InfinityGrid;
                return _grid;
            }
        }

        bool _foldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isPlaying)
            {
                EditorGUILayout.LabelField(string.Format("Cell Count {0}", grid.CellCount));
            }
            else
            {
                grid.CellCount = EditorGUILayout.IntField("Cell Count", grid.CellCount);
            }

            EditorGUI.BeginChangeCheck();
            _foldout = EditorGUILayout.Foldout(_foldout, "Padding");
            if (_foldout)
            {
                grid.Left = EditorGUILayout.IntField("Left", grid.Left);
                grid.Right = EditorGUILayout.IntField("Right", grid.Right);
                grid.Top = EditorGUILayout.IntField("Top", grid.Top);
                grid.Bottom = EditorGUILayout.IntField("Bottom", grid.Bottom);
                EditorGUILayout.Space();
            }            

            grid.CellSize = EditorGUILayout.Vector2Field("Cell Size", grid.CellSize);
            grid.Spacing = EditorGUILayout.Vector2Field("Spacing", grid.Spacing);
            grid.StartAxis = (GridLayoutGroup.Axis)EditorGUILayout.EnumPopup("StartAxis", grid.StartAxis);
            if (grid.StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                grid.AxisLimit = EditorGUILayout.IntField("Horizontal Count", grid.AxisLimit);
            }
            else
            {
                grid.AxisLimit = EditorGUILayout.IntField("Vertical Count", grid.AxisLimit);
            }

            if(EditorGUI.EndChangeCheck())
            {
                grid.Update();
            }

            if (GUILayout.Button("clear"))
            {
                grid.Clear();
            }
            if (GUILayout.Button("test"))
            {
                grid.Update();
            }
        }
    }
}
#endif

/* 使用案例
 *
            // 注册代理  
            infinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            infinityGrid.onCellChange += RoleInfinityGridCellChange;
            
            // 刷新数据
            infinityGrid.CellCount = 元素个数;
            infinityGrid.ForceRefreshActiveCell()
            
            void RoleInfinityGridCreateCell(InfinityGridCell cell) {
                GameObject go = cell.mRootTransform.gameObject;
                
                UI_WarriorGroup_Transfer_Layout.RoleItem vd = new UI_WarriorGroup_Transfer_Layout.RoleItem();
                vd.BindGameObject(go);
                cell.BindUserData(vd);
            }

            void RoleInfinityGridCellChange(InfinityGridCell cell, int index) {
                 var vd = cell.mUserData as UI_WarriorGroup_Transfer_Layout.RoleItem;
            }
 */

public class InfinityGridCell {
    public bool bDirty { get; private set; }
    public int nIndex { get; private set; } = -1;
    public RectTransform mRootTransform { get; private set; }
    public System.Object mUserData { get; private set; }

    internal void SetDirty(bool v) {
        bDirty = v;
    }

    internal void SetIndex(int index) {
        if (nIndex != index) {
            bDirty = true;
            nIndex = index;
            mRootTransform.name = index.ToString();
        }
    }

    public void BindGameObject(GameObject go) {
        mRootTransform = go.transform as RectTransform;
    }

    public void BindUserData(System.Object userData) {
        mUserData = userData;
    }
}

[RequireComponent(typeof(ScrollRect))]
[DisallowMultipleComponent]
public class InfinityGrid : MonoBehaviour {
    public ScrollRect _scrollView = null;
    public RectTransform _content = null;
    public GameObject _element = null;

    public bool setCellPosition = true;
    [SerializeField] [HideInInspector] private RectOffset _padding = new RectOffset();
    [SerializeField] [HideInInspector] private Axis _startAxis = Axis.Horizontal;
    [SerializeField] [HideInInspector] private int _axisLimit = 1;
    [SerializeField] [HideInInspector] private Vector2 _cellSize = Vector2.one;
    [SerializeField] [HideInInspector] private Vector2 _spacing = Vector2.one;

    private int _cellCount;
    private int _indexStart = -1;
    private int _indexEnd = -1;
    private int _showCount = 0;
    private int _xCellCount = 0;
    private int _yCellCount = 0;

    private bool isDrity = true;
    private bool isSettingDrity = true;
    private bool hasCellChange = true;

    private Queue<InfinityGridCell> mCellPool = new Queue<InfinityGridCell>();
    private List<InfinityGridCell> mCells = new List<InfinityGridCell>();

    public Action<InfinityGridCell> onCreateCell;
    public Action<InfinityGridCell, int> onCellChange;

    public Vector2 ContentSize {
        get { return Content.sizeDelta; }
    }

    public RectTransform Content {
        get {
            if (_content == null) {
                _content = ScrollView.content;
            }

            return _content;
        }
    }

    public ScrollRect ScrollView {
        get {
            if (null == _scrollView) {
                _scrollView = gameObject.GetComponent<ScrollRect>();
            }

            return _scrollView;
        }
    }

    public Vector2 NormalizedPosition {
        get { return ScrollView.normalizedPosition; }
        set {
            if (ScrollView.normalizedPosition != value) {
                ScrollView.normalizedPosition = value;
                isDrity = true;
            }
        }
    }

    public Axis StartAxis {
        get { return _startAxis; }
        set {
            if (_startAxis != value) {
                _startAxis = value;
                isSettingDrity = true;
            }
        }
    }

    public int AxisLimit {
        get { return _axisLimit; }
        set {
            if (_axisLimit != value) {
                _axisLimit = value;
                if (_axisLimit < 1) {
                    _axisLimit = 1;
                }

                isSettingDrity = true;
            }
        }
    }

    public Vector2 CellSize {
        get { return _cellSize; }
        set {
            if (_cellSize != value) {
                _cellSize = value;

                if (_cellSize.x < 1 || _cellSize.y < 1) {
                    _cellSize = new Vector2(Mathf.Max(1, _cellSize.x), Mathf.Max(1, _cellSize.y));
                }

                isSettingDrity = true;
            }
        }
    }

    public Vector2 Spacing {
        get { return _spacing; }
        set {
            if (_spacing != value) {
                _spacing = value;
                isSettingDrity = true;
            }
        }
    }

    public int CellCount {
        get { return _cellCount; }
        set {
            if (_cellCount != value) {
                _cellCount = value;
                isSettingDrity = true;
            }
        }
    }

    public int Left {
        get { return _padding.left; }
        set {
            if (_padding.left != value) {
                _padding.left = value;
                isSettingDrity = true;
            }
        }
    }

    public int Right {
        get { return _padding.right; }
        set {
            if (_padding.right != value) {
                _padding.right = value;
                isSettingDrity = true;
            }
        }
    }

    public int Top {
        get { return _padding.top; }
        set {
            if (_padding.top != value) {
                _padding.top = value;
                isSettingDrity = true;
            }
        }
    }

    public int Bottom {
        get { return _padding.bottom; }
        set {
            if (_padding.bottom != value) {
                _padding.bottom = value;
                isSettingDrity = true;
            }
        }
    }

    private int InstanceCount {
        get { return mCellPool.Count + mCells.Count; }
    }

    public void ApplyLayout() {
        isDrity = false;

        //实现逐个加载
        int realShowCount = Mathf.Min(InstanceCount + 1, _showCount);

#region 计算新的开头结尾
        int newStartIndex;
        int newEndIndex;

        if (StartAxis == Axis.Horizontal) {
            newStartIndex = (int)((Content.anchoredPosition.y - Top) / (_cellSize.y + Spacing.y)) * _xCellCount;
        }
        else {
            newStartIndex = (int)((-Content.anchoredPosition.x - Left) / (_cellSize.x + Spacing.x)) * _yCellCount;
        }

        if (newStartIndex > _cellCount - realShowCount) {
            newStartIndex = _cellCount - realShowCount;
        }
        else if (newStartIndex < 0) {
            newStartIndex = 0;
        }

        newEndIndex = realShowCount + newStartIndex - 1;
#endregion

        int head = newStartIndex - _indexStart;
        int tail = _indexEnd - newEndIndex;

        _indexStart = newStartIndex;
        _indexEnd = newEndIndex;

#region 去除多余的开头结尾
        if (head > 0) {
            int removeCount = Mathf.Min(head, mCells.Count);
            if (removeCount > 0) {
                for (int i = 0; i < removeCount; ++i) {
                    Collect(mCells[i]);
                }

                mCells.RemoveRange(0, removeCount);
            }
        }

        if (tail > 0) {
            int removeCount = Mathf.Min(tail, mCells.Count);
            if (removeCount > 0) {
                for (int i = mCells.Count - 1; i >= mCells.Count - removeCount; --i) {
                    Collect(mCells[i]);
                }

                mCells.RemoveRange(mCells.Count - removeCount, removeCount);
            }
        }
#endregion

#region 添加不够的开头结尾
        if (head < 0) {
            int addCount = Mathf.Min(-head, realShowCount);
            for (int i = 0; i < addCount; ++i) {
                mCells.Insert(0, Alloc());
            }
        }

        if (tail < 0) {
            int addCount = Mathf.Min(-tail, realShowCount);
            for (int i = 0; i < addCount; ++i) {
                mCells.Add(Alloc());
            }
        }
#endregion

#region 刷新列表Index
        int x, y = 0;
        InfinityGridCell cell = null;
        for (int i = 0; i < mCells.Count; ++i) {
            cell = mCells[i];
            int oldIndex = cell.nIndex;
            cell.SetIndex(i + _indexStart);

            if (_startAxis == Axis.Horizontal) {
                y = cell.nIndex / _xCellCount;
                x = cell.nIndex - y * _xCellCount;
            }
            else {
                x = cell.nIndex / _yCellCount;
                y = cell.nIndex - x * _yCellCount;
            }

            if (setCellPosition) {
                cell.mRootTransform.localPosition = new Vector3(x * (_cellSize.x + Spacing.x) + Left, -y * (_cellSize.y + Spacing.y) - Top, 0);
            }

            if (cell.nIndex != oldIndex) {
                hasCellChange = true;
            }
        }
#endregion

        //实现逐个加载
        if (InstanceCount < _showCount) {
            isDrity = true;
        }
    }

    public void ApplySetting() {
        isSettingDrity = false;

        if (null == ScrollView) {
            Debug.LogError("UIGrid ApplySetting ScrollRect is null");
            return;
        }

        //_scrollRect.content.anchoredPosition = Vector2.zero;

        if (StartAxis == Axis.Horizontal) {
            _xCellCount = _axisLimit;
            _yCellCount = Mathf.CeilToInt((float)_cellCount / _axisLimit);
            _showCount = (int)((ScrollView.viewport.rect.height / _cellSize.y) + 2) * _xCellCount;
        }
        else {
            _yCellCount = _axisLimit;
            _xCellCount = Mathf.CeilToInt((float)_cellCount / _axisLimit);
            _showCount = (int)((ScrollView.viewport.rect.width / _cellSize.x) + 2) * _yCellCount;
        }

        Content.sizeDelta = new Vector2(_xCellCount * _cellSize.x + (_xCellCount - 1) * Spacing.x + Left + Right, _yCellCount * _cellSize.y + (_yCellCount - 1) * Spacing.y + Top + Bottom);
        //ScrollView.normalizedPosition = new Vector2(0, 1);

        _showCount = Mathf.Min(_showCount, _cellCount);

        isDrity = true;
    }

    public void Clear() {
        CellCount = 0;

        for (int i = 0; i < mCells.Count; ++i) {
            InfinityGridCell cell = mCells[i];
            if (cell != null && cell.mRootTransform != null) {
                cell.BindUserData(null);
                DestroyImmediate(cell.mRootTransform.gameObject);
            }
        }

        mCells.Clear();

        while (mCellPool.Count > 0) {
            InfinityGridCell cell = mCellPool.Dequeue();
            if (cell != null && cell.mRootTransform != null) {
                cell.BindUserData(null);
                DestroyImmediate(cell.mRootTransform.gameObject);
            }
        }

        _indexStart = -1;
        _indexEnd = -1;
    }

    private InfinityGridCell Alloc() {
        InfinityGridCell cell = null;
        if (mCellPool.Count > 0) {
            cell = mCellPool.Dequeue();
        }
        else {
            GameObject go = GameObject.Instantiate(_element);
            cell = new InfinityGridCell();
            cell.SetIndex(-1);
            cell.BindGameObject(go);

            cell.mRootTransform.SetParent(Content, false);
            cell.mRootTransform.localScale = Vector3.one;
            cell.mRootTransform.pivot = new Vector2(0, 1);

            onCreateCell?.Invoke(cell);
        }

        cell.mRootTransform.anchoredPosition3D = Vector3.zero;
        cell.mRootTransform.gameObject.SetActive(true);
        return cell;
    }

    private void Collect(InfinityGridCell uIGridElement) {
        mCellPool.Enqueue(uIGridElement);
        uIGridElement.SetIndex(-1);
        uIGridElement.mRootTransform.gameObject.SetActive(false);
    }

    private void OnValueChanged(Vector2 v) {
        isDrity = true;
        if (Content != ScrollView.content) {
            Content.anchoredPosition = ScrollView.content.anchoredPosition;
        }
    }

    private void Awake() {
        if (null != ScrollView) {
            _scrollView.onValueChanged.AddListener(OnValueChanged);
            Content.anchorMin = new Vector2(0f, 1f); //new Vector2(0.5f, 1f);
            Content.anchorMax = new Vector2(0f, 1f); //new Vector2(0.5f, 1f);
            Content.pivot = new Vector2(0f, 1f); //new Vector2(0.5f, 1f);
        }
    }

    public void Apply() {
        if (isSettingDrity) {
            ApplySetting();
        }

        if (isDrity) {
            ApplyLayout();
        }
    }

    public void Update() {
        if (isSettingDrity) {
            ApplySetting();
        }

        if (isDrity) {
            ApplyLayout();
        }

        if (hasCellChange && onCellChange != null) {
            InfinityGridCell cell;
            for (int i = 0; i < mCells.Count; ++i) {
                cell = mCells[i];
                if (cell.bDirty) {
                    cell.SetDirty(false);
                    onCellChange(cell, cell.nIndex);
                }
            }

            hasCellChange = false;
        }
    }

    public void ForceRefreshActiveCell() {
        InfinityGridCell cell;
        for (int i = 0; i < mCells.Count; ++i) {
            cell = mCells[i];
            cell.SetDirty(true);
        }

        StopMovement();
        isSettingDrity = true;
        hasCellChange = true;
    }

    public void StopMovement() {
        ScrollView.StopMovement();
    }

    public InfinityGridCell GetItemByIndex(int index) {
        if (_indexStart < 0 || _indexEnd < 0) {
            return null;
        }

        if (index < _indexStart || index > _indexEnd) {
            return null;
        }

        for (int i = 0; i < mCells.Count; ++i) {
            if (mCells[i].nIndex == index)
                return mCells[i];
        }

        return null;
    }

    public void MoveIndexToTop(int index) {
        if (index < 0 || index >= CellCount)
            return;

        int l = index / _axisLimit;
        if (l == 0) {
            NormalizedPosition = new Vector2(0, 0);
            return;
        }

        float x = 0;
        float y = 0;

        if (StartAxis == Axis.Horizontal) {
            y = l * (Spacing.y + CellSize.y) / (_scrollView.content.rect.height - _scrollView.viewport.rect.height);
        }
        else {
            x = l * (Spacing.x + CellSize.x) / (_scrollView.content.rect.width - _scrollView.viewport.rect.width);
        }

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        NormalizedPosition = new Vector2(x, y);
    }

    public void MoveToIndex(int index) {
        if (index < 0 || index >= CellCount)
            return;

        int l = index / _axisLimit;
        if (l == 0) {
            _content.anchoredPosition = Vector2.zero;
            //NormalizedPosition = new Vector2(0, 0);
            return;
        }

        float x = 0;
        float y = 0;

        if (StartAxis == Axis.Horizontal) {
            y = ((l - 1) * (Spacing.y + CellSize.y) + Top + CellSize.y * 0.5f - _scrollView.viewport.sizeDelta.y * 0.5f);
        }
        else {
            x = ((l - 1) * (Spacing.x + CellSize.x) + Top + CellSize.x * 0.5f - _scrollView.viewport.sizeDelta.x * 0.5f);
        }

        _content.anchoredPosition = new Vector2(x, y);
        //x = Mathf.Clamp01(x);
        //y = Mathf.Clamp01(y);

        //NormalizedPosition = new Vector2(x, y);
    }

    public void MoveIndexToMiddle(int index) {
        if (index < 0 || index >= CellCount)
            return;

        int l = index / _axisLimit;
        if (l == 0) {
            NormalizedPosition = new Vector2(0, 0);
            return;
        }

        float x = 0;
        float y = 0;

        if (StartAxis == Axis.Horizontal) {
            y = ((l - 1) * (Spacing.y + CellSize.y) + Top + CellSize.y * 0.5f - _scrollView.viewport.rect.height * 0.5f) / (_scrollView.content.rect.height - _scrollView.viewport.rect.height);
        }
        else {
            x = ((l - 1) * (Spacing.x + CellSize.x) + Top + CellSize.x * 0.5f - _scrollView.viewport.rect.width * 0.5f) / (_scrollView.content.rect.width - _scrollView.viewport.rect.width);
        }

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        NormalizedPosition = new Vector2(x, y);
    }

    public IReadOnlyList<InfinityGridCell> GetCells() {
        return mCells;
    }
}
