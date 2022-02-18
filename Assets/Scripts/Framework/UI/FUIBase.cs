using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;

public interface IRegisterEvent {
    void RegisterEvent(bool toRegister);
}

public interface IListenReconnect {
    void OnBeginReconnect();
    void OnEndReconnect();
}

[Serializable]
public class FUIBase : IListenReconnect {
    public int uiType;
    public UIEntry cfg;

    public int order;

    // UIEntry能否将UIEntry设置为表格填写的形式，也就是提剔除uiconfig
    // 因为有时候，可能需要在B ui打开的时候，将前面的A ui关闭掉。所以需要外部设置这些回调
    public Action<UIEntry> onOpen { get; set; }
    public Action<UIEntry> onClose { get; set; }

    public Action<UIEntry> onBeginShow { get; set; }
    public Action<UIEntry> onEndShow { get; set; }
    public Action<UIEntry> onBeginHide { get; set; }
    public Action<UIEntry> onEndHide { get; set; }

    public void Init(int uiType, UIEntry cfg) {
        this.uiType = uiType;
        this.cfg = cfg;
    }

    public void Open(bool toOpen) {
        if (toOpen) {
            Open();
        }
        else {
            Close();
        }
    }

    public void Show(bool toShow) {
        if (toShow) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Open() {
    }

    private void Close() {
    }

    private void Show() {
    }

    private void Hide() {
    }

    public virtual void BlcokRaycast(bool toBlcok) {
        // 防止点击事件
    }

    #region 生命周期

    protected virtual void OnLoaded(Transform transform) {
        // 资源组件解析
    }

    protected virtual void OnOpen(Object arg) {
        // ui没有打开的时候调用UIMgr.Open
    }

    protected virtual void OnTransfer(Object arg) {
        // ui已经打开的时候调用UIMgr.Open
    }

    protected virtual void OnClose() {
    }

    protected virtual void OnShow() {
    }

    protected virtual void OnHide() {
    }

    public virtual void OnBeginReconnect() {
        // 断线重连UI打开之前
    }
    
    public virtual void OnEndReconnect() {
        // 断线重连UI关闭之后
    }

    #endregion

    #region 事件

    protected virtual void HandleEvent(bool toRegister) {
    }

    #endregion
}
