using System;
using System.Collections.Generic;
using UnityEngine;

// 一般工作在框架层
[DisallowMultipleComponent]
public class COWComponentQuickLoader<T> : MonoBehaviour where T : Component {
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

    private COWComponentQuickLoader<T> TryBuild(int targetCount, Action<T, int /* index */> onInit) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            GameObject clone = Instantiate<GameObject>(proto.gameObject, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            T t = clone.GetComponent<T>();
            onInit?.Invoke(t, this.Count);

            this.components.Add(t);
        }

        return this;
    }

    private COWComponentQuickLoader<T> TryRefresh(int targetCount, Action<T, int /* index */> onRefresh) {
        this.RealCount = targetCount;
        int componentCount = this.Count;
        for (int i = 0; i < componentCount; ++i) {
            T cur = this.components[i];
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

    public COWComponentQuickLoader<T> TryBuildOrRefresh(int targetCount, Action<T, int /* index */> onInit, Action<T, int /* index */> onRrfresh) {
        this.TryBuild(targetCount, onInit);
        return this.TryRefresh(targetCount, onRrfresh);
    }
}
