using System;

using UnityEngine;

[DisallowMultipleComponent]
public class DefaultCommponent<T> where T : Component {
    public T defaultValue;
    public T currentValue;

    public Action<T, T> onChanged;

}
