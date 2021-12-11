using System;
using UnityEngine;

// 框架层提供给热更层的快捷口
[DisallowMultipleComponent]
public class MonoLefetime : MonoBehaviour {
    public Action onAwake;
    public Action onDestroy;
    public Action onEnable;
    public Action onDisable;

    public Action onUpdate;
    public Action onStart;

    private void Start() {
        onStart?.Invoke();
    }

    private void Awake() {
        onAwake?.Invoke();
    }

    private void OnDestroy() {
        onStart?.Invoke();
    }

    private void OnEnable() {
        onEnable?.Invoke();
    }

    private void OnDisable() {
        onDisable?.Invoke();
    }

    private void Update() {
        onUpdate?.Invoke();
    }
}
