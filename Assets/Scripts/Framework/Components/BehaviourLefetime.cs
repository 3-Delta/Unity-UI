using System;
using UnityEngine;

[DisallowMultipleComponent]
public class BehaviourLefetime : MonoBehaviour {
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
        onDestroy?.Invoke();
    }

    private void OnDisable() {
        onDisable?.Invoke();
    }

    private void Update() {
        onUpdate?.Invoke();
    }
}