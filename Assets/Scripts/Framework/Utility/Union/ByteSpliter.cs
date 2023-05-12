using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ByteSpliter {
    public static void Split(Int64 aim, byte[] array, bool littleEndian /*BitConverter.IsLittleEndian*/) {
        if (array == null || array.Length < sizeof(Int64)) {
            return;
        }
        
        Union64LocalSpliter u = new Union64LocalSpliter(aim);
        if (littleEndian) {
            // 内存低地址存储数值低位，高地址存储数值高位
            array[0] = u.x7;
            array[1] = u.x6;
            array[2] = u.x5;
            array[3] = u.x4;
            array[4] = u.x3;
            array[5] = u.x2;
            array[6] = u.x1;
            array[7] = u.x0;
        }
        else {
            array[0] = u.x0;
            array[1] = u.x1;
            array[2] = u.x2;
            array[3] = u.x3;
            array[4] = u.x4;
            array[5] = u.x5;
            array[6] = u.x6;
            array[7] = u.x7;
        }
    }

    public static void Split(Int32 aim, byte[] array, bool littleEndian) {
        if (array == null || array.Length < sizeof(Int32)) {
            return;
        }

        Union32LocalSpliter u = new Union32LocalSpliter(aim);
        if (littleEndian) {
            array[0] = u.x3;
            array[1] = u.x2;
            array[2] = u.x1;
            array[3] = u.x0;
        }
        else {
            array[0] = u.x0;
            array[1] = u.x1;
            array[2] = u.x2;
            array[3] = u.x3;
        }
    }

    public static void Split(Int16 aim, byte[] array, bool littleEndian) {
        if (array == null || array.Length < sizeof(Int16)) {
            return;
        }

        Union16LocalSpliter u = new Union16LocalSpliter(aim);
        if (littleEndian) {
            array[0] = u.x1;
            array[1] = u.x0;
        }
        else {
            array[0] = u.x0;
            array[1] = u.x1;
        }
    }
}
