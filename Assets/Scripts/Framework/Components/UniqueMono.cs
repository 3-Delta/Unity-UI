using System;
using UnityEngine;

// 唯一性组件
[DisallowMultipleComponent]
public class UniqueMono<T> : MonoBehaviour where T : Component {
    public bool CanReplace = false;

    public static T finalComponent { get; private set; }

    public T _component;
    public T component {
        get {
            if (_component != null) {
                if (!TryGetComponent(out _component)) {
                    _component = gameObject.AddComponent<T>();
                }
            }

            return _component;
        }
    }

    private void OnEnable() {
        TryRegister(true);
    }

    private void OnDisable() {
        TryRegister(false);
    }

    private void TryRegister(bool toRegister) {
        if (component != null) {
            if (toRegister) {
                if (finalComponent == null) {
                    finalComponent = component;
                }

                if (CanReplace) {
                    finalComponent = component;
                }
            }
            else {
                finalComponent = null;
            }
        }
    }
}

[DisallowMultipleComponent]
public class UniqueDefaultMono<T> : MonoBehaviour where T : Behaviour {
    public static T finalComponent { get; private set; }
    protected static T _defaultComponent = null;

    public T _component;
    public T component {
        get {
            if (_component != null) {
                if (!TryGetComponent(out _component)) {
                    _component = gameObject.AddComponent<T>();
                }
            }

            return _component;
        }
    }

    protected void Awake() {
        if (_defaultComponent != null) {
            if (!TryGetComponent(out _defaultComponent)) {
                _defaultComponent = gameObject.AddComponent<T>();
            }
        }
    }

    protected void OnEnable() {
        TryRegister(true);
    }

    protected void OnDisable() {
        TryRegister(false);
    }

    private void TryRegister(bool toRegister) {
        if (component != null) {
            if (toRegister) {
                finalComponent = component;
                if (finalComponent != _defaultComponent) {
                    _defaultComponent.enabled = false;
                }
            }
            else {
                _defaultComponent.enabled = true;
            }
        }
    }
}

// 因为auolistener全局只能保持一个，所以需要保证这一点
public class UniqueAudioListener : UniqueDefaultMono<AudioListener> {
    
}
