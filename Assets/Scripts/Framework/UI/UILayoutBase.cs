using System;
using UnityEngine;

[Serializable]
public abstract class UILayoutBase : IRegisterEvent {
    private bool isFind = false;
    public Transform transform { get; protected set; }

    public UILayoutBase() {
        this.isFind = false;
    }

    public void TryInit(Transform transform) {
        this.transform = transform;
        if (!isFind) {
            OnInit();
            isFind = true;
        }
    }

    protected virtual void OnInit() {
    }

    public virtual void RegisterEvent(bool toRegister) {
    }
}

// 用于实现空对象模式
[Serializable]
public class DefaultUILayout : UILayoutBase {
}