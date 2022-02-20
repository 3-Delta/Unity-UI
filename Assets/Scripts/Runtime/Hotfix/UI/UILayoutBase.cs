using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class UILayoutBase {
    public UILayoutBase TryBind(Transform transform) {
        this.FindByPath(transform);

        var binder = transform.GetComponent<UIBindComponents>();
        if (binder != null) {
            FindByIndex(binder);
        }

        return this;
    }

    protected virtual void FindByIndex(UIBindComponents binder) { }

    protected virtual void FindByPath(Transform transform) { }

    // public interface IListener { }
    // public void Listen(IListener listener) { }
}

// 用于实现空对象模式
[Serializable]
public class EmptyUILayout : UILayoutBase { }
