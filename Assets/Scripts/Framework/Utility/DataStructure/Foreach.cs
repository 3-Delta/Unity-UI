using System;
using System.Collections.Generic;
using UnityEngine;

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

    // 原生Func不支持参数为out或者ref类型，所以需要拓展
    public delegate TResult RefFunc<TRef, out TResult>(ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TRef, out TResult>(TArg1 arg1, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TRef, out TResult>(TArg1 arg1, TArg2 arg2, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TArg3, TRef, out TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TArg3, TArg4, TRef, out TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TArg3, TArg4, TArg5, TRef, out TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRef, out TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, ref TRef refRlt);

    public delegate TResult RefFunc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRef, out TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, ref TRef refRlt);

    public static void Dictionary<TK, TV, TArg1, TRef>(IDictionary<TK, TV> dictionary, RefFunc<KeyValuePair<TK, TV>, int, TArg1, TRef, bool> func, TArg1 arg1, ref TRef rlt) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1, ref rlt);
            if (toBreak) {
                break;
            }
        }
    }

    public static void Dictionary<TK, TV, TArg1, TArg2, TRef>(IDictionary<TK, TV> dictionary, RefFunc<KeyValuePair<TK, TV>, int, TArg1, TArg2, TRef, bool> func, TArg1 arg1, TArg2 arg2, ref TRef rlt) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1, arg2, ref rlt);
            if (toBreak) {
                break;
            }
        }
    }

    public static void Dictionary<TK, TV, TArg1, TArg2, TArg3, TRef>(IDictionary<TK, TV> dictionary, RefFunc<KeyValuePair<TK, TV>, int, TArg1, TArg2, TArg3, TRef, bool> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, ref TRef rlt) {
        if (dictionary == null || func == null) {
            return;
        }

        int i = 0;
        foreach (var kvp in dictionary) {
            bool toBreak = func.Invoke(kvp, i++, arg1, arg2, arg3, ref rlt);
            if (toBreak) {
                break;
            }
        }
    }
}
