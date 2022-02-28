using System;
using System.Collections;
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
}
