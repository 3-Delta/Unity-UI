using System;
using System.Collections.Generic;

public struct ValueAssigner<T> where T : IEquatable<T> {
    public Action<T, T> onValueChanged;

    private T _value;

    public T value {
        get { return _value; }
        set {
            if(!EqualityComparer<T>.Default.Equals(this._value, value)) {
                T oldValue = this._value;
                this._value = value;
                onValueChanged?.Invoke(oldValue, value);
            }
        }
    }

    public ValueAssigner(T v, Action<T, T> onValueChanged = null) {
        this._value = v;
        this.onValueChanged = onValueChanged;
    }
}
