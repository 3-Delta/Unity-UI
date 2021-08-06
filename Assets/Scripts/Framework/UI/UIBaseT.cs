using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;

public interface IRegisterEvent {
    void RegisterEvent(bool toRegister);
}

[Serializable]
public class UIBaseT {
    public UIEntry entry;
    
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

    protected virtual void OnInit() {
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
