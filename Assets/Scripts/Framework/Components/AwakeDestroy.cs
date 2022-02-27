using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AwakeDestroy : MonoBehaviour {
    public Action<bool> onTrigger;

    private void Awake() {
        onTrigger?.Invoke(true);
    }

    private void OnDestroy() {
        onTrigger?.Invoke(false);
    }
}
