using System;
using System.Collections.Generic;
using UnityEngine;

// 一般工作在框架层
[DisallowMultipleComponent]
public class COWComponentLoader<T> : MonoBehaviour where T : Component {
    // 每x帧加载一个
    [Range(1, 99)] public int rate = 5;
    public GameObject proto;
    public Transform parent;

    public ComponentCell<T> this[int index] {
        get {
            if (0 <= index && index < this.Count) {
                return this._components[index];
            }

            return null;
        }
    }
#if UNITY_EDITOR
    public int Count;

    public int RealCount;
    public List<ComponentCell<T>> _components = new List<ComponentCell<T>>();
#else
    public int Count {
        get { return this._components.Count; }
    }

    public int RealCount { get; private set; }
    private readonly List<ComponentCell<T>> _components = new List<ComponentCell<T>>();
#endif

    private int _leftCount = 0;
    private Action<ComponentCell<T>, int /* index */> _onInit;
    private Action<ComponentCell<T>, int /* index */> _onRefresh;

    private void Awake() {
        enabled = false;
    }

    public COWComponentLoader<T> TryBuildOrRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onInit, Action<ComponentCell<T>, int /* index */> onRefresh) {
        RealCount = targetCount;

        _leftCount = targetCount - Count;
        _onInit = onInit;
        _onRefresh = onRefresh;

        if (_leftCount < 0) {
            for (int i = 0, length = Count; i < length; ++i) {
                ComponentCell<T> cur = this._components[i];
                if (i < targetCount) {
                    cur.gameObject.SetActive(true);
                    _onRefresh?.Invoke(cur, i);
                }
                else {
                    cur.gameObject.SetActive(false);
                }
            }
        }
        else {
            enabled = true;
        }

        return this;
    }

    private ComponentCell<T> BuildOne() {
        GameObject clone = Instantiate<GameObject>(proto, parent);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;
        clone.transform.localScale = Vector3.one;
        clone.SetActive(false);

        if (!clone.TryGetComponent<ComponentCell<T>>(out var rlt)) {
            rlt = clone.gameObject.AddComponent<ComponentCell<T>>();
        }

        return rlt;
    }

    private void Update() {
        if (_leftCount > 0) {
            if (Time.frameCount % rate == 0) {
                ComponentCell<T> cell = BuildOne();

                int index = Count;
                _onInit?.Invoke(cell, index);
                _onRefresh(cell, index);

                _components.Add(cell);
                --_leftCount;
            }
        }
        else {
            // 停止update
            enabled = false;
        }
    }
}
