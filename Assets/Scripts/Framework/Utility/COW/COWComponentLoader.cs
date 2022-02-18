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

    public int Count {
        get { return this.components.Count; }
    }

    public int RealCount { get; private set; }

    public T this[int index] {
        get {
            if (0 <= index && index < this.Count) {
                return this.components[index];
            }

            return null;
        }
    }

    private readonly List<T> components = new List<T>();

    private int _leftCount = 0;
    private Action<T, int /* index */> _onInit;
    private Action<T, int /* index */> _onRefresh;

    private void Awake() {
        enabled = false;
    }

    public COWComponentLoader<T> TryBuildOrRefresh(int targetCount, Action<T, int /* index */> onInit, Action<T, int /* index */> onRefresh) {
        RealCount = targetCount;

        _leftCount = targetCount - Count;
        _onInit = onInit;
        _onRefresh = onRefresh;

        if (_leftCount < 0) {
            for (int i = 0, length = Count; i < length; ++i) {
                T cur = this.components[i];
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

    private T BuildOne() {
        GameObject clone = Instantiate<GameObject>(proto, parent);
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localEulerAngles = Vector3.zero;
        clone.transform.localScale = Vector3.one;
        clone.SetActive(false);

        T t = clone.GetComponent<T>();
        return t;
    }

    private void Update() {
        if (_leftCount > 0) {
            if (Time.frameCount % rate == 0) {
                T t = BuildOne();

                int index = Count;
                _onInit?.Invoke(t, index);
                _onRefresh(t, index);

                components.Add(t);
                --_leftCount;
            }
        }
        else {
            // 停止update
            enabled = false;
        }
    }
}
