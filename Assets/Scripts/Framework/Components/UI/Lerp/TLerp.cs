using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TLerp<T> : MonoBehaviour where T : Component {
    public enum ERefreshRate {
        PerFrame,
        PerSecond,
        PerMinute,
    }

    public ERefreshRate refreshRate = ERefreshRate.PerSecond;

    private T _component;

    public T component {
        get {
            if (_component == null) {
                _component = GetComponent<T>();
            }

            return _component;
        }
    }

#if UNITY_EDITOR
    [SerializeField] protected float from;
    [SerializeField] protected float to;
    [SerializeField] [Range(0.05f, 99f)] protected float duration;
#else
    protected float from;
    protected float to;
    protected float duration;
#endif

    protected bool _first = false;
    private float _startTime;
    private float _accumulateTime;

    [ContextMenu("Begin")]
    public virtual void Begin() { }

    [ContextMenu("End")]
    public virtual void End() { }

    // 子类的begin需要调用该函数
    protected void _BaseBegin() {
        enabled = true;
        _first = true;
    }

    protected void _BaseEnd() {
        enabled = false;
    }

    protected virtual void OnInit() {
        /* 初始化记录数据 */
        _startTime = Time.time;
        _accumulateTime = 0f;
    }

    protected virtual void OnBegin() { }
    protected virtual void OnChange() { }
    protected virtual void OnEnd() { }

    protected void Update() {
        if (_first) {
            OnInit();
            OnBegin();

            _first = false;
        }
        else {
            float diff = Time.time - _startTime;
            if (diff <= duration) {
                if (refreshRate == ERefreshRate.PerFrame) {
                    OnChange();
                }
                else if (refreshRate == ERefreshRate.PerSecond) {
                    _accumulateTime += Time.deltaTime;
                    if (_accumulateTime >= 1f) {
                        OnChange();
                    }
                }
                else if (refreshRate == ERefreshRate.PerMinute) {
                    _accumulateTime += Time.deltaTime;
                    if (_accumulateTime >= 1f * 60) {
                        OnChange();
                    }
                }
            }
            else {
                OnEnd();
            }
        }
    }
}
