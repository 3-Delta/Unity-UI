using System;
using UnityEngine;
using Object = System.Object;

public interface IListenReconnect {
    void OnBeginReconnect();
    void OnEndReconnect();
}

[Serializable]
public class FUIBase /*: IListenReconnect*/ {
    public int uiType;
    [SerializeField] public FUIEntry cfg;

    public int order;

    protected CanvasAdapter adapter;

    // UIEntry能否将UIEntry设置为表格填写的形式，也就是提剔除uiconfig
    // 因为有时候，可能需要在B ui打开的时候，将前面的A ui关闭掉。所以需要外部设置这些回调
    public void Init(int uiType, FUIEntry cfg) {
        this.uiType = uiType;
        this.cfg = cfg;
    }

    public void BlcokRaycast(bool toBlcok) {
        // 防止点击事件
    }

    #region Open/Close
    private bool _firstOpen = true;
    public bool hasExecutedOpen { get; private set; }

    public void Open(Tuple<ulong, ulong, ulong, object> arg) {
        if (_firstOpen) {
            OnOpen(arg);
            _firstOpen = false;
        }
        else {
            OnTransfer(arg);
        }

        if (!TryLoad()) {
            Show();
        }

        hasExecutedOpen = true;
    }

    public void Close() {
        if (hasExecutedOpen) {
            OnClose();
            hasExecutedOpen = false;
        }
    }
    #endregion

    #region Show/Hide
    private bool _firstShow = true;
    public bool hasExecutedShow { get; private set; }

    public void Show() {
        if (!hasLoaded) {
            return;
        }

        if (_firstShow) {
            OnOpened();
            _firstShow = false;
        }

        OnShow();
        hasExecutedShow = true;
    }

    public void Hide() {
        if (hasExecutedShow) {
            OnHide();
            hasExecutedShow = false;
        }
    }
    #endregion

    #region 加载Prefab
    public bool TryLoad() {
        if (!hasLoaded && !isLoading) {
            _request = Resources.LoadAsync<GameObject>(cfg.prefabPath);
            _request.completed += _Loaded;
            return true;
        }

        return false;
    }

    private ResourceRequest _request;
    public bool hasLoaded { get; private set; }

    public bool isLoading {
        get { return _request != null; }
    }

    private void _Loaded(AsyncOperation op) {
        _request.completed -= _Loaded;
        hasLoaded = true;

        GameObject clone = GameObject.Instantiate(_request.asset, FUIMgr.uiParent) as GameObject;
        var transform = clone.transform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
#if UNITY_EDITOR
        transform.name = string.Format("{0} {1} {2}", uiType.ToString(), cfg.ui, transform.name);
#endif

        if (!transform.TryGetComponent<UIInjector>(out var injector)) {
            injector = transform.gameObject.AddComponent<UIInjector>();
        }

        injector.value = this;

        if (!transform.TryGetComponent<CanvasAdapter>(out adapter)) {
            adapter = transform.gameObject.AddComponent<CanvasAdapter>();
        }

        adapter.SetOrder(order);

        _request = null;

        OnLoaded(transform);
        Show();
    }
    #endregion

    #region 生命周期
    protected virtual void OnLoaded(Transform transform) {
        // 资源组件解析
    }

    public virtual void OnTransfer(Object arg) {
        // ui已经打开的时候调用UIMgr.Open
    }

    public virtual void OnOpen(Object arg) {
        // ui没有打开的时候调用UIMgr.Open
    }

    public virtual void OnClose() { }

    protected virtual void OnOpened() { }

    protected virtual void OnShow() { }

    public virtual void OnHide() { }

    // public virtual void OnBeginReconnect() {
    //     // 断线重连UI打开之前
    // }
    //
    // public virtual void OnEndReconnect() {
    //     // 断线重连UI关闭之后
    // }
    #endregion
}
