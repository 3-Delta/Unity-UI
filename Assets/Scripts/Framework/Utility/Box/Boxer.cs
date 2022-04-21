using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// https://zhuanlan.zhihu.com/p/476725200
// 以及clr via c# page.111
public class ClassHeader {
    public byte value = 0;

    public static int HeaderSize {
        get { return UnsafeUtility.GetFieldOffset(typeof(ClassHeader).GetField(nameof(value))); }
    }
}

// int等内置值类型也可以通过这个struct的限制
public static class Boxer<T> where T : struct {
    // 提前box创建一个模板
    private static readonly object _BOXER = Activator.CreateInstance<T>();

    public unsafe static object Box(ref T target) {
        byte* ptr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(_BOXER, out ulong gcHandler);
        byte* valuePtr = ptr + ClassHeader.HeaderSize;
        // 高C#版本可以直接：ref T t = UnsafeUtility.AsRef<T>((void*)ptr);
        UnsafeUtility.CopyStructureToPtr(ref target, valuePtr);
        UnsafeUtility.ReleaseGCObject(gcHandler);
        return _BOXER;
    }
}
