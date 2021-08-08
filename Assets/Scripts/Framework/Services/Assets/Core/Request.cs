using System;
using System.Collections.Generic;

using UnityEngine;

public enum ELoadStage {
    WaitToLoad = 0,
    BeginLoad = 1,
    Loading = 2,
    DependencyLoading = 3,
    EndLoadFial = 4,
    EndLoadSuccess = 5,
}

// 所有可加载资源的基类
public class Request<T> where T : Request<T> {
    // 引用计数为0回收的，等待被销毁
    private static readonly List<T> Unused = new List<T>();

    public RefCounter refCounter { get; protected set; } = new RefCounter();
    public string path;
    public ELoadStage loadStage { get; protected set; } = ELoadStage.WaitToLoad;

    protected Action<T> onCompleted;

    public bool IsDone {
        get {
            return loadStage == ELoadStage.EndLoadSuccess || loadStage == ELoadStage.EndLoadSuccess;
        }
    }

    public bool IsSuccess {
        get {
            return loadStage == ELoadStage.EndLoadSuccess;
        }
    }

    #region 对外接口
    // 加载：同步/异步
    public void Load() {
        OnBeginLoad();
        OnLoaded(true);
    }

    public void LoadAsync(Action<T> onLoaded) {
        OnBeginLoad();
    }

    public void Unload() {

    }
    #endregion

    #region 生命周期
    // 开始加载
    protected virtual void OnBeginLoad() {

    }

    // 加载完毕，成功/失败
    protected virtual void OnLoaded(bool result) {

    }

    // 卸载， 可能没有加载结束的时候，就人为的进行来卸载
    protected virtual void OnUnload() {

    }

    // 引用计数为0回调
    protected virtual void OnUnused() {

    }
    #endregion
}
