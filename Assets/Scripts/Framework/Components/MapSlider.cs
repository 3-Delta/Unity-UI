using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class MapSlider : MonoBehaviour {
    [Serializable]
    public class Ctrl {
        public Toggle toggle;
        public GameObject pageGo;
        public GameObject scaledGo;
    }

    public Slider slider;
    [Range(0.001f, 0.3f)] public float threshold = 0.01f;
    public bool considerMultiSmooth = true;
    [Range(0.1f, 5f)] public float singleSmoothTime = 1f;
    public Vector2 positiveDragScale = new Vector2(1f, 1.2f);
    public Vector2 negetiveDragScale = new Vector2(1f, 0.8f);

    public Ctrl[] ctrls = new Ctrl[0];

    public Action<int> onIndexed;
    public Action<bool, int, float> onSliding;

    private int _curIndex = 0;

    public int curIndex {
        get { return _curIndex; }
        private set {
            if (_curIndex != value) {
                int oldIndex = _curIndex;
                _curIndex = value;
                OnIndexChanged(oldIndex, value);
            }
        }
    }

    private float preValue = 0f;

    private void Awake() {
        slider.minValue = 0;
        slider.maxValue = ctrls.Length - 1;
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        for (int i = 0, length = ctrls.Length; i < length; ++i) {
            int index = i;
            ctrls[i].toggle.onValueChanged.AddListener((flag) => {
                if (flag) {
                    OnTabClicked(index, false);
                }
            });
        }

        preValue = slider.value;
    }

    private bool allow = false;
    private float from;
    private float to;
    private float startTime;

    private void Update() {
        if (allow) {
            float time = singleSmoothTime;
            if (considerMultiSmooth) {
                time = Mathf.Abs(to - from) * singleSmoothTime;
            }

            float rate = (Time.realtimeSinceStartup - startTime) / time;
            slider.value = Mathf.Lerp(from, to, rate);

            if (rate >= 1f) {
                allow = false;
            }
        }
    }

    private void OnIndexChanged(int oldIndex, int newIndex) {
        ctrls[newIndex].toggle.SetIsOnWithoutNotify(true);
        onIndexed?.Invoke(newIndex);
    }

    public void OnTabClicked(int index, bool force = false) {
        if (index < 0 || index >= ctrls.Length) {
            return;
        }

        if (force) {
            allow = false;
            slider.value = index;
        }
        else {
            allow = true;
            from = slider.value;
            to = index;
            startTime = Time.realtimeSinceStartup;
        }
    }

    private void OnSliderValueChanged(float value) {
        // 正向滑动，还是逆向滑动
        bool positive = value - preValue >= 0f;

        preValue = value;

        curIndex = CalcIndex(positive);

        float scale = LerpScale(positive);
        ctrls[curIndex].scaledGo.transform.localScale = new Vector3(scale, scale, scale);
        onSliding?.Invoke(positive, curIndex, slider.value);
    }

    private int CalcIndex(bool positive) {
        float value = slider.value;
        int ceil = Mathf.CeilToInt(value);
        bool isEnterCeil = ceil - value < threshold;
        int floor = Mathf.FloorToInt(value);
        bool isEnterFloor = value - floor < threshold;

        if (positive) {
            return isEnterCeil ? ceil : curIndex;
        }
        else {
            return isEnterFloor ? floor : curIndex;
        }
    }

    private float LerpScale(bool positive) {
        float value = slider.value;

        if (positive) {
            return Mathf.Lerp(positiveDragScale.x, positiveDragScale.y, value - curIndex);
        }
        else {
            // 负方向有两个
            if (value > curIndex) {
                return Mathf.Lerp(positiveDragScale.y, positiveDragScale.x, 1 - (value - curIndex));
            }
            else {
                return Mathf.Lerp(negetiveDragScale.x, negetiveDragScale.y, curIndex - value);
            }
        }
    }
}
