using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class MapSlider : MonoBehaviour {
    [Serializable]
    public class Ctrl {
        public GameObject tabGo;
        public GameObject scaledGo;
    }

    public struct Gap {
        public Vector2Int indexRange;
        public Vector2 range;

        public float mid {
            get {
                return (range.y - range.x) * 0.5f;
            }
        }

        public void OnTriggerMid(Slider.Direction dir, bool positiveOrNegative) { 

        }
    }

    public Slider slider;

    //[Range(0.001f, 0.1f)]  public float threshold = 0.01f;
    [Range(2, 9)] public int range = 4;

    public Ctrl[] ctrls = null;

    private Gap[] gaps;
    private float preValue;

    private void Awake() {
        preValue = 0;
        ctrls = new Ctrl[range];
        gaps = new Gap[range - 1];
        for (int i = 0; i < range - 1; i++) {
            gaps[i].indexRange = new Vector2Int(i, i + 1);
            float min = 1f * i / (range - 1);
            float max = 1f * (i + 1) / (range - 1);
            gaps[i].range = new Vector2(min, max);
        }

        slider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(float value) {
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
}
