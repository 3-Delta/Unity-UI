using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class TextScrollNum : TLerp<Text> {
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

    protected override void OnBegin() {
        component.text = from.ToString();
        base.OnBegin();
    }

    protected override void OnLerp(float rate, float current) {
        component.text = Mathf.RoundToInt(current).ToString();
        base.OnLerp(rate, current);
    }

    protected override void OnEnd() {
        component.text = to.ToString();
        base.OnEnd();
    }
}
