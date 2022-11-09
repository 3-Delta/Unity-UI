using System;
using System.Collections.Generic;
using UnityEngine;

public interface IListenReconnect {
    void OnBeginReconnect();
    void OnEndReconnect();
}

[Serializable]
public class FUIBase /*: IListenReconnect*/ {
    public Action<int, int> onSortOrderChanged;

    public int uiType;
    public FUIEntry cfg;

    public int order;

    // 从哪个UI点击，打开的本UI,如果没有，则-1
    public int sourceUIType = -1;

    private Transform transform;
    private CanvasAdapter adapter;
    private UIAnimController animPlayer;
    private UIAudioPlayer audioPlayer;

#if UNITY_EDITOR
    public bool showHide = true;
    public bool hasListenedEvent = false;
    public bool hasListenedEventForShowHide = false;
    public bool hasExecutedOpen;
    public bool firstShow = true;
    public bool hasExecutedShow;
    public List<int> relatives = new List<int>();
#else
    public bool showHide { get; private set; }
    private bool hasListenedEvent = false;
    private bool hasListenedEventForShowHide = false;
    public bool hasExecutedOpen { get; private set; }
    private bool firstShow = true;
    public bool hasExecutedShow { get; private set; }
    public List<int> relatives { get; private set; } = new List<int>();
#endif
    
    public FUIBase() { }

    // UIEntry能否将UIEntry设置为表格填写的形式，也就是提剔除uiconfig
    // 因为有时候，可能需要在B ui打开的时候，将前面的A ui关闭掉。所以需要外部设置这些回调
    public void Init(int uiType, FUIEntry cfg) {
        Debug.LogError(string.Format("Init {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));

        this.uiType = uiType;
        this.cfg = cfg;
    }

    // 防止点击事件
    public void BlockRaycaster(bool toBlcok) {
        Debug.LogError(string.Format("BlockRaycaster {0} {1} {2}", uiType.ToString(), cfg.uiTypeWithNamespace, toBlcok.ToString()));

        if (adapter != null) {
            adapter.BlockRaycaster(toBlcok);
        }
    }

    public void SetOrder(int order) {
        Debug.LogError(string.Format("SetOrder {0} {1} {2}", uiType.ToString(), cfg.uiTypeWithNamespace, order.ToString()));
        int oldOrder = this.order;
        this.order = order;

        if (adapter != null) {
            adapter.SetOrder(order);
        }

        if (oldOrder != order) {
            onSortOrderChanged?.Invoke(oldOrder, order);
        }
    }

#region Open/Close
    public void Open(Tuple<ulong, ulong, ulong, object> arg) {
        OnOpen(arg);
        FUIMgr.OnOpen?.Invoke(uiType, cfg);

        // open的时候，如果prefab已经存在，则会调用show
        TryLoad();
        firstShow = true;
        Show();

        hasExecutedOpen = true;
    }

    public void Transfer(Tuple<ulong, ulong, ulong, object> arg) {
        OnTransfer(arg);
        firstShow = true;
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
            FUIMgr.OnClose?.Invoke(uiType, cfg);
            hasExecutedOpen = false;

            if (hasLoaded) {
                GameObject.Destroy(transform.gameObject);
            }

            transform = null;
        }
    }

    public void CloseSelf() {
        FUIMgr.Close(uiType);
    }

    // 关闭UI来源
    public void CloseSource() {
        if (this.sourceUIType != -1) {
            FUIMgr.Close(this.sourceUIType);
        }
    }
    
    // 从当前UI打开目标UI
    // 传递uiType给目标UI的sourceUIType
    public void Open(int targetUIType, Tuple<ulong, ulong, ulong, object> arg = null) {
        FUIMgr.Open(targetUIType/*, this.uiType*/, arg);
    }

    public void Close(int targetUIType) {
        FUIMgr.Close(targetUIType);
    }
#endregion

#region Show/Hide
    public void Show() {
        showHide = true;

        if (!hasLoaded) {
            return;
        }

        transform.gameObject.SetActive(true);

        if (firstShow) {
            OnOpened();
            firstShow = false;
        }

        if (!hasListenedEventForShowHide) {
            ProcessEventForShowHide(true);
            hasListenedEventForShowHide = true;
        }

        adapter.SetOrder(order);
        OnShow();
        FUIMgr.OnShow?.Invoke(uiType, cfg);
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
            FUIMgr.OnHide?.Invoke(uiType, cfg);
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

    private void SetName() {
#if UNITY_EDITOR
        if (transform != null) {
            transform.name = string.Format("{0} {1} {2}", uiType.ToString(), order.ToString(), cfg.uiTypeWithNamespace);
        }
#endif
    }

    private void _Loaded(AsyncOperation op) {
        _request.completed -= _Loaded;
        hasLoaded = true;

        GameObject clone = GameObject.Instantiate(_request.asset, FUIMgr.roots[cfg.layer].root.transform) as GameObject;
        if (clone == null) {
            Debug.LogError(string.Format("uiType {0}'s prefab {1} is not valid", uiType.ToString(), cfg.prefabPath));
            return;
        }

        transform = clone.transform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;

        SetName();

        if (!transform.TryGetComponent<UIInjector>(out var injector)) {
            injector = transform.gameObject.AddComponent<UIInjector>();
        }

        if (transform.TryGetComponent<FUICloser>(out FUICloser closer)) {
            closer.uiType = uiType;
        }

        injector.value = this;

        if (!transform.TryGetComponent<CanvasAdapter>(out var ad)) {
            ad = transform.gameObject.AddComponent<CanvasAdapter>();
        }

        adapter = ad;
        adapter.SetMode();

        transform.TryGetComponent<UIAudioPlayer>(out audioPlayer);
        transform.TryGetComponent<UIAnimController>(out animPlayer);

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
        Debug.LogError(string.Format("OnLoaded {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnTransfer(Tuple<ulong, ulong, ulong, object> arg) {
        // ui已经打开的时候调用OnTransfer
        Debug.LogError(string.Format("OnTransfer {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnOpen(Tuple<ulong, ulong, ulong, object> arg) {
        // ui没有打开的时候调用UIMgr.Open
        Debug.LogError(string.Format("OnOpen {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnClose() {
        Debug.LogError(string.Format("OnClose {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnOpened() {
        Debug.LogError(string.Format("OnOpened {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnShow() {
        Debug.LogError(string.Format("OnShow {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void OnHide() {
        Debug.LogError(string.Format("OnHide {0} {1}", uiType.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void ProcessEvent(bool toListen) {
        Debug.LogError(string.Format("ProcessEvent {0} {1} {2}", uiType.ToString(), toListen.ToString(), cfg.uiTypeWithNamespace));
    }

    protected virtual void ProcessEventForShowHide(bool toListen) {
        Debug.LogError(string.Format("ProcessEventForShowHide {0} {1} {2}", uiType.ToString(), toListen.ToString(), cfg.uiTypeWithNamespace));
    }
#endregion

}
