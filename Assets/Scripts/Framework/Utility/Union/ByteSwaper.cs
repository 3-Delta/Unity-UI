using System;

public class ByteSwaper {
    public static void Swap(ref byte left, ref byte right) {
        left = (byte)(left ^ right);
        right = (byte)(left ^ right);
        left = (byte)(left ^ right);
    }

    public static Int64 Swap(Int64 aim) {
        /*
        Int64 ret = 0;
        Int64 t = (byte)(aim >> 0);
        ret |= (Int64)(t << 56);
        t = (byte)(aim >> 8);
        ret |= (Int64)(t << 48);
        t = (byte)(aim >> 16);
        ret |= (Int64)(t << 40);
        t = (byte)(aim >> 24);
        ret |= (Int64)(t << 32);
        t = (byte)(aim >> 32);
        ret |= (Int64)(t << 24);
        t = (byte)(aim >> 40);
        ret |= (Int64)(t << 16);
        t = (byte)(aim >> 48);
        ret |= (Int64)(t << 8);
        t = (byte)(aim >> 56);
        ret |= (Int64)(t << 0);
        return ret;
        */

        Union64LocalSpliter u = new Union64LocalSpliter(aim);
        Swap(ref u.x0, ref u.x7);
        Swap(ref u.x1, ref u.x6);
        Swap(ref u.x2, ref u.x5);
        Swap(ref u.x3, ref u.x4);
        return u.value;
    }

    public static Int32 Swap(Int32 aim) {
        /*
        Int32 ret = 0;
        Int32 t = (byte)(aim >> 0);
        ret |= (Int32)(t << 24);
        t = (byte)(aim >> 8);
        ret |= (Int32)(t << 16);
        t = (byte)(aim >> 16);
        ret |= (Int32)(t << 8);
        t = (byte)(aim >> 24);
        ret |= (Int32)(t << 0);
        return ret;
        */

        Union32LocalSpliter u = new Union32LocalSpliter(aim);
        Swap(ref u.x0, ref u.x3);
        Swap(ref u.x1, ref u.x2);
        return u.value;
    }

    public static Int16 Swap(Int16 aim) {
        /*
        Int16 ret = 0;
        Int16 t = (byte)(aim >> 0);
        ret |= (Int16)(t << 8);
        t = (byte)(aim >> 8);
        ret |= (Int16)(t << 0);
        return ret;
        */

        Union16LocalSpliter u = new Union16LocalSpliter(aim);
        Swap(ref u.x0, ref u.x1);
        return u.value;
    }
}
