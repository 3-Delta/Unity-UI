using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIMethodInvoker : MethodInvoker<FUIBase> {
    private void Reset() {
        hideFlags = HideFlags.DontSaveInBuild;
    }

    [ContextMenu(nameof(OnOpened))]
    private void OnOpened() {
        var m = GetMethod(nameof(OnOpened));
        m?.Invoke(this.instance, null);
    }
    
    [ContextMenu(nameof(OnShow))]
    private void OnShow() {
        var m = GetMethod(nameof(OnShow));
        m?.Invoke(this.instance, null);
    }
    
    [ContextMenu(nameof(CloseSelf))]
    private void CloseSelf() {
        var m = GetMethod(nameof(CloseSelf));
        m?.Invoke(this.instance, null);
    }

    [ContextMenu(nameof(InvokeMethod))]
    public override void InvokeMethod() {
        base.InvokeMethod();
    }
}
