using System;
using UnityEngine;
using UnityEngine.UI;

// 倒计时
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class CDText : MonoBehaviour {
    public enum ERefreshRate {
        PerFrame,
        PerSecond,
        PerMinute,
    }

    public ERefreshRate refreshRate = ERefreshRate.PerSecond;
    [Range(0.01f, 999f)] public float cd = 5f;

    private float remainTime;
    private float startTime;
    private bool first;

    private Text _text;

    public Text text {
        get {
            if (_text == null) {
                _text = GetComponent<Text>();
            }

            return _text;
        }
    }

    public Action<Text, float, bool> onTimeRefresh;

    private void Awake() {
        enabled = false;
    }

    [ContextMenu("Begin")]
    public void Begin() {
        enabled = true;

        first = true;
    }

    [ContextMenu("End")]
    private void End() {
        enabled = false;
    }

    private void Update() {
        if (first) {
            startTime = Time.time;
            remainTime = cd;
            first = false;
            
            _OnTimeRefresh(false);
        }
        else {
            remainTime -= Time.deltaTime;
            bool isEnd = remainTime <= 0f;

            if (refreshRate == ERefreshRate.PerFrame) {
                _OnTimeRefresh(false);
            }
            else if (refreshRate == ERefreshRate.PerSecond) {
                if (Time.time - startTime >= 1f) {
                    startTime = Time.time;
                    _OnTimeRefresh(false);
                }
            }
            else if (refreshRate == ERefreshRate.PerMinute) {
                if (Time.time - startTime >= 1f * 60) {
                    startTime = Time.time;
                    _OnTimeRefresh(false);
                }
            }

            if (isEnd) {
                _OnTimeRefresh(true);
                End();
            }
        }
    }

    private void _OnTimeRefresh(bool isEnd) {
        // if (isEnd) {
        //     text.text = "0";
        // }
        // else {
        //     text.text = remainTime.ToString();
        // }

        onTimeRefresh?.Invoke(text, remainTime, isEnd);
    }
}
