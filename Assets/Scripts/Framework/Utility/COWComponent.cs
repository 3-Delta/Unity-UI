using System;
using System.Collections.Generic;
using UnityEngine;

// 一般工作在框架层
[DisallowMultipleComponent]
public class COWComponent<T> : MonoBehaviour where T : Component {
    private readonly List<T> components = new List<T>();

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

    public COWComponent<T> TryBuild(GameObject proto, Transform parent, int targetCount, Action<T, int /* index */> onInit) {
        proto.SetActive(false);
        while (targetCount > this.Count) {
            GameObject clone = Instantiate<GameObject>(proto, parent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localEulerAngles = Vector3.zero;
            clone.transform.localScale = Vector3.one;
            clone.SetActive(false);

            T t = clone.GetComponent<T>();
            onInit?.Invoke(t, this.Count - 1);

            this.components.Add(t);
        }

        return this;
    }

    public COWComponent<T> TryRefresh(int targetCount, Action<T, int /* index */> onRefresh) {
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

    public COWComponent<T> TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount, Action<T, int /* index */> onInit, Action<T, int /* index */> onRrfresh) {
        this.TryBuild(proto, parent, targetCount, onInit);
        return this.TryRefresh(targetCount, onRrfresh);
    }

    public void Clear() {
        for (int i = 0, count = this.Count; i < count; ++i) {
            T cur = this.components[i];
            GameObject.Destroy(cur);
        }

        this.components.Clear();
    }
}

[DisallowMultipleComponent]
public class COWComponentTabCollector<T> : MonoBehaviour where T : Component {
    public COWComponent<T> cow { get; private set; } = new COWComponent<T>();

    public int Count {
        get { return this.cow.RealCount; }
    }

    public T this[int index] {
        get {
            if (0 <= index && index < this.Count) {
                return this.cow[index];
            }

            return null;
        }
    }

    public void Clear() {
        this.cow.Clear();
    }

    // onInit函数主要作用就是达成和cowVD一样的作用，在外部构建一个vd的字典进行刷新
    public COWComponentTabCollector<T> TryBuildOrRefresh(GameObject proto, Transform parent, int targetCount,
        Action<T, int /* index */> onInit, Action<T, int /* index */> onRrefresh) {
        this.cow.TryBuild(proto, parent, targetCount, onInit).TryRefresh(targetCount, onRrefresh);
        return this;
    }

    public bool SetSelect(ref int currentIndex) {
        if (!(0 <= currentIndex && currentIndex < this.Count)) {
            if (this.Count <= 0) {
                currentIndex = -1;
                return false;
            }
            else {
                currentIndex = 0;
            }
        }

        //(cow[currentIndex] as UITabElement).SetSelected(true, true);
        return this;
    }
}

// 带有id管理的collector
[DisallowMultipleComponent]
public class COWComponentTabCollector<T_CP, T_Id> : MonoBehaviour where T_CP : Component {
    private readonly COWComponent<T_CP> cow = new COWComponent<T_CP>();
    private IList<T_Id> idList = new List<T_Id>();

    public int Count {
        get { return this.idList.Count; }
    }

    public T_CP this[T_Id id] {
        get {
            int index = this.idList.IndexOf(id);
            return this.cow[index];
        }
    }

    public void Clear() {
        this.cow.Clear();
    }

    public COWComponentTabCollector<T_CP, T_Id> TryBuildOrRefresh(GameObject proto, Transform parent, IList<T_Id> ids,
        Action<T_CP, T_Id, int /* index */> onInit, Action<T_CP, T_Id, int /* index */> onRrefresh) {
        this.idList = ids;
        Action<T_CP, int> initAction = (vd, index) => { onInit?.Invoke(vd, ids[index], index); };
        Action<T_CP, int> refreshAction = (vd, index) => { onRrefresh?.Invoke(vd, ids[index], index); };
        this.cow.TryBuild(proto, parent, ids.Count, initAction).TryRefresh(ids.Count, refreshAction);
        return this;
    }

    public bool SetSelect(ref T_Id currentId) {
        int index = this.idList.IndexOf(currentId);
        if (index == -1) {
            if (this.Count <= 0) {
                currentId = default;
                return false;
            }
            else {
                currentId = this.idList[0];
            }
        }

        // 设置选中
        //(this[currentId] as UITabElement).SetSelected(true, true);
        return true;
    }
}
