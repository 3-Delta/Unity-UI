using System;
using System.Collections.Generic;

// 用于sortedKey之类的进行value排序
public class TComparer<TKey> : IComparer<TKey> {
    protected Func<TKey, TKey, int> onCompare;

    public TComparer(Func<TKey, TKey, int> onCompare) {
        this.onCompare = onCompare;
    }

    public int Compare(TKey x, TKey y) {
        if (onCompare == null) {
            return 0;
        }

        return onCompare.Invoke(x, y);
    }
}
