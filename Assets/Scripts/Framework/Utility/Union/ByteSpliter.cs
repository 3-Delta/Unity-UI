using System;
using System.Runtime.InteropServices;

// 解析成和本机一样的大小端数据
[StructLayout(LayoutKind.Explicit)]
public struct Union64LocalSpliter {
    [FieldOffset(0)] public readonly Int64 value;
    [FieldOffset(0)] public readonly byte[] bytes;

    public Union64LocalSpliter(Int64 value) {
        this.value = value;
        this.bytes = new byte[8];
    }

    public Union64LocalSpliter(byte[] bytes) {
        this.value = 0;
        if (bytes != null && bytes.Length >= sizeof(Int64)) {
            this.bytes = bytes;
        }
        else {
            this.bytes = new byte[8];
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union32LocalSpliter {
    [FieldOffset(0)] public readonly Int32 value;
    [FieldOffset(0)] public readonly byte[] bytes;

    public Union32LocalSpliter(Int32 value) {
        this.value = value;
        this.bytes = new byte[4];
    }

    public Union32LocalSpliter(byte[] bytes) {
        this.value = 0;
        if (bytes != null && bytes.Length >= sizeof(Int32)) {
            this.bytes = bytes;
        }
        else {
            this.bytes = new byte[4];
        }
    }
}

[StructLayout(LayoutKind.Explicit)]
public struct Union16LocalSpliter {
    [FieldOffset(0)] public readonly Int16 value;
    [FieldOffset(0)] public readonly byte[] bytes;

    public Union16LocalSpliter(Int16 value) {
        this.value = value;
        this.bytes = new byte[2];
    }

    public Union16LocalSpliter(byte[] bytes) {
        this.value = 0;
        if (bytes != null && bytes.Length >= sizeof(Int16)) {
            this.bytes = bytes;
        }
        else {
            this.bytes = new byte[2];
        }
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
            aim = (uint)(array[1] << 8) | aim;
            aim = (uint)(array[2] << 8) | aim;
            aim = (uint)(array[3] << 8) | aim;
            aim = (uint)(array[4] << 8) | aim;
            aim = (uint)(array[5] << 8) | aim;
            aim = (uint)(array[6] << 8) | aim;
            aim = (uint)(array[7] << 8) | aim;
        }
        else {
            aim = array[7];
            aim = (uint)(array[6] << 8) | aim;
            aim = (uint)(array[5] << 8) | aim;
            aim = (uint)(array[4] << 8) | aim;
            aim = (uint)(array[3] << 8) | aim;
            aim = (uint)(array[2] << 8) | aim;
            aim = (uint)(array[1] << 8) | aim;
            aim = (uint)(array[0] << 8) | aim;
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
            aim = (array[2] << 8) | aim;
            aim = (array[3] << 8) | aim;
        }
        else {
            aim = array[3];
            aim = (array[2] << 8) | aim;
            aim = (array[1] << 8) | aim;
            aim = (array[0] << 8) | aim;
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
