using System;
using System.Collections.Generic;

// 枚举作为key的box 
public class Enum2IntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct {
    public bool Equals(TEnum x, TEnum y) {
        return EnumInt32ToInt.Convert(x) == EnumInt32ToInt.Convert(y);
    }

    public int GetHashCode(TEnum obj) {
        return EnumInt32ToInt.Convert(obj);
    }
}

public class DelegateList {
    // 为了保证顺序性，即先注册先执行，所以使用list,而不是map
    private List<Delegate> delegates = new List<Delegate>(0);

    public Delegate this[int index] => delegates[index];
    public int Count => delegates.Count;

    public DelegateList Add(Delegate callback) {
        if (callback != null) {
            delegates.Add(callback);
        }

        return this;
    }

    public DelegateList Remove(Delegate callback) {
        if (callback != null) {
            delegates.Remove(callback);
        }

        return this;
    }

    public DelegateList Clear() {
        delegates.Clear();
        return this;
    }

    public DelegateList Invoke<T>(T eventType) {
        for (int i = 0, length = Count; i < length; ++i) {
            // 强转是为了减少传递给params object[]的装箱消耗
            // delegates[i].DynamicInvoke(arg); 
            try {
                (delegates[i] as Action)?.Invoke();
            }
            catch (Exception e) {
                UnityEngine.Debug.LogErrorFormat("{0} eventType invoke fail : {1}", e.ToString());
            }
        }

        return this;
    }

    public DelegateList Invoke<T, T1>(T eventType, T1 arg) {
        for (int i = 0, length = Count; i < length; ++i) {
            (delegates[i] as Action<T1>)?.Invoke(arg);
        }

        return this;
    }

    public DelegateList Invoke<T, T1, T2>(T eventType, T1 arg1, T2 arg2) {
        for (int i = 0, length = Count; i < length; ++i) {
            (delegates[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        return this;
    }

    public DelegateList Invoke<T, T1, T2, T3>(T eventType, T1 arg1, T2 arg2, T3 arg3) {
        for (int i = 0, length = Count; i < length; ++i) {
            (delegates[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        return this;
    }

    public DelegateList Invoke<T, T1, T2, T3, T4>(T eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
        for (int i = 0, length = Count; i < length; ++i) {
            (delegates[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        return this;
    }
}

public class DelegateService<T> {
    public static readonly DelegateService<T> Default = new DelegateService<T>();
    private readonly Dictionary<T, DelegateList> delegates = new Dictionary<T, DelegateList>(EqualityComparer<T>.Default /*防止T是enum等的GetHashCode的装箱*/);

    public int Count => delegates.Count;

    private void Add(T eventType, Delegate callback) {
        if (callback == null) {
            return;
        }

        if (!delegates.TryGetValue(eventType, out DelegateList delList)) {
            delList = new DelegateList();
            delegates.Add(eventType, delList);
        }

        if (delList.Count > 0 && delList[0].GetType() != callback.GetType()) {
            UnityEngine.Debug.LogErrorFormat("EventType: {0} has some different callback!", eventType.ToString());
        }
        else {
            delList.Add(callback);
        }
    }

    private void Remove(T eventType, Delegate callback) {
        if (callback == null) {
            return;
        }

        if (delegates.TryGetValue(eventType, out DelegateList delList)) {
            delList.Remove(callback);

            // 空则移除
            if (delList.Count <= 0) {
                delegates.Remove(eventType);
            }
        }
    }

    public void Clear() {
        foreach (var kvp in delegates) {
            kvp.Value.Clear();
        }

        delegates.Clear();
    }

    // handle
    private void _Handle(T eventType, Delegate handler, bool toBeAdd) {
        if (toBeAdd) {
            Add(eventType, handler);
        }
        else {
            Remove(eventType, handler);
        }
    }

    public void Handle(T eventType, Action handler, bool toBeAdd) {
        _Handle(eventType, handler, toBeAdd);
    }

    public void Handle<T1>(T eventType, Action<T1> handler, bool toBeAdd) {
        _Handle(eventType, handler, toBeAdd);
    }

    public void Handle<T1, T2>(T eventType, Action<T1, T2> handler, bool toBeAdd) {
        _Handle(eventType, handler, toBeAdd);
    }

    public void Handle<T1, T2, T3>(T eventType, Action<T1, T2, T3> handler, bool toBeAdd) {
        _Handle(eventType, handler, toBeAdd);
    }

    public void Handle<T1, T2, T3, T4>(T eventType, Action<T1, T2, T3, T4> handler, bool toBeAdd) {
        _Handle(eventType, handler, toBeAdd);
    }

    private bool PreFire(T eventType, out DelegateList list) {
        if (delegates == null) {
            list = null;
            return false;
        }

        return delegates.TryGetValue(eventType, out list);
    }

    public void Fire(T eventType) {
        if (PreFire(eventType, out DelegateList list)) {
            list?.Invoke<T>(eventType);
        }
    }

    public void Fire<T1>(T eventType, T1 arg) {
        if (PreFire(eventType, out DelegateList list)) {
            list.Invoke<T, T1>(eventType, arg);
        }
    }

    public void Fire<T1, T2>(T eventType, T1 arg1, T2 arg2) {
        if (PreFire(eventType, out DelegateList list)) {
            list.Invoke<T, T1, T2>(eventType, arg1, arg2);
        }
    }

    public void Fire<T1, T2, T3>(T eventType, T1 arg1, T2 arg2, T3 arg3) {
        if (PreFire(eventType, out DelegateList list)) {
            list.Invoke<T, T1, T2, T3>(eventType, arg1, arg2, arg3);
        }
    }

    public void Fire<T1, T2, T3, T4>(T eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
        if (PreFire(eventType, out DelegateList list)) {
            list.Invoke<T, T1, T2, T3, T4>(eventType, arg1, arg2, arg3, arg4);
        }
    }
}
