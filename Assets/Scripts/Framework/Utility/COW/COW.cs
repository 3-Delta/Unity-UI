using System;
using System.Collections.Generic;

// 数据层面的复用
[Serializable]
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
