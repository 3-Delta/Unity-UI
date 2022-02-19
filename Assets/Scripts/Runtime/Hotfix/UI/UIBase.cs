using System;
using UnityEngine;

[Serializable]
public class UIBase : FUIBase {
    
}

[Serializable]
public class UIBaseWithLayout<TLayout> : UIBase where TLayout : UILayoutBase, new() {
    [SerializeField] protected TLayout layout;

    protected override void OnLoaded(Transform transform) {
        this.layout = new TLayout();
        this.layout.TryBind(transform);
    }
}
