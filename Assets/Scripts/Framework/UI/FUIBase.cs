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
    public FUIEntry cfg;

    public int order;

#if UNITY_EDITOR
    public Transform transform;
    public bool showHide = true;
    public bool hasListenedEvent = false;
    public bool hasListenedEventForShowHide = false;
    public bool hasExecutedOpen;
    public CanvasAdapter adapter;
    public bool _firstShow = true;
    public bool hasExecutedShow;
#else
    private Transform transform;
    private bool showHide = true;
    private bool hasListenedEvent = false;
    private bool hasListenedEventForShowHide = false;
    public bool hasExecutedOpen { get; private set; }
    public CanvasAdapter adapter { get; private set; }
    private bool _firstShow = true;
    public bool hasExecutedShow { get; private set; }
#endif

    // UIEntry能否将UIEntry设置为表格填写的形式，也就是提剔除uiconfig
    // 因为有时候，可能需要在B ui打开的时候，将前面的A ui关闭掉。所以需要外部设置这些回调
    public void Init(int uiType, FUIEntry cfg) {
        Debug.LogError(string.Format("Init {0}", uiType.ToString()));
        this.uiType = uiType;
        this.cfg = cfg;
    }

    public void BlcokRaycast(bool toBlcok) {
        // 防止点击事件
    }

    #region Open/Close
    public void Open(Tuple<ulong, ulong, ulong, object> arg) {
        OnOpen(arg);

        // open的时候，如果prefab已经存在，则会调用show
        TryLoad();
        _firstShow = true;
        Show();

        hasExecutedOpen = true;
    }

    public void Transfer(Tuple<ulong, ulong, ulong, object> arg) {
        OnTransfer(arg);
        _firstShow = true;
        Show();
    }

    public void Close() {
        if (hasExecutedOpen) {
            if (_request != null) {
                _request.completed -= _Loaded;
            }

            _request = null;

            Hide();

            if (hasListenedEvent) {
                ProcessEvent(false);
            }

            hasListenedEvent = false;

            OnClose();
            hasExecutedOpen = false;

            if (hasLoaded) {
                GameObject.Destroy(transform.gameObject);
            }

            transform = null;
        }
    }
    #endregion

    #region Show/Hide
    public void Show() {
        showHide = true;

        if (!hasLoaded) {
            return;
        }

        transform.gameObject.SetActive(true);

        if (!hasListenedEventForShowHide) {
            ProcessEventForShowHide(true);
            hasListenedEventForShowHide = true;
        }

        if (_firstShow) {
            OnOpened();
            _firstShow = false;
        }

        OnShow();
        hasExecutedShow = true;
    }

    public void Hide() {
        // 已加载，立即隐藏，否则设置showHide=false进行隐藏
        showHide = false;
        if (hasLoaded) {
            transform.gameObject.SetActive(false);
        }

        if (hasListenedEventForShowHide) {
            ProcessEventForShowHide(false);
        }

        hasListenedEventForShowHide = false;

        if (hasExecutedShow) {
            OnHide();
            hasExecutedShow = false;
        }
    }
    #endregion

    #region 加载Prefab
    private bool TryLoad() {
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
        if (clone == null) {
            Debug.LogError(string.Format("uiType {0}'s prefab {1} is not valid", uiType.ToString(), cfg.prefabPath));
            return;
        }

        transform = clone.transform;
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

        if (!transform.TryGetComponent<CanvasAdapter>(out CanvasAdapter _)) {
            adapter = transform.gameObject.AddComponent<CanvasAdapter>();
        }

        _request = null;

        OnLoaded(transform);

        if (!hasListenedEvent) {
            ProcessEvent(true);
            hasListenedEvent = true;
        }

        // open但是prefab还没加载完毕的时候，执行了hide操作，此时不能show
        if (showHide) {
            Show();
        }
        else {
            Hide();
        }
    }
    #endregion

    #region 生命周期
    protected virtual void OnLoaded(Transform transform) {
        // 资源组件解析
        Debug.LogError(string.Format("OnLoaded {0}", uiType.ToString()));
    }

    protected virtual void OnTransfer(Object arg) {
        // ui已经打开的时候调用OnTransfer
        Debug.LogError(string.Format("OnTransfer {0}", uiType.ToString()));
    }

    protected virtual void OnOpen(Object arg) {
        // ui没有打开的时候调用UIMgr.Open
        Debug.LogError(string.Format("OnOpen {0}", uiType.ToString()));
    }

    protected virtual void OnClose() {
        Debug.LogError(string.Format("OnClose {0}", uiType.ToString()));
    }

    protected virtual void OnOpened() {
        Debug.LogError(string.Format("OnOpened {0}", uiType.ToString()));
    }

    protected virtual void OnShow() {
        Debug.LogError(string.Format("OnShow {0}", uiType.ToString()));
    }

    protected virtual void OnHide() {
        Debug.LogError(string.Format("OnHide {0}", uiType.ToString()));
    }

    protected virtual void ProcessEvent(bool toListen) {
        Debug.LogError(string.Format("ProcessEvent {0} {1}", uiType.ToString(), toListen.ToString()));
    }

    protected virtual void ProcessEventForShowHide(bool toListen) {
        Debug.LogError(string.Format("ProcessEventForShowHide {0} {1}", uiType.ToString(), toListen.ToString()));
    }

    // public virtual void OnBeginReconnect() {
    //     // 断线重连UI打开之前
    // }
    //
    // public virtual void OnEndReconnect() {
    //     // 断线重连UI关闭之后
    // }
    #endregion
}
