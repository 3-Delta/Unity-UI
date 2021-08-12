using System.Collections.Generic;

/// <summary>
/// 被对象池管理的对象
/// </summary>
public class PoolObject {
    public bool setActive = true;

    /// <summary>
    /// 对象初始化时调用
    /// </summary>
    public virtual void OnCreate() {
    }

    /// <summary>
    /// 被销毁时调用
    /// </summary>
    public virtual void OnObjectDestroy() {
    }

    /// <summary>
    /// 从对象池中取出时调用
    /// </summary>
    public virtual void OnGet() {
    }

    /// <summary>
    /// 被回收时调用
    /// </summary>
    public virtual void OnPush() {
    }
}

public interface IPoolable<T> where T : IPoolable<T> //, PoolObject
{
    ObjectPool<T> ownerPool { get; set; }
}

public class ObjectPool<TValue> where TValue : IPoolable<TValue> {
    public readonly uint limit = 6;
    private Stack<TValue> pool = new Stack<TValue>();

    private System.Func<TValue> onCreate;
    private System.Action<TValue> onDestroy;
    private System.Action<TValue> onGet;
    private System.Action<TValue> onRelease;

    public ObjectPool(System.Func<TValue> onCreate = null, System.Action<TValue> onDestroy = null,
        System.Action<TValue> onGet = null, System.Action<TValue> onRelease = null) {
        this.onCreate = onCreate;
        this.onDestroy = onDestroy;
        this.onGet = onGet;
        this.onRelease = onRelease;
    }

    public ObjectPool(uint limit, System.Func<TValue> onCreate = null, System.Action<TValue> onDestroy = null,
        System.Action<TValue> onGet = null, System.Action<TValue> onRelease = null) {
        this.limit = limit;
        this.onCreate = onCreate;
        this.onDestroy = onDestroy;
        this.onGet = onGet;
        this.onRelease = onRelease;
    }

    public TValue Get() {
        TValue ret = default(TValue);
        if (pool.Count > 0) {
            ret = pool.Pop();
        }
        else {
            if (onCreate != null) {
                ret = onCreate.Invoke();
            }
        }

        // 设置属主
        if (ret != null) {
            onGet?.Invoke(ret);
            ret.ownerPool = this;
        }

        return ret;
    }

    public bool Release(TValue target) {
        bool ret = false;
        if (target != null) {
            if (pool.Count < limit) {
                ret = true;
                pool.Push(target);
            }
            else {
                onDestroy?.Invoke(target);
            }
        }

        // 取消属主
        if (target != null) {
            onRelease?.Invoke(target);
            target.ownerPool = null;
        }

        return ret;
    }

    public void Clear() {
        pool.Clear();
    }
}

// Map对象池
public class ObjectPool<TKey, TValue> where TValue : IPoolable<TValue> {
    private Dictionary<TKey, ObjectPool<TValue>> pool = new Dictionary<TKey, ObjectPool<TValue>>();
    private uint limit = 6;

    private System.Func<TValue> onCreate;
    private System.Action<TValue> onDestroy;
    private System.Action<TValue> onGet;
    private System.Action<TValue> onRelease;

    public ObjectPool(System.Func<TValue> onCreate = null, System.Action<TValue> onDestroy = null,
        System.Action<TValue> onGet = null, System.Action<TValue> onRelease = null) {
        this.onCreate = onCreate;
        this.onDestroy = onDestroy;
        this.onGet = onGet;
        this.onRelease = onRelease;
    }

    public ObjectPool(uint limit, System.Func<TValue> onCreate = null, System.Action<TValue> onDestroy = null,
        System.Action<TValue> onGet = null, System.Action<TValue> onRelease = null) {
        this.limit = limit;
        this.onCreate = onCreate;
        this.onDestroy = onDestroy;
        this.onGet = onGet;
        this.onRelease = onRelease;
    }

    public TValue Get(TKey key) {
        ObjectPool<TValue> innerPool = null;
        if (!pool.TryGetValue(key, out innerPool)) {
            innerPool = new ObjectPool<TValue>((uint) this.limit, this.onCreate, this.onDestroy, this.onGet,
                this.onRelease);
        }

        return innerPool.Get();
    }

    public bool Release(TKey key, TValue u) {
        ObjectPool<TValue> innerPool = null;
        if (!pool.TryGetValue(key, out innerPool)) {
            innerPool = new ObjectPool<TValue>((uint) this.limit, this.onCreate, this.onDestroy, this.onGet,
                this.onRelease);
            pool.Add(key, innerPool);
        }

        return innerPool.Release(u);
    }

    public void Clear() {
        foreach (var kvp in pool) {
            kvp.Value.Clear();
        }

        pool.Clear();
    }
}