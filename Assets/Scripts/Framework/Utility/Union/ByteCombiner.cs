using System;

public class ByteCombiner {
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
