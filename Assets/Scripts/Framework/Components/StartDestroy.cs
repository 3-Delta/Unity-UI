using System;
using UnityEngine;

[DisallowMultipleComponent]
public class StartDestroy : MonoBehaviour {
    public bool awakeStart = false;
    public Action<bool> onTrigger;

    private void Awake() {
        if (awakeStart) {
            onTrigger?.Invoke(true);
        }
    }

    private void Start() {
        if (!awakeStart) {
            onTrigger?.Invoke(true);
        }
    }

    private void OnDestroy() {
        onTrigger?.Invoke(false);
        onTrigger = null;
    }
}
