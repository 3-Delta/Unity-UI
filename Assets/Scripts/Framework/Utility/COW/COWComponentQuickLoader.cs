using System;
using System.Collections.Generic;
using UnityEngine;

// 一般工作在框架层
[DisallowMultipleComponent]
public class COWComponentQuickLoader<T> : MonoBehaviour where T : Component {
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

    private COWComponentQuickLoader<T> TryBuild(int targetCount, Action<ComponentCell<T>, int /* index */> onInit) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            GameObject clone = Instantiate<GameObject>(proto.gameObject, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            if (!clone.TryGetComponent<ComponentCell<T>>(out var rlt)) {
                rlt = clone.AddComponent<ComponentCell<T>>();
            }

            onInit?.Invoke(rlt, this.Count);

            this._components.Add(rlt);
        }

        return this;
    }

    private COWComponentQuickLoader<T> TryRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onRefresh) {
        this.RealCount = targetCount;
        int componentCount = this.Count;
        for (int i = 0; i < componentCount; ++i) {
            ComponentCell<T> cur = this._components[i];
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

    public COWComponentQuickLoader<T> TryBuildOrRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onInit, Action<ComponentCell<T>, int /* index */> onRrfresh) {
        this.TryBuild(targetCount, onInit);
        return this.TryRefresh(targetCount, onRrfresh);
    }

    public void Add(ComponentCell<T> cell, bool activeRealCount) {
        this._components.Add(cell);

        if (activeRealCount) {
            this.RealCount += 1;
        }
    }

    public void Clear() {
        this.RealCount = 0;
    }
}

// 数据层面的复用
public class COW<T> {
    private readonly List<T> ls = new List<T>();
    
    public int Count {
        get { return this.ls.Count; }
    }

    public int RealCount { get; private set; }

    public T this[int index] {
        get { return ls[index]; }
    }

    public void Clear() {
        ls.Clear();
        RealCount = 0;
    }
    
    public COW<T> TrySet<P>(int targetCount, IList<P> list, Func<int /*index*/, T> onCreate, Action<int /*index*/, P /*data*/, T> onRefresh) {
        RealCount = targetCount;
        while (targetCount > Count) {
            int index = Count;
            T t = onCreate.Invoke(index);
            
            this.ls.Add(t);
        }
        
        for (int i = 0, length = targetCount; i < length; ++i) {
            onRefresh?.Invoke(i, list[i], ls[i]);
        }
        return this;
    }
    
    public COW<T> TrySet<TKey, TValue>(int targetCount, IDictionary<TKey, TValue> dict, Func<int /*index*/, T> onCreate, Action<int, KeyValuePair<TKey, TValue> /*data*/, T> onRefresh) {
        RealCount = targetCount;
        while (targetCount > Count) {
            int index = Count;
            T t = onCreate.Invoke(index);
            
            this.ls.Add(t);
        }

        int i = 0;
        foreach (var kvp in dict) {
            onRefresh?.Invoke(i, kvp, ls[i]);
            ++i;
        }
        return this;
    }

    public void Add(T t, bool activeRealCount) {
        this.ls.Add(t);
        if (activeRealCount) {
            RealCount += 1;
        }
    }
}

