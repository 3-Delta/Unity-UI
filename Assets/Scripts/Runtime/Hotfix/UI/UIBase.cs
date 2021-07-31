using System;
using UnityEngine;

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