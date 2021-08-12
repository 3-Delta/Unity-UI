using System;
using System.Collections.Generic;
using UnityEngine;

// 用于处理爱心区域，进入/离开 区域的动态检测
// 更加核心的功能是： 将Update中一直检测的形式 转换为 event形式
public class StateChecker<T> {
    private Func<bool> checker;
    public T owner { get; private set; }

    public Action<T, bool, bool> onValueChanged;

    private int _checkFrequency = 1;

    public int checkFrequency {
        get { return _checkFrequency; }
        set {
            _checkFrequency = value;
            if (_checkFrequency <= 0) {
                _checkFrequency = 1;
            }
        }
    }

    private bool _state = false;

    public bool state {
        get { return _state; }
        private set {
            if (_state != value) {
                bool old = _state;
                _state = value;
                onValueChanged?.Invoke(owner, old, _state);
            }
        }
    }

    public StateChecker(T owner, bool originalState, Func<bool> checker, Action<T, bool, bool> onValueChanged,
        int frequency = 1) {
        this.owner = owner;
        this.checker = checker;
        this.onValueChanged = onValueChanged;

        _state = originalState;
        _checkFrequency = frequency;
    }

    public StateChecker(T owner, Func<bool> originalState, Func<bool> checker, Action<T, bool, bool> onValueChanged,
        int frequency = 1) {
        this.owner = owner;
        this.checker = checker;
        this.onValueChanged = onValueChanged;

        _state = originalState.Invoke();
        _checkFrequency = frequency;
    }

    public void Check() {
        if (Time.frameCount % checkFrequency == 0) {
            state = checker.Invoke();
        }
    }
}