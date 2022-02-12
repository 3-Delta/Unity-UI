using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class SliderChanger : TLerp<Slider> {
#if UNITY_EDITOR
    [SerializeField] private float max;
#else
    private float max;
#endif

    public Action onCompleted;
    public Action<float, float, float, float> onValueChanged;

    private float startTime;

    private void Awake() {
        enabled = false;
    }

    [ContextMenu("Begin")]
    public void Begin() {
        Begin(to, max, duration);
    }

    public void Begin(float to, float max, float duration = 0.8f) {
        this.from = component.value;
        this.to = to;
        this.max = max;
        this.duration = duration;

        enabled = true;
        _BaseBegin();
    }

    private void Update() {
        if (_first) {
            this.startTime = Time.time;
            _first = false;

            onValueChanged?.Invoke(from, from, to, max);
            return;
        }

        float diff = Time.time - startTime;
        if (diff <= duration) {
            if (refreshRate == ERefreshRate.PerFrame) {
                float current = Mathf.Lerp(from, to, diff / duration);
                float rate = current / max;
                component.value = rate;

                onValueChanged?.Invoke(current, from, to, max);
            }
            else if (refreshRate == ERefreshRate.PerSecond) { }
        }
        else {
            float current = to;
            float rate = current / max;
            component.value = rate;

            onValueChanged?.Invoke(current, from, to, max);
            onCompleted?.Invoke();

            enabled = false;
        }
    }
}
