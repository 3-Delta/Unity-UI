using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DefaultCommponent<T> : MonoBehaviour where T : Component {
    public static T defaultValue { get; private set; } = null;
    public static T finalValue { get; private set; } = null;
    
    private T _self = null;

    public T self {
        get {
            if (_self == null) {
                _self = GetComponent<T>();
            }

            return _self;
        }
    }
    
    private void OnEnable() {
        if (finalValue != null) {
            finalValue = self;
        }
        else {
            defaultValue = finalValue = self;
        }
    }

    private void OnDisable() {
        if (finalValue != null) {
            finalValue = self;
        }
    }
}