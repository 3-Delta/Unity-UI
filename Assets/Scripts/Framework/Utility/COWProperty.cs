using System;
using System.Collections.Generic;

// 其实本质上就是: Lazy<T>
public class COWProperty<T> {
    private T _value;

    public Action<T, T> onValueChanged;

    // 强制重新计算
    public bool needReCalculate;

    public T Value {
        get {
            if (_value == null || this.needReCalculate) {
                _value = _getAction.Invoke();
            }

            return _value;
        }
        set {
            if (!EqualityComparer<T>.Default.Equals(_value, value)) {
                T old = _value;
                _value = value;
                onValueChanged?.Invoke(old, _value);
            }
        }
    }

    private Func<T> _getAction = null;

    public COWProperty(Func<T> getAction) {
        this._getAction = getAction;
    }
}