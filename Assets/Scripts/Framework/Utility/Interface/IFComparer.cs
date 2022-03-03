using System;
using System.Collections.Generic;

public class IFComparer<T> : IComparer<T> {
    public Func<T, T, int> OnDOCompare;

    public int Compare(T x, T y) {
        if (OnDOCompare == null) {
            return 0;
        }

        return OnDOCompare.Invoke(x, y);
    }
}

public class IFEqualityComparer<T> : IEqualityComparer<T> {
    public Func<T, T, bool> OnDoEquals;
    public Func<T, int> OnDoGetHashCode;

    public bool Equals(T x, T y) {
        if (OnDoEquals == null) {
            return false;
        }

        return OnDoEquals.Invoke(x, y);
    }

    public int GetHashCode(T target) {
        if (OnDoGetHashCode == null) {
            target.GetHashCode();
        }

        return OnDoGetHashCode.Invoke(target);
    }
}
