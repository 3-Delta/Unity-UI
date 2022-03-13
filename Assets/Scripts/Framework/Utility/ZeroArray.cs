using System;
using System.Collections.Generic;

// 来源优化
// https://edu.uwa4d.com/lesson-detail/165/844/0?isPreview=0
public static class ZeroArray<T> {
    public static readonly T[] Value = Array.Empty<T>();
}

public static class ZeroList<T> {
    public static readonly List<T> Value = new List<T>(0);
}

