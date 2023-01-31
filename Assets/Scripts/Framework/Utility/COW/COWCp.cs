using System;
using System.Collections.Generic;
using UnityEngine;

// 可以被序列化的COWGo
[Serializable]
public class COWGo {
    [SerializeField] public Transform proto;
    [SerializeField] public Transform parent;

    public Transform this[int index] {
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
    public List<Transform> _components = new List<Transform>();
#else
    public int Count {
        get { return this._components.Count; }
    }

    public int RealCount { get; private set; }
    private readonly List<Transform> _components = new List<Transform>();
#endif

    public COWGo TryBuildGo(int targetCount, Action<Transform, int /* index */> onInit = null) {
        proto.gameObject.SetActive(false);
        while (targetCount > this.Count) {
            Transform clone = GameObject.Instantiate<GameObject>(proto.gameObject, parent).transform;
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            TryAddGo(clone, onInit);
        }

        return this;
    }

    // 为了方便将一些特定的go添加进入，而不是统一样式的go
    public COWGo TryAddGo(Transform clone, Action<Transform, int /* index */> onInit = null) {
        onInit?.Invoke(clone, this.Count);
        this._components.Add(clone);
        return this;
    }

    public COWGo TryRefresh(int targetCount, Action<Transform, int /* index */> onRefresh = null) {
        this.RealCount = targetCount;
        int componentCount = this.Count;
        for (int i = 0; i < componentCount; ++i) {
            Transform cur = this._components[i];
            if (i < targetCount) {
                cur.gameObject.SetActive(true);
                onRefresh?.Invoke(cur, i);
            }
            else {
                cur.gameObject.SetActive(false);
            }
        }

        return this;
    }

    public COWGo TryBuildOrRefresh(int targetCount, Action<Transform, int /* index */> onInit = null, Action<Transform, int /* index */> onRefresh = null) {
        this.TryBuildGo(targetCount, onInit);
        return this.TryRefresh(targetCount, onRefresh);
    }
}

// 这个类居然在unity的inspector下序列化不出来，只能手写一个固定的
[Serializable]
public class COWGo<T> where T : Component {
    [SerializeField] public T proto;
    [SerializeField] public Transform parent;

    public T this[int index] {
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
    public List<T> _components = new List<T>();
#else
    public int Count {
        get { return this._components.Count; }
    }

    public int RealCount { get; private set; }
    private readonly List<T> _components = new List<T>();
#endif

    public COWGo<T> TryBuildGo(int targetCount, Action<T, int /* index */> onInit) {
        proto.gameObject.SetActive(false);
        while (targetCount > this.Count) {
            T clone = GameObject.Instantiate<GameObject>(proto.gameObject, parent).GetComponent<T>();
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;

            TryAddGo(clone, onInit);
        }

        return this;
    }

    // 为了方便将一些特定的go添加进入，而不是统一样式的go
    public COWGo<T> TryAddGo(T clone, Action<T, int /* index */> onInit) {
        onInit?.Invoke(clone, this.Count);
        this._components.Add(clone);
        return this;
    }

    public COWGo<T> TryRefresh(int targetCount, Action<T, int /* index */> onRefresh) {
        this.RealCount = targetCount;
        int componentCount = this.Count;
        for (int i = 0; i < componentCount; ++i) {
            T cur = this._components[i];
            if (i < targetCount) {
                cur.gameObject.SetActive(true);
                onRefresh?.Invoke(cur, i);
            }
            else {
                cur.gameObject.SetActive(false);
            }
        }

        return this;
    }

    public COWGo<T> TryBuildOrRefresh(int targetCount, Action<T, int /* index */> onInit, Action<T, int /* index */> onRefresh) {
        this.TryBuildGo(targetCount, onInit);
        return this.TryRefresh(targetCount, onRefresh);
    }
}
