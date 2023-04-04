using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class FUILayoutBase
#if UNITY_EDITOR
    // 编辑器下可视化方便
    : MonoBehaviour
#endif
{
#if UNITY_EDITOR
    private void Reset() {
        this.hideFlags = HideFlags.DontSaveInBuild;
        this.FindByPath(transform, true);
    }

    [ContextMenu(nameof(SyncToBinder))]
    private void SyncToBinder() {
        if (!transform.TryGetComponent<UIBindComponents>(out UIBindComponents r)) {
            r = transform.gameObject.AddComponent<UIBindComponents>();
        }

        r.fieldStyle = UIBindComponents.ECSharpFieldStyle.Field;
        r.bindComponents = this.Collect();
    }

    private List<UIBindComponents.BindComponent> Collect() {
        var fis = this.GetType().GetFields();
        List<UIBindComponents.BindComponent> array = new List<UIBindComponents.BindComponent>();
        foreach (var fi in fis) {
            if (!fi.FieldType.IsSubclassOf(typeof(UnityEngine.Component))) {
                continue;
            }

            var cp = fi.GetValue(this) as UnityEngine.Component;
            var a = new UIBindComponents.BindComponent() {
                component = cp,
                toListen = true,
                name = fi.Name
            };
            array.Add(a);
        }

        return array;
    }
#endif
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
