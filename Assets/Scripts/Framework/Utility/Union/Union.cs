using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// 解析成和本机一样的大小端数据
// https://www.zhihu.com/pin/1633497757422940160
[StructLayout(LayoutKind.Explicit)]
public struct Union64LocalSpliter {
    [FieldOffset(0)] public Int64 value;

    [FieldOffset(0)] public byte x0;
    [FieldOffset(1)] public byte x1;
    [FieldOffset(2)] public byte x2;
    [FieldOffset(3)] public byte x3;
    [FieldOffset(4)] public byte x4;
    [FieldOffset(5)] public byte x5;
    [FieldOffset(6)] public byte x6;
    [FieldOffset(7)] public byte x7;
    
    [FieldOffset(0)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = sizeof(Int64))] public byte[] bytes;
    
    public Union64LocalSpliter(Int64 value) {
        this.x0 = this.x1 = this.x2 = this.x3 = this.x4 = this.x5 = this.x6 = this.x7 = 0;
        this.bytes = new byte[sizeof(Int64)] {0, 0, 0, 0, 0, 0, 0, 0};
        this.value = value;
    }

    public void CopyFrom(IList<byte> from, int beginIndex, int endIndex) {
        if (from == null || beginIndex > endIndex || beginIndex < 0) {
            value = 0;
            return;
        }

        int i = 0;
        while (i < sizeof(Int64) && beginIndex < endIndex) {
            bytes[i++] = from[beginIndex++];
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union32LocalSpliter {
    [FieldOffset(0)] public Int32 value;

    [FieldOffset(0)] public byte x0;
    [FieldOffset(1)] public byte x1;
    [FieldOffset(2)] public byte x2;
    [FieldOffset(3)] public byte x3;
    
    [FieldOffset(0)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = sizeof(Int32))] public byte[] bytes;

    public Union32LocalSpliter(Int32 value) {
        this.x0 = this.x1 = this.x2 = this.x3 = 0;
        this.bytes = new byte[sizeof(Int32)] {0, 0, 0, 0};
        this.value = value;
    }
    
    public void CopyFrom(IList<byte> from, int beginIndex, int endIndex) {
        if (from == null || beginIndex > endIndex || beginIndex < 0) {
            value = 0;
            return;
        }

        int i = 0;
        while (i < sizeof(Int32) && beginIndex < endIndex) {
            bytes[i++] = from[beginIndex++];
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union16LocalSpliter {
    [FieldOffset(0)] public Int16 value;

    [FieldOffset(0)] public byte x0;
    [FieldOffset(1)] public byte x1;
    
    [FieldOffset(0)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = sizeof(Int16))] public byte[] bytes;

    public Union16LocalSpliter(Int16 value) {
        this.x0 = this.x1 = 0;
        this.bytes = new byte[sizeof(Int16)] {0, 0};
        this.value = value;
    }
    
    public void CopyFrom(IList<byte> from, int beginIndex, int endIndex) {
        if (from == null || beginIndex > endIndex || beginIndex < 0) {
            value = 0;
            return;
        }

        int i = 0;
        while (i < sizeof(Int16) && beginIndex < endIndex) {
            bytes[i++] = from[beginIndex++];
        }
    }

    // 大小端判断
    public static bool IslittleEndian() {
        return new Union16LocalSpliter(0x0102).x0 == 0x02;
    }
}
