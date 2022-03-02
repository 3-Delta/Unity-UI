using System;
using System.Collections.Generic;

public class Foreach {
    // 优化ilr字典的遍历
    public static void Dictionary<TK, TV>(IDictionary<TK, TV> dictionary, Func<KeyValuePair<TK, TV>, int, bool> func) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++);
            if (toBreak) {
                break;
            }
        }
    }

    public static void Dictionary<TK, TV, TArg1>(IDictionary<TK, TV> dictionary, Func<KeyValuePair<TK, TV>, int, TArg1, bool> func, TArg1 arg1) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1);
            if (toBreak) {
                break;
            }
        }
    }

    public static void Dictionary<TK, TV, TArg1, Targ2>(IDictionary<TK, TV> dictionary, Func<KeyValuePair<TK, TV>, int, TArg1, Targ2, bool> func, TArg1 arg1, Targ2 arg2) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1, arg2);
            if (toBreak) {
                break;
            }
        }
    }

    public static void Dictionary<TK, TV, TArg1, Targ2, Targ3>(IDictionary<TK, TV> dictionary, Func<KeyValuePair<TK, TV>, int, TArg1, Targ2, Targ3, bool> func, TArg1 arg1, Targ2 arg2, Targ3 arg3) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1, arg2, arg3);
            if (toBreak) {
                break;
            }
        }
    }
}
