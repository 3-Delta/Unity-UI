using System;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ScrollbarMoveTo : MonoBehaviour {
    public Scrollbar slider;
    [Range(0.01f, 99f)] public float duration = 0.3f;

    // private Timer timer;
    private float beginValue;
    private float endValue;
    
    private void Awake() {
        if (slider == null) {
            slider = GetComponent<Scrollbar>();
        }
    }

    private void OnDestroy() {
        // timer?.Cancel();
    }

    public void MoveTo(float value) {
        this.beginValue = slider.value;
        this.endValue = value;
        // this.timer?.Cancel();

        if (duration <= 0f) {
            this.slider.value = endValue;
        }
        else {
            // this.timer = Timer.Create(ref timer, duration, OnDone, OnDoing);
        }
    }

    private void OnDone() {
        this.slider.value = endValue;
    }

    private void OnDoing(float timeSinceDo) {
        // float rate = dt / Timer.duration;
        // float value = Mathf.Lerp(beginValue, endValue, rate);
        // this.slider.value = value;
    }
}
