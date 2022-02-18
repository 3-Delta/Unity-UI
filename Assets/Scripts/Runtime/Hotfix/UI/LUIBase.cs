using System;
using UnityEngine;

[Serializable]
public class LUIBase : FUIBase { }

[Serializable]
public class UIBaseWithLayout<TLayout> : LUIBase where TLayout : FUILayoutBase, new() {
    [SerializeField] protected TLayout layout;

    protected override void OnLoaded(Transform transform) {
        this.layout = new TLayout();
        this.layout.TryBind(transform);
    }
}
