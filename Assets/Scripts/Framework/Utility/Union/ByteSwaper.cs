using System;

public class ByteSwaper {
    public static Int64 Swap(Int64 aim) {
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
    }

    public static Int32 Swap(Int32 aim) {
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
    }

    public static Int16 Swap(Int16 aim) {
        Int16 ret = 0;
        Int16 t = (byte)(aim >> 0);
        ret |= (Int16)(t << 8);
        t = (byte)(aim >> 8);
        ret |= (Int16)(t << 0);
        return ret;
    }
}
