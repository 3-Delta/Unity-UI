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
    [Range(0.001f, 0.5f)] public float threshold = 0.01f;
    public Vector2 dragScale = new Vector2(1f, 1.2f);

    // public Vector2 scale = new Vector2(0.8f, 1f);
    public Ctrl[] ctrls = new Ctrl[0];

    public Action<int> onIndexed;
    public Action<bool, int, float> onSliding;

    private int _curIndex = 0;

    public int curIndex {
        get { return _curIndex; }
        private set {
            if (_curIndex != value) {
                _curIndex = value;
                Debug.LogError(_curIndex);
                onIndexed?.Invoke(_curIndex);
            }
        }
    }

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
        // 正向滑动，还是逆向滑动
        bool positive = value - preValue >= 0f;
        preValue = value;

        curIndex = CalcIndex();

        float scale = LerpScale(positive);
        ctrls[curIndex].scaledGo.transform.localScale = new Vector3(scale, scale, scale);
        onSliding?.Invoke(positive, curIndex, slider.value);
    }

    private int CalcIndex() {
        float value = slider.value;
        int ceil = Mathf.CeilToInt(value);
        int index = ceil - value < threshold ? ceil : ceil - 1;
        return index;
    }

    private float LerpScale(bool positive) {
        float value = slider.value;

        if (positive) {
            return Mathf.Lerp(dragScale.x, dragScale.y, value - curIndex);
        }

        return Mathf.Lerp(dragScale.y, dragScale.x, 1f - (value - curIndex));
    }
}
