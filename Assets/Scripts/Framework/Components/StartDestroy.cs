using System;
using UnityEngine;

[DisallowMultipleComponent]
public class StartDestroy : MonoBehaviour {
    public Action<bool> onTrigger;

    private void Start() {
        onTrigger?.Invoke(true);
    }

    private void OnDestroy() {
        onTrigger?.Invoke(false);
        onTrigger = null;
    }
}
