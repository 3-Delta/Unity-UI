using System;
using System.Runtime.InteropServices;

// 解析成和本机一样的大小端数据
// https://www.zhihu.com/pin/1633497757422940160
[StructLayout(LayoutKind.Explicit)]
public struct Union64LocalSpliter {
    [FieldOffset(0)] public readonly Int64 value;
    [FieldOffset(0)] public readonly byte v0;
    [FieldOffset(1)] public readonly byte v1;
    [FieldOffset(2)] public readonly byte v2;
    [FieldOffset(3)] public readonly byte v3;
    [FieldOffset(4)] public readonly byte v4;
    [FieldOffset(5)] public readonly byte v5;
    [FieldOffset(6)] public readonly byte v6;
    [FieldOffset(7)] public readonly byte v7;

    public Union64LocalSpliter(Int64 value) {
        this.v0 = this.v1 = this.v2 = this.v3 = this.v4 = this.v5 = this.v6 = this.v7 = 0;
        this.value = value;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union32LocalSpliter {
    [FieldOffset(0)] public readonly Int32 value;
    [FieldOffset(0)] public readonly byte v0;
    [FieldOffset(1)] public readonly byte v1;
    [FieldOffset(2)] public readonly byte v2;
    [FieldOffset(3)] public readonly byte v3;

    public Union32LocalSpliter(Int32 value) {
        this.v0 = this.v1 = this.v2 = this.v3 = 0;
        this.value = value;
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union16LocalSpliter {
    [FieldOffset(0)] public readonly Int16 value;
    [FieldOffset(0)] public readonly byte v0;
    [FieldOffset(1)] public readonly byte v1;

    public Union16LocalSpliter(Int16 value) {
        this.v0 = this.v1 = 0;
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

    public static bool Combine(byte[] array, out Int64 aim, bool littleEndian) {
        aim = 0;
        if (array == null || array.Length < sizeof(Int64)) {
            return false;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            aim = array[0];
            aim = (Int64)(array[1] << 8) | aim;
            aim = (Int64)(array[2] << 16) | aim;
            aim = (Int64)(array[3] << 24) | aim;
            aim = (Int64)((Int64)array[4] << 32) | aim;
            aim = (Int64)((Int64)array[5] << 40) | aim;
            aim = (Int64)((Int64)array[6] << 48) | aim;
            aim = (Int64)((Int64)array[7] << 56) | aim;
        }
        else {
            aim = array[7];
            aim = (Int64)(array[6] << 8) | aim;
            aim = (Int64)(array[5] << 16) | aim;
            aim = (Int64)(array[4] << 24) | aim;
            aim = (Int64)((Int64)array[3] << 32) | aim;
            aim = (Int64)((Int64)array[2] << 40) | aim;
            aim = (Int64)((Int64)array[1] << 48) | aim;
            aim = (Int64)((Int64)array[0] << 56) | aim;
        }

        return true;
    }

    public static bool Combine(byte[] array, out Int32 aim, bool littleEndian) {
        aim = 0;
        if (array == null || array.Length < sizeof(Int32)) {
            return false;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            aim = array[0];
            aim = (array[1] << 8) | aim;
            aim = (array[2] << 16) | aim;
            aim = (array[3] << 24) | aim;
        }
        else {
            aim = array[3];
            aim = (array[2] << 8) | aim;
            aim = (array[1] << 16) | aim;
            aim = (array[0] << 24) | aim;
        }

        return true;
    }

    public static bool Combine(byte[] array, out Int16 aim, bool littleEndian) {
        aim = 0;
        if (array == null || array.Length < sizeof(Int16)) {
            return false;
        }

        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            Int32 u32 = array[0];
            u32 = (array[1] << 8) | u32;

            aim = (Int16)u32;
        }
        else {
            Int32 u32 = array[1];
            u32 = (array[0] << 8) | u32;

            aim = (Int16)u32;
        }

        return true;
    }
}
