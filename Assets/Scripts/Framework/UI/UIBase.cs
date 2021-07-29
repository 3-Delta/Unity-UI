using System;
using UnityEngine;
using System.Runtime.CompilerServices;

[Serializable]
public class UIBaseT {
    public UIEntry entry;

    protected bool isEventHandled = false;

    public void Open(bool toOpen) {
        if (toOpen) {
            Open();
        }
        else {
            Close();
        }
    }

    private void Open() {
    }

    private void Close() {
    }

    public void Show(bool toShow) {
        if (toShow) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        // if (!isEventHandled) {
        //     isEventHandled = true;
        // }
    }

    private void Hide() {
    }

    #region 生命周期

    protected virtual void OnInit() {
        // 资源组件解析
    }

    protected virtual void OnOpen(ITuple arg) {
        // ui没有打开的时候调用UIMgr.Open
    }

    protected virtual void OnTransfer(ITuple arg) {
        // ui已经打开的时候调用UIMgr.Open
    }

    protected virtual void OnClose() {
    }

    protected virtual void OnShow() {
    }

    protected virtual void OnHide() {
    }

    protected virtual void OnReconnectBegin() {
        // 断线重连UI打开之前
    }

    protected virtual void OnReconnectEnd() {
        // 断线重连UI关闭之后
    }

    #endregion

    #region 事件

    protected virtual void HandleEvent(bool toRegister) {
    }

    #endregion
}

[Serializable]
public class UIBase<TLayout> : UIBaseT where TLayout : UILayoutBase, new() {
    [SerializeField] public TLayout layout;

    private void Load() {
        // 加载prefab资源
        Transform instance = null;

        this.layout = new TLayout();
        this.layout.TryInit(instance);

        OnLoaded();
    }

    private void OnLoaded() {
        OnInit();
    }
}