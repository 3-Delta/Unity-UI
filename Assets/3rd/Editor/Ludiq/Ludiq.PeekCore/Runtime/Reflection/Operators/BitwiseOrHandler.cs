#pragma warning disable 675

using System;

namespace Ludiq.PeekCore
{
	public class BitwiseOrHandler : BinaryOperatorHandler
	{
		public BitwiseOrHandler() : base(OperatorCategory.Bitwise, BinaryOperator.Or, "Bitwise Or", "Bitwise Or", "|", "|", "op_BitwiseOr")
		{
			Handle<bool, bool>((a, b) => a | b, typeof(bool));

			Handle<byte, byte>((a, b) => a | b, typeof(byte));
			Handle<byte, sbyte>((a, b) => a | b, typeof(int));
			Handle<byte, short>((a, b) => a | b, typeof(int));
			Handle<byte, ushort>((a, b) => a | b, typeof(uint));
			Handle<byte, int>((a, b) => a | b, typeof(int));
			Handle<byte, uint>((a, b) => a | b, typeof(uint));
			Handle<byte, long>((a, b) => a | b, typeof(long));
			Handle<byte, ulong>((a, b) => a | b, typeof(ulong));

			Handle<sbyte, byte>((a, b) => a | b, typeof(int));
			Handle<sbyte, sbyte>((a, b) => a | b, typeof(sbyte));
			Handle<sbyte, short>((a, b) => a | b, typeof(int));
			Handle<sbyte, ushort>((a, b) => a | b, typeof(int));
			Handle<sbyte, int>((a, b) => a | b, typeof(int));
			Handle<sbyte, uint>((a, b) => a | b, typeof(long));
			Handle<sbyte, long>((a, b) => a | b, typeof(long));
			//Handle<sbyte, ulong>((a, b) => a | b, typeof(long));

			Handle<short, byte>((a, b) => a | b, typeof(int));
			Handle<short, sbyte>((a, b) => a | b, typeof(int));
			Handle<short, short>((a, b) => a | b, typeof(short));
			Handle<short, ushort>((a, b) => a | b, typeof(ushort));
			Handle<short, int>((a, b) => a | b, typeof(int));
			Handle<short, uint>((a, b) => a | b, typeof(long));
			Handle<short, long>((a, b) => a | b, typeof(long));
			//Handle<short, ulong>((a, b) => a | b, typeof(long));

			Handle<ushort, byte>((a, b) => a | b, typeof(uint));
			Handle<ushort, sbyte>((a, b) => a | b, typeof(int));
			Handle<ushort, short>((a, b) => a | b, typeof(int));
			Handle<ushort, ushort>((a, b) => a | b, typeof(ushort));
			Handle<ushort, int>((a, b) => a | b, typeof(int));
			Handle<ushort, uint>((a, b) => a | b, typeof(uint));
			Handle<ushort, long>((a, b) => a | b, typeof(ulong));
			Handle<ushort, ulong>((a, b) => a | b, typeof(ulong));

			Handle<int, byte>((a, b) => a | b, typeof(int));
			Handle<int, sbyte>((a, b) => a | b, typeof(int));
			Handle<int, short>((a, b) => a | b, typeof(int));
			Handle<int, ushort>((a, b) => a | b, typeof(int));
			Handle<int, int>((a, b) => a | b, typeof(int));
			Handle<int, uint>((a, b) => a | b, typeof(long));
			Handle<int, long>((a, b) => a | b, typeof(long));
			//Handle<int, ulong>((a, b) => a | b, typeof(long));

			Handle<uint, byte>((a, b) => a | b, typeof(uint));
			Handle<uint, sbyte>((a, b) => a | b, typeof(int));
			Handle<uint, short>((a, b) => a | b, typeof(int));
			Handle<uint, ushort>((a, b) => a | b, typeof(uint));
			Handle<uint, int>((a, b) => a | b, typeof(long));
			Handle<uint, uint>((a, b) => a | b, typeof(uint));
			Handle<uint, long>((a, b) => a | b, typeof(long));
			Handle<uint, ulong>((a, b) => a | b, typeof(ulong));

			Handle<long, byte>((a, b) => a | b, typeof(long));
			Handle<long, sbyte>((a, b) => a | b, typeof(long));
			Handle<long, short>((a, b) => a | b, typeof(long));
			Handle<long, ushort>((a, b) => a | b, typeof(long));
			Handle<long, int>((a, b) => a | b, typeof(long));
			Handle<long, uint>((a, b) => a | b, typeof(long));
			Handle<long, long>((a, b) => a | b, typeof(long));
			//Handle<long, ulong>((a, b) => a | b, typeof(long));

			Handle<ulong, byte>((a, b) => a | b, typeof(ulong));
			//Handle<ulong, sbyte>((a, b) => a | b, typeof(ulong));
			//Handle<ulong, short>((a, b) => a | b, typeof(ulong));
			Handle<ulong, ushort>((a, b) => a | b, typeof(ulong));
			//Handle<ulong, int>((a, b) => a | b, typeof(ulong));
			Handle<ulong, uint>((a, b) => a | b, typeof(ulong));
			//Handle<ulong, long>((a, b) => a | b, typeof(ulong));
			Handle<ulong, ulong>((a, b) => a | b, typeof(ulong));
		}

		public override string GetDescriptionFormat(Type leftType, Type rightType) => "Returns the bitwise union of {0}.";

		protected override object CustomHandling(object leftOperand, object rightOperand)
		{
			if (leftOperand.GetType() is var leftType && leftType == rightOperand.GetType() && leftType.IsPseudoFlagsEnum())
			{
				return Enum.ToObject(leftType, (ulong) Convert.ChangeType(leftOperand, typeof(ulong)) ^ (ulong) Convert.ChangeType(rightOperand, typeof(ulong)));
			}

			return base.CustomHandling(leftOperand, rightOperand);
		}

		protected override Type GetCustomHandlingType(Type leftType, Type rightType)
		{
			if (leftType == rightType && leftType.IsPseudoFlagsEnum())
			{
				return leftType;
			}

			return null;
		}
	}
}