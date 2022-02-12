using System;
using UnityEngine;
using UnityEngine.UI;

// 倒计时
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class TextCountDown : TLerp<Text> {
    [Range(0.01f, 999f)] public float cd = 5f;

    private float remainTime;
    private float startTime;

    public Action<Text, float, bool> onTimeRefresh;

    private void Awake() {
        enabled = false;
    }

    [ContextMenu("Begin")]
    public void Begin() {
       _BaseBegin();
    }

    [ContextMenu("End")]
    private void End() {
        _BaseEnd();
    }

    private void Update() {
        if (_first) {
            startTime = Time.time;
            remainTime = cd;
            _first = false;
            
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

        onTimeRefresh?.Invoke(component, remainTime, isEnd);
    }
}
