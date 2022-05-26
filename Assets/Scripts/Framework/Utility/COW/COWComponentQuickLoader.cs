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

    private COWLoader<T> TryBuild(int targetCount, Action<ComponentCell<T>, int /* index */> onInit) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            Transform clone = GameObject.Instantiate<GameObject>(proto.gameObject, parent).transform;
            clone.localPosition = Vector3.zero;
            clone.localEulerAngles = Vector3.zero;
            clone.localScale = Vector3.one;
            clone.gameObject.SetActive(false);

            if (!clone.TryGetComponent<ComponentCell<T>>(out var rlt)) {
                rlt = clone.gameObject.AddComponent<ComponentCell<T>>();
            }

            onInit?.Invoke(rlt, this.Count);

            this._components.Add(rlt);
        }

        return this;
    }

    private COWLoader<T> TryRefresh(int targetCount, Action<ComponentCell<T>, int /* index */> onRefresh) {
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
        this.TryBuild(targetCount, onInit);
        return this.TryRefresh(targetCount, onRefresh);
    }
}
