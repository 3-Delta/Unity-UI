using System;
using UnityEngine;

[Serializable]
public abstract class FUILayoutBase
#if UNITY_EDITOR
    // 编辑器下可视化方便
    : MonoBehaviour
#endif
{
    public FUILayoutBase TryBind(Transform transform) {
        this.FindByPath(transform);

        var binder = transform.GetComponent<UIBindComponents>();
        if (binder != null) {
            FindByIndex(binder);
        }

        return this;
    }

    protected virtual void FindByIndex(UIBindComponents binder) { }

    protected virtual void FindByPath(Transform transform, bool check = false) { }
}

// 用于实现空对象模式
[Serializable]
public class FEmptyUILayout : FUILayoutBase { }
