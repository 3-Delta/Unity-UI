using System;
using System.Collections.Generic;

public class BoolUtils {
    public static bool AllTrue<T>(IList<T> container, Func<int, T, bool> condition, out int failIndex) {
        bool ret = true;
        failIndex = -1;
        for (int i = 0, length = container.Count; i < length; ++i) {
            ret &= (condition.Invoke(i, container[i]));
            if (!ret) {
                failIndex = i;
                break;
            }
        }

        return ret;
    }

    public static bool AllTrue<T>(ICollection<T> container, Func<T, bool> condition) {
        bool ret = true;
        foreach (var one in container) {
            ret &= (condition.Invoke(one));
            if (!ret) {
                break;
            }
        }

        return ret;
    }

    public static bool AllFalse<T>(IList<T> container, Func<int, T, bool> condition, out int failIndex) {
        bool ret = false;
        failIndex = -1;
        for (int i = 0, length = container.Count; i < length; ++i) {
            ret |= (condition.Invoke(i, container[i]));
            if (ret) {
                failIndex = i;
                break;
            }
        }

        return !ret;
    }

    public static bool AllFalse<T>(ICollection<T> container, Func<T, bool> condition) {
        bool ret = false;
        foreach (var one in container) {
            ret |= (condition.Invoke(one));
            if (ret) {
                break;
            }
        }

        return !ret;
    }

    public static bool AnyTrue<T>(IList<T> container, Func<int, T, bool> condition, out int firstIndex) {
        return !AllFalse(container, condition, out firstIndex);
    }

    public static bool AnyTrue<T>(ICollection<T> container, Func<T, bool> condition) {
        return !AllFalse(container, condition);
    }

    public static bool AnyFalse<T>(IList<T> container, Func<int, T, bool> condition, out int firstIndex) {
        return !AllTrue(container, condition, out firstIndex);
    }

    public static bool AnyFalse<T>(ICollection<T> container, Func<T, bool> condition) {
        return !AllTrue(container, condition);
    }
}
