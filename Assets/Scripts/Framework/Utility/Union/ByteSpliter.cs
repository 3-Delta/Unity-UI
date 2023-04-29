using System;
using System.Runtime.InteropServices;

// 解析成和本机一样的大小端数据
// https://www.zhihu.com/pin/1633497757422940160
[StructLayout(LayoutKind.Explicit)]
public struct Union64LocalSpliter {
    [FieldOffset(0)] public readonly Int64 value;
    
    [FieldOffset(0)] public readonly byte x0;
    [FieldOffset(1)] public readonly byte x1;
    [FieldOffset(2)] public readonly byte x2;
    [FieldOffset(3)] public readonly byte x3;
    [FieldOffset(4)] public readonly byte x4;
    [FieldOffset(5)] public readonly byte x5;
    [FieldOffset(6)] public readonly byte x6;
    [FieldOffset(7)] public readonly byte x7;

    [FieldOffset(0)] public readonly byte y7;
    [FieldOffset(1)] public readonly byte y6;
    [FieldOffset(2)] public readonly byte y5;
    [FieldOffset(3)] public readonly byte y4;
    [FieldOffset(4)] public readonly byte y3;
    [FieldOffset(5)] public readonly byte y2;
    [FieldOffset(6)] public readonly byte y1;
    [FieldOffset(7)] public readonly byte y0;

    public Union64LocalSpliter(Int32 value) {
        this.x0 = this.x1 = this.x2 = this.x3 = this.x4 = this.x5 = this.x6 = this.x7 = this.y0 = this.y1 = this.y2 = this.y3 = this.y4 = this.y5 = this.y6 = this.y7 = 0;
        this.value = value;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union32LocalSpliter {
    [FieldOffset(0)] public readonly Int32 value;

    [FieldOffset(0)] public readonly byte x0;
    [FieldOffset(1)] public readonly byte x1;
    [FieldOffset(2)] public readonly byte x2;
    [FieldOffset(3)] public readonly byte x3;

    [FieldOffset(0)] public readonly byte y3;
    [FieldOffset(1)] public readonly byte y2;
    [FieldOffset(2)] public readonly byte y1;
    [FieldOffset(3)] public readonly byte y0;

    public Union32LocalSpliter(Int32 value) {
        this.x0 = this.x1 = this.x2 = this.x3 = this.y0 = this.y1 = this.y2 = this.y3 = 0;
        this.value = value;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union16LocalSpliter {
    [FieldOffset(0)] public readonly Int16 value;

    [FieldOffset(0)] public readonly byte x0;
    [FieldOffset(1)] public readonly byte x1;

    [FieldOffset(0)] public readonly byte y1;
    [FieldOffset(1)] public readonly byte y0;

    public Union16LocalSpliter(Int16 value) {
        this.x0 = this.x1 = this.y0 = this.y1 = 0;
        this.value = value;
    }
}

public class ByteSpliter {
    public static void Split(Int64 aim, byte[] array, bool littleEndian /*BitConverter.IsLittleEndian*/) {
        if (array == null || array.Length < sizeof(Int64)) {
            return;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            byte t = 0;
            t = (byte)aim;
            array[0] = t;
            t = (byte)(aim >> 8);
            array[1] = t;
            t = (byte)(aim >> 16);
            array[2] = t;
            t = (byte)(aim >> 24);
            array[3] = t;
            t = (byte)(aim >> 32);
            array[4] = t;
            t = (byte)(aim >> 40);
            array[5] = t;
            t = (byte)(aim >> 48);
            array[6] = t;
            t = (byte)(aim >> 56);
            array[7] = t;
        }
        else {
            byte t = 0;
            t = (byte)aim;
            array[7] = t;
            t = (byte)(aim >> 8);
            array[6] = t;
            t = (byte)(aim >> 16);
            array[5] = t;
            t = (byte)(aim >> 24);
            array[4] = t;
            t = (byte)(aim >> 32);
            array[3] = t;
            t = (byte)(aim >> 40);
            array[2] = t;
            t = (byte)(aim >> 48);
            array[1] = t;
            t = (byte)(aim >> 56);
            array[0] = t;
        }
    }

    public static void Split(Int32 aim, byte[] array, bool littleEndian) {
        if (array == null || array.Length < sizeof(Int32)) {
            return;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            byte t = 0;
            t = (byte)aim;
            array[0] = t;
            t = (byte)(aim >> 8);
            array[1] = t;
            t = (byte)(aim >> 16);
            array[2] = t;
            t = (byte)(aim >> 24);
            array[3] = t;
        }
        else {
            byte t = 0;
            t = (byte)aim;
            array[3] = t;
            t = (byte)(aim >> 8);
            array[2] = t;
            t = (byte)(aim >> 16);
            array[1] = t;
            t = (byte)(aim >> 24);
            array[0] = t;
        }
    }

    public static void Split(Int16 aim, byte[] array, bool littleEndian) {
        if (array == null || array.Length < sizeof(Int16)) {
            return;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            byte t = 0;
            t = (byte)aim;
            array[0] = t;
            t = (byte)(aim >> 8);
            array[1] = t;
        }
        else {
            byte t = 0;
            t = (byte)(aim);
            array[1] = t;
            t = (byte)(aim >> 8);
            array[0] = t;
        }
    }
}
