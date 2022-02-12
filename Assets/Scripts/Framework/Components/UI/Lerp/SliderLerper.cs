using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class SliderLerper : TLerp<Slider> {
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

    public void Begin(float to, float duration = 0.8f) {
        base.Begin(component.value, to, duration);
    }

    protected override void OnBegin() {
        component.value = from;
    }

    protected override void OnChange(float rate, float current) {
        component.value = current;
        base.OnChange(rate, current);
    }

    protected override void OnEnd() {
        component.value = to;
        base.OnEnd();
    }
}
