using System;
using System.Collections.Generic;

public class EventDictionary<TKey, TValue> {
    protected readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

    public Action<TKey, TValue> onAdd;
    public Action<TKey> onReplace;
    public Action<TKey> onRemove;

    public bool TryGet(TKey key, out TValue actor) {
        bool ret = this.dict.TryGetValue(key, out actor);
        return ret;
    }

    public bool Add(TKey key, TValue actor) {
        bool exist = this.TryGet(key, out actor);
        if (!exist) {
            this.dict.Add(key, actor);
            onAdd?.Invoke(key, actor);
        }

        return exist;
    }

    public void Replace(TKey key, TValue actor) {
        this.dict[key] = actor;
        onReplace?.Invoke(key);
    }

    public bool Remove(TKey key) {
        bool ret = this.dict.Remove(key);
        if (ret) {
            onRemove?.Invoke(key);
        }

        return ret;
    }

    public void Clear() {
        dict.Clear();
    }
}