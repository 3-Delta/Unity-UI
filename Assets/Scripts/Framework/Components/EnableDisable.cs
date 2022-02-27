using System;
using UnityEngine;

[DisallowMultipleComponent]
public class EnableDisable : MonoBehaviour {
    public Action<bool> onTrigger;

    private void OnEnable() {
        onTrigger?.Invoke(true);
    }

    private void OnDisable() {
        onTrigger?.Invoke(false);
    }
}
