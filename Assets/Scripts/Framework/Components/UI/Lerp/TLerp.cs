using System;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class TLerp<T> : MonoBehaviour where T : Component {
    public enum ERefreshRate {
        PerFrame,
        PerSecond,
        PerMinute,
    }

    public bool useTimeScale = true;
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
    [SerializeField] [Range(0.05f, 99f)] protected float duration = 0.3f;
#else
    protected float from;
    protected float to;
    protected float duration = 0.3f;
#endif

    [SerializeField] protected float _current;

    protected bool _first = false;
    private float _startTime;
    private double _accumulateTime;

    public Action<float> onBegin;
    public Action<float> onEnd;
    public Action<float, float, float, float> onLerp;

    public virtual void Begin(float from, float to, float duration = 0.8f) {
        this.from = from;
        this.to = to;
        this.duration = duration;

        enabled = true;
        _first = true;
    }

    protected virtual void OnInit() {
        /* 初始化记录数据 */
        _startTime = useTimeScale ? Time.time : Time.unscaledTime;
        _accumulateTime = 0;
    }

    protected virtual void OnBegin() {
        onBegin?.Invoke(from);
    }

    protected virtual void OnLerp(float rate, float current) {
        onLerp?.Invoke(current, rate, from, to);
    }

    protected virtual void OnEnd() {
        onEnd?.Invoke(to);
        enabled = false;
    }

    protected void Update() {
        if (_first) {
            OnInit();
            OnBegin();

            _first = false;
        }
        else {
            float diff = (useTimeScale ? Time.time : Time.unscaledTime) - _startTime;
            if (diff <= duration) {
                if (refreshRate == ERefreshRate.PerFrame) {
                    float rate = diff / duration;
                    _current = Mathf.Lerp(from, to, rate);
                    OnLerp(rate, _current);
                }
                else if (refreshRate == ERefreshRate.PerSecond) {
                    _accumulateTime += (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);

                    if (_accumulateTime >= 1) {
                        _accumulateTime = 0;
                        float rate = diff / duration;
                        _current = Mathf.Lerp(from, to, rate);
                        OnLerp(rate, _current);
                    }
                }
                else if (refreshRate == ERefreshRate.PerMinute) {
                    _accumulateTime += (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime);

                    if (_accumulateTime >= 1 * 60) {
                        _accumulateTime = 0;
                        float rate = diff / duration;
                        _current = Mathf.Lerp(from, to, rate);
                        OnLerp(rate, _current);
                    }
                }
            }
            else {
                OnEnd();
            }
        }
    }
}
