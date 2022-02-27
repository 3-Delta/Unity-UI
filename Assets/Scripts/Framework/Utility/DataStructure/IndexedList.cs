using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 为了避免IList的Contains的On消耗，从而使用HahsSet辅助，空间获取时间
/// </summary>
/// <typeparam name="T"></typeparam>
public class IndexedList<T> : IList<T> {
    //This is a container that gives:
    //  - Unique items
    //  - Fast random removal
    //  - Fast unique inclusion to the end
    //  - Sequential access
    //Downsides:
    //  - Uses more memory
    //  - Ordering is not persistent
    //  - Not Serialization Friendly.

    //We use a Dictionary to speed up list lookup, this makes it cheaper to guarantee no duplicates (set)
    //When removing we move the last item to the removed item position, this way we only need to update the index cache of a single item. (fast removal)
    //Order of the elements is not guaranteed. A removal will change the order of the items.

    private readonly List<T> list = new List<T>();
    private readonly Dictionary<T, int> dict = new Dictionary<T, int>();

    public void Add(T item) {
        list.Add(item);
        dict.Add(item, list.Count - 1);
    }

    public bool AddUnique(T item) {
        if (dict.ContainsKey(item)) {
            return false;
        }

        Add(item);
        return true;
    }

    public bool Remove(T item) {
        int index = -1;
        if (!dict.TryGetValue(item, out index)) {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    public IEnumerator<T> GetEnumerator() {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Clear() {
        list.Clear();
        dict.Clear();
    }

    public bool Contains(T item) {
        return dict.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        list.CopyTo(array, arrayIndex);
    }

    public int Count {
        get { return list.Count; }
    }

    public bool IsReadOnly {
        get { return false; }
    }

    public int IndexOf(T item) {
        int index = -1;
        dict.TryGetValue(item, out index);
        return index;
    }

    public void Insert(int index, T item) {
        //We could support this, but the semantics would be weird. Order is not guaranteed..
        throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
    }

    public void RemoveAt(int index) {
        T item = list[index];
        dict.Remove(item);
        if (index == list.Count - 1) {
            list.RemoveAt(index);
        }
        else {
            // swap indexItem with lastItem
            int lastIndex = list.Count - 1;
            T lastItem = list[lastIndex];
            list[index] = lastItem;
            dict[lastItem] = index;
            list.RemoveAt(lastIndex);
        }
    }

    public T this[int index] {
        get { return list[index]; }
        set {
            T item = list[index];
            dict.Remove(item);
            list[index] = value;
            dict.Add(item, index);
        }
    }

    public void RemoveAll(Predicate<T> match) {
        //I guess this could be optmized by instead of removing the items from the list immediatly,
        //We move them to the end, and then remove all in one go.
        //But I don't think this is going to be the bottleneck, so leaving as is for now.
        int i = 0;
        while (i < list.Count) {
            T item = list[i];
            if (match(item))
                Remove(item);
            else
                i++;
        }
    }

    //Sorts the internal list, this makes the exposed index accessor sorted as well.
    //But note that any insertion or deletion, can unorder the collection again.
    public void Sort(Comparison<T> sortLayoutFunction) {
        //There might be better ways to sort and keep the dictionary index up to date.
        list.Sort(sortLayoutFunction);
        //Rebuild the dictionary index.
        for (int i = 0; i < list.Count; ++i) {
            T item = list[i];
            dict[item] = i;
        }
    }
}
