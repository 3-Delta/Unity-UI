using UnityEngine;

[DisallowMultipleComponent]
public class UniqueComponent<T> : MonoBehaviour where T : Component {
    public T value { get; protected set; } = null;

    private T _self = null;

    public T self {
        get {
            if (_self == null) {
                _self = GetComponent<T>();
            }

            return _self;
        }
    }
}