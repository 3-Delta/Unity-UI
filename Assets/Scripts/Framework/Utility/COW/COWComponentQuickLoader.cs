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
}
