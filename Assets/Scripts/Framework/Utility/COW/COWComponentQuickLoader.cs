using System;
using System.Collections.Generic;
using UnityEngine;

// 一般工作在框架层
[DisallowMultipleComponent]
public class COWComponentQuickLoader<T> : MonoBehaviour where T : Component {
    public COWLoader<T> cowLoader;
}

[Serializable]
public class COWLoader<T> where T : Component {
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

    public COWLoader(GameObject proto, Transform parent) {
        this.proto = proto;
        this.parent = parent;
    }

    public COWLoader<T> TryBuildGo(int targetCount, Action<ComponentCell<T>, int /* index */> onInit) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            Transform clone = GameObject.Instantiate<GameObject>(proto.gameObject, parent).transform;
            clone.localPosition = Vector3.zero;
            clone.localEulerAngles = Vector3.zero;
            clone.localScale = Vector3.one;
            clone.gameObject.SetActive(false);

            TryAddGo(clone, onInit);
        }

        return this;
    }

    // 为了方便将一些特定的go添加进入，而不是统一样式的go
    public COWLoader<T> TryAddGo(Transform clone, Action<ComponentCell<T>, int /* index */> onInit) {
        if (!clone.TryGetComponent<ComponentCell<T>>(out var rlt)) {
            rlt = clone.gameObject.AddComponent<ComponentCell<T>>();
        }

        onInit?.Invoke(rlt, this.Count);

        this._components.Add(rlt);
        return this;
    }

    public COWLoader<T> TryRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onRefresh) {
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

    public COWLoader<T> TryBuildOrRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onInit, Action<ComponentCell<T>, int /* index */> onRefresh) {
        this.TryBuildGo(targetCount, onInit);
        return this.TryRefresh(targetCount, onRefresh);
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

    public COW<T> TrySet<P>(int targetCount, IList<P> list, Func<int /*index*/, P /*data*/, T> onCreate) {
        RealCount = targetCount;
        while (targetCount > Count) {
            int index = Count;
            T t = onCreate.Invoke(index, list[index]);
            
            this.ls.Add(t);
        }
        return this;
    }
    
    public COW<T> TrySet<TKey, TValue>(int targetCount, IDictionary<TKey, TValue> dict, Func<int /*index*/, IDictionary<TKey, TValue> /*data*/, T> onCreate) {
        RealCount = targetCount;
        while (targetCount > Count) {
            int index = Count;
            T t = onCreate.Invoke(index, dict);
            
            this.ls.Add(t);
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

