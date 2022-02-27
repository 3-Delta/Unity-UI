using System;
using System.Runtime.InteropServices;
using UnityEngine;

// https://zhuanlan.zhihu.com/p/91911998
// 但是很可惜：Could not load type 'TypeUnionConverter`2' from assembly 'xxx' because generic types cannot have explicit layout.
// 需要考虑大小端的问题
public static class StructTypeConverter {
    [StructLayout(LayoutKind.Explicit)]
    public struct UnionConverter<T, R> where T : struct where R : struct {
        [FieldOffset(0)] public T known;

        [FieldOffset(0)] public R result;
    }

    public static R Convert<T, R>(T value) where T : struct where R : struct {
        return new UnionConverter<T, R> { known = value }.result;
    }

    public static R Convert<T, R>(Func<T> action) where T : struct where R : struct {
        return new UnionConverter<T, R> { known = action() }.result;
    }

    // 指针
    public static unsafe R ConvertByPointer<T, R>(T value) where T : struct where R : struct {
        //return *(R*)&value;
        return default;
    }

    public static bool IsLittleEndian {
        get { return BitConverter.IsLittleEndian; }
    }

    public static unsafe void Test() {
        UnityEngine.Debug.LogError("IsLittleEndian: " + IsLittleEndian);

        int v = 0x01020304;
        UnityEngine.Color32 color = *(Color32*)&(v);
        Debug.LogError("color: " + color);
    }

    /*
    #region converter  // 利用union进行数据转换，相比较BitConverter.GetBytes会少一部分gc的存在
    [StructLayout(LayoutKind.Explicit)]
    public struct Int32Converter
    {
        [FieldOffset(0)]
        public int known;

        [FieldOffset(0)]
        public byte[] result;

        public Int32Converter(int value)
        {
            known = value;
            result = new byte[0];
        }
        //public Int32Converter(byte[] value, int startIndex)
        //{
        //    result = new byte[sizeof(int)];
        //}
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UInt32Converter
    {
        [FieldOffset(0)]
        public uint known;

        [FieldOffset(0)]
        public byte[] result;

        public UInt32Converter(uint value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Int16Converter
    {
        [FieldOffset(0)]
        public short known;

        [FieldOffset(0)]
        public byte[] result;

        public Int16Converter(short value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UInt16Converter
    {
        [FieldOffset(0)]
        public ushort known;

        [FieldOffset(0)]
        public byte[] result;

        public UInt16Converter(ushort value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Int64Converter
    {
        [FieldOffset(0)]
        public long known;

        [FieldOffset(0)]
        public byte[] result;

        public Int64Converter(long value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UInt64Converter
    {
        [FieldOffset(0)]
        public ulong known;

        [FieldOffset(0)]
        public byte[] result;

        public UInt64Converter(ulong value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ByteConverter
    {
        [FieldOffset(0)]
        public byte known;

        [FieldOffset(0)]
        public byte[] result;

        public ByteConverter(byte value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SByteConverter
    {
        [FieldOffset(0)]
        public sbyte known;

        [FieldOffset(0)]
        public byte[] result;

        public SByteConverter(sbyte value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharConverter
    {
        [FieldOffset(0)]
        public char known;

        [FieldOffset(0)]
        public byte[] result;

        public CharConverter(char value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct BoolConverter
    {
        [FieldOffset(0)]
        public bool known;

        [FieldOffset(0)]
        public byte[] result;

        public BoolConverter(bool value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct FloatConverter
    {
        [FieldOffset(0)]
        public float known;

        [FieldOffset(0)]
        public byte[] result;

        public FloatConverter(float value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct DoubleConverter
    {
        [FieldOffset(0)]
        public double known;

        [FieldOffset(0)]
        public byte[] result;

        public DoubleConverter(double value)
        {
            known = value;
            result = new byte[0];
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct StringConverter
    {
        [FieldOffset(0)]
        public string known;

        [FieldOffset(0)]
        public byte[] result;

        public StringConverter(string value)
        {
            known = value;
            result = new byte[0];
        }
    }

    #endregion
    */
}
