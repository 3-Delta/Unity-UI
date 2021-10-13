using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class MapSlider : MonoBehaviour {
    [Serializable]
    public class Ctrl {
        public GameObject tabGo;
        public GameObject scaledGo;
    }

    public Slider slider;

    [Range(0.001f, 0.1f)]  public float threshold = 0.01f;
    // public Vector2 scale = new Vector2(0.8f, 1f);
    public Ctrl[] ctrls = new Ctrl[0];

    // 是否在临界区域
    public bool inIndexRange { get; private set; } = true;
    public int curIndex { get; private set; } = 0;
    
    private float preValue = 0f;

    private void Awake() {
        slider.minValue = 0;
        slider.maxValue = ctrls.Length - 1;
        slider.onValueChanged.AddListener(OnValueChanged);
        
        preValue = slider.value;
    }

    public void OnTabClicked(int index) {
        if (index < 0 || index >= ctrls.Length) {
            return;
        }
        
        // trigger event
        slider.value = index;
    }
    
    private void OnValueChanged(float value) {
        inIndexRange = Approximately(slider.value, value);
        // 正向滑动，还是逆向滑动
        bool positive = value - preValue >= 0f;

        if (inIndexRange) {
            curIndex = Mathf.FloorToInt(value) + 1;
        }

        switch (slider.direction) {
            case Slider.Direction.BottomToTop:
                break;
            case Slider.Direction.TopToBottom:
                break;
            case Slider.Direction.LeftToRight:
                break;
            case Slider.Direction.RightToLeft:
                break;
        }
    }

    private bool Approximately(float left, float right) {
        return Mathf.Approximately(Mathf.Abs(left - right), threshold);
    }
}
