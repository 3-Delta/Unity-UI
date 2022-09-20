using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUtils {
    // 是否为继承体系
    public static bool IsFrom<T1, T2>() {
        var tA = typeof(T1);
        var tB = typeof(T2);
        return tB.IsAssignableFrom(tA);
    }
}
