using System;
using UnityEngine;
using UnityEngine.UI;

// 倒计时
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class TextCountDown : TLerp<Text> {
    private void Awake() {
        enabled = false;
    }
    
    [ContextMenu("Begin")]
    public void Begin() {
        Begin(from, to, duration);
    }

    [ContextMenu("End")]
    public void End() {
        enabled = false;
    }

    public override void Begin(float from, float to, float duration = 0.8f) {
        Begin(from);
    }

    public void Begin(float from) {
        base.Begin(from, 0f, from);
    }
}
