using System;
using UnityEngine;

// 框架层提供给热更层的快捷口
[DisallowMultipleComponent]
public class MonoLifetime : MonoBehaviour {
    public Action onAwake;
    public Action onDestroy;
    public Action onEnable;
    public Action onDisable;

    public Action onLateUpdate;
    public Action onStart;

    private void Awake() {
        onAwake?.Invoke();
    }

    private void OnDestroy() {
        onDestroy?.Invoke();
    }

    private void Start() {
        onStart?.Invoke();
    }

    private void OnEnable() {
        onEnable?.Invoke();
    }

    private void OnDisable() {
        onDisable?.Invoke();
    }

    private void LateUpdate() {
        onLateUpdate?.Invoke();
    }
}
