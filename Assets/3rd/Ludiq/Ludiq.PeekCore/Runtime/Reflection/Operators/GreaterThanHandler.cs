using System;

namespace Ludiq.PeekCore
{
	public class GreaterThanHandler : BinaryOperatorHandler
	{
		public GreaterThanHandler() : base(OperatorCategory.Comparison, BinaryOperator.GreaterThan, "Greater Than", "Greater Than", ">", ">", "op_GreaterThan")
		{
			Handle<byte, byte>((a, b) => a > b, typeof(bool));
            Handle<byte, sbyte>((a, b) => a > b, typeof(bool));
            Handle<byte, short>((a, b) => a > b, typeof(bool));
            Handle<byte, ushort>((a, b) => a > b, typeof(bool));
            Handle<byte, int>((a, b) => a > b, typeof(bool));
            Handle<byte, uint>((a, b) => a > b, typeof(bool));
            Handle<byte, long>((a, b) => a > b, typeof(bool));
            Handle<byte, ulong>((a, b) => a > b, typeof(bool));
            Handle<byte, float>((a, b) => a > b, typeof(bool));
            Handle<byte, decimal>((a, b) => a > b, typeof(bool));
            Handle<byte, double>((a, b) => a > b, typeof(bool));

            Handle<sbyte, byte>((a, b) => a > b, typeof(bool));
            Handle<sbyte, sbyte>((a, b) => a > b, typeof(bool));
            Handle<sbyte, short>((a, b) => a > b, typeof(bool));
            Handle<sbyte, ushort>((a, b) => a > b, typeof(bool));
            Handle<sbyte, int>((a, b) => a > b, typeof(bool));
            Handle<sbyte, uint>((a, b) => a > b, typeof(bool));
            Handle<sbyte, long>((a, b) => a > b, typeof(bool));
            //Handle<sbyte, ulong>((a, b) => a > b, typeof(bool));
            Handle<sbyte, float>((a, b) => a > b, typeof(bool));
            Handle<sbyte, decimal>((a, b) => a > b, typeof(bool));
            Handle<sbyte, double>((a, b) => a > b, typeof(bool));

            Handle<short, byte>((a, b) => a > b, typeof(bool));
            Handle<short, sbyte>((a, b) => a > b, typeof(bool));
            Handle<short, short>((a, b) => a > b, typeof(bool));
            Handle<short, ushort>((a, b) => a > b, typeof(bool));
            Handle<short, int>((a, b) => a > b, typeof(bool));
            Handle<short, uint>((a, b) => a > b, typeof(bool));
            Handle<short, long>((a, b) => a > b, typeof(bool));
            //Handle<short, ulong>((a, b) => a > b, typeof(bool));
            Handle<short, float>((a, b) => a > b, typeof(bool));
            Handle<short, decimal>((a, b) => a > b, typeof(bool));
            Handle<short, double>((a, b) => a > b, typeof(bool));

            Handle<ushort, byte>((a, b) => a > b, typeof(bool));
            Handle<ushort, sbyte>((a, b) => a > b, typeof(bool));
            Handle<ushort, short>((a, b) => a > b, typeof(bool));
            Handle<ushort, ushort>((a, b) => a > b, typeof(bool));
            Handle<ushort, int>((a, b) => a > b, typeof(bool));
            Handle<ushort, uint>((a, b) => a > b, typeof(bool));
            Handle<ushort, long>((a, b) => a > b, typeof(bool));
            Handle<ushort, ulong>((a, b) => a > b, typeof(bool));
            Handle<ushort, float>((a, b) => a > b, typeof(bool));
            Handle<ushort, decimal>((a, b) => a > b, typeof(bool));
            Handle<ushort, double>((a, b) => a > b, typeof(bool));

            Handle<int, byte>((a, b) => a > b, typeof(bool));
            Handle<int, sbyte>((a, b) => a > b, typeof(bool));
            Handle<int, short>((a, b) => a > b, typeof(bool));
            Handle<int, ushort>((a, b) => a > b, typeof(bool));
            Handle<int, int>((a, b) => a > b, typeof(bool));
            Handle<int, uint>((a, b) => a > b, typeof(bool));
            Handle<int, long>((a, b) => a > b, typeof(bool));
            //Handle<int, ulong>((a, b) => a > b, typeof(bool));
            Handle<int, float>((a, b) => a > b, typeof(bool));
            Handle<int, decimal>((a, b) => a > b, typeof(bool));
            Handle<int, double>((a, b) => a > b, typeof(bool));

            Handle<uint, byte>((a, b) => a > b, typeof(bool));
            Handle<uint, sbyte>((a, b) => a > b, typeof(bool));
            Handle<uint, short>((a, b) => a > b, typeof(bool));
            Handle<uint, ushort>((a, b) => a > b, typeof(bool));
            Handle<uint, int>((a, b) => a > b, typeof(bool));
            Handle<uint, uint>((a, b) => a > b, typeof(bool));
            Handle<uint, long>((a, b) => a > b, typeof(bool));
            Handle<uint, ulong>((a, b) => a > b, typeof(bool));
            Handle<uint, float>((a, b) => a > b, typeof(bool));
            Handle<uint, decimal>((a, b) => a > b, typeof(bool));
            Handle<uint, double>((a, b) => a > b, typeof(bool));

            Handle<long, byte>((a, b) => a > b, typeof(bool));
            Handle<long, sbyte>((a, b) => a > b, typeof(bool));
            Handle<long, short>((a, b) => a > b, typeof(bool));
            Handle<long, ushort>((a, b) => a > b, typeof(bool));
            Handle<long, int>((a, b) => a > b, typeof(bool));
            Handle<long, uint>((a, b) => a > b, typeof(bool));
            Handle<long, long>((a, b) => a > b, typeof(bool));
            //Handle<long, ulong>((a, b) => a > b, typeof(bool));
            Handle<long, float>((a, b) => a > b, typeof(bool));
            Handle<long, decimal>((a, b) => a > b, typeof(bool));
            Handle<long, double>((a, b) => a > b, typeof(bool));

            Handle<ulong, byte>((a, b) => a > b, typeof(bool));
            //Handle<ulong, sbyte>((a, b) => a > b, typeof(bool));
            //Handle<ulong, short>((a, b) => a > b, typeof(bool));
            Handle<ulong, ushort>((a, b) => a > b, typeof(bool));
            //Handle<ulong, int>((a, b) => a > b, typeof(bool));
            Handle<ulong, uint>((a, b) => a > b, typeof(bool));
            //Handle<ulong, long>((a, b) => a > b, typeof(bool));
            Handle<ulong, ulong>((a, b) => a > b, typeof(bool));
            Handle<ulong, float>((a, b) => a > b, typeof(bool));
            Handle<ulong, decimal>((a, b) => a > b, typeof(bool));
            Handle<ulong, double>((a, b) => a > b, typeof(bool));

            Handle<float, byte>((a, b) => a > b, typeof(bool));
            Handle<float, sbyte>((a, b) => a > b, typeof(bool));
            Handle<float, short>((a, b) => a > b, typeof(bool));
            Handle<float, ushort>((a, b) => a > b, typeof(bool));
            Handle<float, int>((a, b) => a > b, typeof(bool));
            Handle<float, uint>((a, b) => a > b, typeof(bool));
            Handle<float, long>((a, b) => a > b, typeof(bool));
            Handle<float, ulong>((a, b) => a > b, typeof(bool));
            Handle<float, float>((a, b) => a > b, typeof(bool));
            //Handle<float, decimal>((a, b) => a > b, typeof(bool));
            Handle<float, double>((a, b) => a > b, typeof(bool));

            Handle<decimal, byte>((a, b) => a > b, typeof(bool));
            Handle<decimal, sbyte>((a, b) => a > b, typeof(bool));
            Handle<decimal, short>((a, b) => a > b, typeof(bool));
            Handle<decimal, ushort>((a, b) => a > b, typeof(bool));
            Handle<decimal, int>((a, b) => a > b, typeof(bool));
            Handle<decimal, uint>((a, b) => a > b, typeof(bool));
            Handle<decimal, long>((a, b) => a > b, typeof(bool));
            Handle<decimal, ulong>((a, b) => a > b, typeof(bool));
            //Handle<decimal, float>((a, b) => a > b, typeof(bool));
            Handle<decimal, decimal>((a, b) => a > b, typeof(bool));
            //Handle<decimal, double>((a, b) => a > b, typeof(bool));

            Handle<double, byte>((a, b) => a > b, typeof(bool));
            Handle<double, sbyte>((a, b) => a > b, typeof(bool));
            Handle<double, short>((a, b) => a > b, typeof(bool));
            Handle<double, ushort>((a, b) => a > b, typeof(bool));
            Handle<double, int>((a, b) => a > b, typeof(bool));
            Handle<double, uint>((a, b) => a > b, typeof(bool));
            Handle<double, long>((a, b) => a > b, typeof(bool));
            Handle<double, ulong>((a, b) => a > b, typeof(bool));
            Handle<double, float>((a, b) => a > b, typeof(bool));
            //Handle<double, decimal>((a, b) => a > b, typeof(bool));
            Handle<double, double>((a, b) => a > b, typeof(bool));
		}

		public override string GetDescriptionFormat(Type leftType, Type rightType) => "Compares {0} to determine the first is greater than the second.";
	}
}