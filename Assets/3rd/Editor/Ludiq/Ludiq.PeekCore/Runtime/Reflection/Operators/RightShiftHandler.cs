using System;

namespace Ludiq.PeekCore
{
	public class RightShiftHandler : BinaryOperatorHandler
	{
		public RightShiftHandler() : base(OperatorCategory.Bitwise, BinaryOperator.RightShift, "Right Shift", "Right Shift", ">>", ">>", "op_RightShift")
		{
            Handle<byte, byte>((a, b) => a >> b, typeof(byte));
            Handle<byte, sbyte>((a, b) => a >> b, typeof(byte));
            Handle<byte, short>((a, b) => a >> b, typeof(byte));
            Handle<byte, ushort>((a, b) => a >> b, typeof(byte));
            Handle<byte, int>((a, b) => a >> b, typeof(byte));

            Handle<sbyte, byte>((a, b) => a >> b, typeof(sbyte));
            Handle<sbyte, sbyte>((a, b) => a >> b, typeof(sbyte));
            Handle<sbyte, short>((a, b) => a >> b, typeof(sbyte));
            Handle<sbyte, ushort>((a, b) => a >> b, typeof(sbyte));
            Handle<sbyte, int>((a, b) => a >> b, typeof(sbyte));

            Handle<short, byte>((a, b) => a >> b, typeof(short));
            Handle<short, sbyte>((a, b) => a >> b, typeof(short));
            Handle<short, short>((a, b) => a >> b, typeof(short));
            Handle<short, ushort>((a, b) => a >> b, typeof(short));
            Handle<short, int>((a, b) => a >> b, typeof(short));

            Handle<ushort, byte>((a, b) => a >> b, typeof(ushort));
            Handle<ushort, sbyte>((a, b) => a >> b, typeof(ushort));
            Handle<ushort, short>((a, b) => a >> b, typeof(ushort));
            Handle<ushort, ushort>((a, b) => a >> b, typeof(ushort));
            Handle<ushort, int>((a, b) => a >> b, typeof(ushort));

            Handle<int, byte>((a, b) => a >> b, typeof(int));
            Handle<int, sbyte>((a, b) => a >> b, typeof(int));
            Handle<int, short>((a, b) => a >> b, typeof(int));
            Handle<int, ushort>((a, b) => a >> b, typeof(int));
            Handle<int, int>((a, b) => a >> b, typeof(int));

            Handle<uint, byte>((a, b) => a >> b, typeof(uint));
            Handle<uint, sbyte>((a, b) => a >> b, typeof(uint));
            Handle<uint, short>((a, b) => a >> b, typeof(uint));
            Handle<uint, ushort>((a, b) => a >> b, typeof(uint));
            Handle<uint, int>((a, b) => a >> b, typeof(uint));

            Handle<long, byte>((a, b) => a >> b, typeof(long));
            Handle<long, sbyte>((a, b) => a >> b, typeof(long));
            Handle<long, short>((a, b) => a >> b, typeof(long));
            Handle<long, ushort>((a, b) => a >> b, typeof(long));
            Handle<long, int>((a, b) => a >> b, typeof(long));

            Handle<ulong, byte>((a, b) => a >> b, typeof(ulong));
            Handle<ulong, sbyte>((a, b) => a >> b, typeof(ulong));
            Handle<ulong, short>((a, b) => a >> b, typeof(ulong));
            Handle<ulong, ushort>((a, b) => a >> b, typeof(ulong));
            Handle<ulong, int>((a, b) => a >> b, typeof(ulong));
		}

		public override string GetDescriptionFormat(Type leftType, Type rightType) => "Takes {0}, and returns the first input shifted right by the second.";
	}
}