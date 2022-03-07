using System.Collections.Generic;
using System;

public class DelegateList
{
    // 为了保证顺序性，即先注册先执行，所以使用list,而不是map
    private List<Delegate> delegates = new List<Delegate>(0);

    public Delegate this[int index] => delegates[index];
    public int Count => delegates.Count;

    public DelegateList Add(Delegate callback)
    {
        if (callback != null)
        {
            delegates.Add(callback);
        }

        return this;
    }

    public DelegateList Remove(Delegate callback)
    {
        if (callback != null)
        {
            delegates.Remove(callback);
        }

        return this;
    }

    public DelegateList Clear()
    {
        delegates.Clear();
        return this;
    }

    public DelegateList Invoke()
    {
        for (int i = 0, length = Count; i < length; ++i)
        {
            // 强转是为了减少传递给params object[]的装箱消耗
            // delegates[i].DynamicInvoke(arg); 
            (delegates[i] as Action)?.Invoke();
        }

        return this;
    }

    public DelegateList Invoke<T>(T arg)
    {
        for (int i = 0, length = Count; i < length; ++i)
        {
            (delegates[i] as Action<T>)?.Invoke(arg);
        }

        return this;
    }

    public DelegateList Invoke<T1, T2>(T1 arg1, T2 arg2)
    {
        for (int i = 0, length = Count; i < length; ++i)
        {
            (delegates[i] as Action<T1, T2>)?.Invoke(arg1, arg2);
        }

        return this;
    }

    public DelegateList Invoke<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
    {
        for (int i = 0, length = Count; i < length; ++i)
        {
            (delegates[i] as Action<T1, T2, T3>)?.Invoke(arg1, arg2, arg3);
        }

        return this;
    }

    public DelegateList Invoke<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        for (int i = 0, length = Count; i < length; ++i)
        {
            (delegates[i] as Action<T1, T2, T3, T4>)?.Invoke(arg1, arg2, arg3, arg4);
        }

        return this;
    }
}

public static class DelegateMgr<T>
{
    private static readonly Dictionary<T, DelegateList> delegates =
        new Dictionary<T, DelegateList>(EqualityComparer<T>.Default /*防止T是enum等的GetHashCode的装箱*/);

    public static int Count => delegates.Count;

    private static void Add(T eventType, Delegate callback)
    {
        if (callback == null)
        {
            return;
        }

        if (!delegates.TryGetValue(eventType, out DelegateList delList))
        {
            delList = new DelegateList();
            delegates.Add(eventType, delList);
        }

        if (delList.Count > 0 && delList[0].GetType() != callback.GetType())
        {
            UnityEngine.Debug.LogErrorFormat("EventType: {0} has some different callback!", eventType.ToString());
        }
        else
        {
            delList.Add(callback);
        }
    }

    private static void Remove(T eventType, Delegate callback)
    {
        if (callback == null)
        {
            return;
        }

        if (delegates.TryGetValue(eventType, out DelegateList delList))
        {
            delList.Remove(callback);

            // 空则移除
            if (delList.Count <= 0)
            {
                delegates.Remove(eventType);
            }
        }
    }

    public static void Clear()
    {
        foreach (var kvp in delegates)
        {
            kvp.Value.Clear();
        }

        delegates.Clear();
    }

    // handle
    private static void _Handle(T eventType, Delegate handler, bool toBeAdd)
    {
        if (toBeAdd)
        {
            Add(eventType, handler);
        }
        else
        {
            Remove(eventType, handler);
        }
    }

    public static void Handle(T eventType, Action handler, bool toBeAdd)
    {
        _Handle(eventType, handler, toBeAdd);
    }

    public static void Handle<T1>(T eventType, Action<T1> handler, bool toBeAdd)
    {
        _Handle(eventType, handler, toBeAdd);
    }

    public static void Handle<T1, T2>(T eventType, Action<T1, T2> handler, bool toBeAdd)
    {
        _Handle(eventType, handler, toBeAdd);
    }

    public static void Handle<T1, T2, T3>(T eventType, Action<T1, T2, T3> handler, bool toBeAdd)
    {
        _Handle(eventType, handler, toBeAdd);
    }

    public static void Handle<T1, T2, T3, T4>(T eventType, Action<T1, T2, T3, T4> handler, bool toBeAdd)
    {
        _Handle(eventType, handler, toBeAdd);
    }

    private static bool PreFire(T eventType, out DelegateList list)
    {
        if (delegates == null)
        {
            list = null;
            return false;
        }

        return delegates.TryGetValue(eventType, out list);
    }

    public static void Fire(T eventType)
    {
        if (PreFire(eventType, out DelegateList list))
        {
            list?.Invoke();
        }
    }

    public static void Fire<T1>(T eventType, T1 arg)
    {
        if (PreFire(eventType, out DelegateList list))
        {
            list.Invoke<T1>(arg);
        }
    }

    public static void Fire<T1, T2>(T eventType, T1 arg1, T2 arg2)
    {
        if (PreFire(eventType, out DelegateList list))
        {
            list.Invoke<T1, T2>(arg1, arg2);
        }
    }

    public static void Fire<T1, T2, T3>(T eventType, T1 arg1, T2 arg2, T3 arg3)
    {
        if (PreFire(eventType, out DelegateList list))
        {
            list.Invoke<T1, T2, T3>(arg1, arg2, arg3);
        }
    }

    public static void Fire<T1, T2, T3, T4>(T eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (PreFire(eventType, out DelegateList list))
        {
            list.Invoke<T1, T2, T3, T4>(arg1, arg2, arg3, arg4);
        }
    }
}