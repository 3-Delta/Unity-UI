using System;

namespace Ludiq.PeekCore
{
	public sealed class MultiplicationHandler : BinaryOperatorHandler
	{
		public MultiplicationHandler() : base(OperatorCategory.Math, BinaryOperator.Multiplication, "Multiplication", "Multiply", "*", "\u00D7", "op_Multiply")
		{
            Handle<byte, byte>((a, b) => a * b, typeof(byte));
            Handle<byte, sbyte>((a, b) => a * b, typeof(int));
            Handle<byte, short>((a, b) => a * b, typeof(int));
            Handle<byte, ushort>((a, b) => a * b, typeof(uint));
            Handle<byte, int>((a, b) => a * b, typeof(int));
            Handle<byte, uint>((a, b) => a * b, typeof(uint));
            Handle<byte, long>((a, b) => a * b, typeof(long));
            Handle<byte, ulong>((a, b) => a * b, typeof(ulong));
            Handle<byte, float>((a, b) => a * b, typeof(float));
            Handle<byte, decimal>((a, b) => a * b, typeof(decimal));
            Handle<byte, double>((a, b) => a * b, typeof(double));

            Handle<sbyte, byte>((a, b) => a * b, typeof(int));
            Handle<sbyte, sbyte>((a, b) => a * b, typeof(sbyte));
            Handle<sbyte, short>((a, b) => a * b, typeof(int));
            Handle<sbyte, ushort>((a, b) => a * b, typeof(int));
            Handle<sbyte, int>((a, b) => a * b, typeof(int));
            Handle<sbyte, uint>((a, b) => a * b, typeof(long));
            Handle<sbyte, long>((a, b) => a * b, typeof(long));
            //Handle<sbyte, ulong>((a, b) => a * b, typeof(long));
            Handle<sbyte, float>((a, b) => a * b, typeof(float));
            Handle<sbyte, decimal>((a, b) => a * b, typeof(decimal));
            Handle<sbyte, double>((a, b) => a * b, typeof(double));

            Handle<short, byte>((a, b) => a * b, typeof(int));
            Handle<short, sbyte>((a, b) => a * b, typeof(int));
            Handle<short, short>((a, b) => a * b, typeof(short));
            Handle<short, ushort>((a, b) => a * b, typeof(int));
            Handle<short, int>((a, b) => a * b, typeof(int));
            Handle<short, uint>((a, b) => a * b, typeof(long));
            Handle<short, long>((a, b) => a * b, typeof(long));
            //Handle<short, ulong>((a, b) => a * b, typeof(ulong));
            Handle<short, float>((a, b) => a * b, typeof(float));
            Handle<short, decimal>((a, b) => a * b, typeof(decimal));
            Handle<short, double>((a, b) => a * b, typeof(double));

            Handle<ushort, byte>((a, b) => a * b, typeof(uint));
            Handle<ushort, sbyte>((a, b) => a * b, typeof(int));
            Handle<ushort, short>((a, b) => a * b, typeof(int));
            Handle<ushort, ushort>((a, b) => a * b, typeof(ushort));
            Handle<ushort, int>((a, b) => a * b, typeof(int));
            Handle<ushort, uint>((a, b) => a * b, typeof(uint));
            Handle<ushort, long>((a, b) => a * b, typeof(long));
            Handle<ushort, ulong>((a, b) => a * b, typeof(ulong));
            Handle<ushort, float>((a, b) => a * b, typeof(float));
            Handle<ushort, decimal>((a, b) => a * b, typeof(decimal));
            Handle<ushort, double>((a, b) => a * b, typeof(double));

            Handle<int, byte>((a, b) => a * b, typeof(int));
            Handle<int, sbyte>((a, b) => a * b, typeof(int));
            Handle<int, short>((a, b) => a * b, typeof(int));
            Handle<int, ushort>((a, b) => a * b, typeof(int));
            Handle<int, int>((a, b) => a * b, typeof(int));
            Handle<int, uint>((a, b) => a * b, typeof(long));
            Handle<int, long>((a, b) => a * b, typeof(long));
            //Handle<int, ulong>((a, b) => a * b, typeof(ulong));
            Handle<int, float>((a, b) => a * b, typeof(float));
            Handle<int, decimal>((a, b) => a * b, typeof(decimal));
            Handle<int, double>((a, b) => a * b, typeof(double));

            Handle<uint, byte>((a, b) => a * b, typeof(uint));
            Handle<uint, sbyte>((a, b) => a * b, typeof(long));
            Handle<uint, short>((a, b) => a * b, typeof(long));
            Handle<uint, ushort>((a, b) => a * b, typeof(uint));
            Handle<uint, int>((a, b) => a * b, typeof(long));
            Handle<uint, uint>((a, b) => a * b, typeof(uint));
            Handle<uint, long>((a, b) => a * b, typeof(long));
            Handle<uint, ulong>((a, b) => a * b, typeof(ulong));
            Handle<uint, float>((a, b) => a * b, typeof(float));
            Handle<uint, decimal>((a, b) => a * b, typeof(decimal));
            Handle<uint, double>((a, b) => a * b, typeof(double));

            Handle<long, byte>((a, b) => a * b, typeof(long));
            Handle<long, sbyte>((a, b) => a * b, typeof(long));
            Handle<long, short>((a, b) => a * b, typeof(long));
            Handle<long, ushort>((a, b) => a * b, typeof(long));
            Handle<long, int>((a, b) => a * b, typeof(long));
            Handle<long, uint>((a, b) => a * b, typeof(long));
            Handle<long, long>((a, b) => a * b, typeof(long));
            //Handle<long, ulong>((a, b) => a * b, typeof(ulong));
            Handle<long, float>((a, b) => a * b, typeof(float));
            Handle<long, decimal>((a, b) => a * b, typeof(decimal));
            Handle<long, double>((a, b) => a * b, typeof(double));

            Handle<ulong, byte>((a, b) => a * b, typeof(ulong));
            //Handle<ulong, sbyte>((a, b) => a * b, typeof(ulong));
            //Handle<ulong, short>((a, b) => a * b, typeof(ulong));
            Handle<ulong, ushort>((a, b) => a * b, typeof(ulong));
            //Handle<ulong, int>((a, b) => a * b, typeof(ulong));
            Handle<ulong, uint>((a, b) => a * b, typeof(ulong));
            //Handle<ulong, long>((a, b) => a * b, typeof(ulong));
            Handle<ulong, ulong>((a, b) => a * b, typeof(ulong));
            Handle<ulong, float>((a, b) => a * b, typeof(float));
            Handle<ulong, decimal>((a, b) => a * b, typeof(decimal));
            Handle<ulong, double>((a, b) => a * b, typeof(double));

            Handle<float, byte>((a, b) => a * b, typeof(float));
            Handle<float, sbyte>((a, b) => a * b, typeof(float));
            Handle<float, short>((a, b) => a * b, typeof(float));
            Handle<float, ushort>((a, b) => a * b, typeof(float));
            Handle<float, int>((a, b) => a * b, typeof(float));
            Handle<float, uint>((a, b) => a * b, typeof(float));
            Handle<float, long>((a, b) => a * b, typeof(float));
            Handle<float, ulong>((a, b) => a * b, typeof(float));
            Handle<float, float>((a, b) => a * b, typeof(float));
            //Handle<float, decimal>((a, b) => a * b, typeof(decimal));
            Handle<float, double>((a, b) => a * b, typeof(double));

            Handle<decimal, byte>((a, b) => a * b, typeof(decimal));
            Handle<decimal, sbyte>((a, b) => a * b, typeof(decimal));
            Handle<decimal, short>((a, b) => a * b, typeof(decimal));
            Handle<decimal, ushort>((a, b) => a * b, typeof(decimal));
            Handle<decimal, int>((a, b) => a * b, typeof(decimal));
            Handle<decimal, uint>((a, b) => a * b, typeof(decimal));
            Handle<decimal, long>((a, b) => a * b, typeof(decimal));
            Handle<decimal, ulong>((a, b) => a * b, typeof(decimal));
            //Handle<decimal, float>((a, b) => a * b, typeof(decimal));
            Handle<decimal, decimal>((a, b) => a * b, typeof(decimal));
            //Handle<decimal, double>((a, b) => a * b, typeof(decimal));

            Handle<double, byte>((a, b) => a * b, typeof(double));
            Handle<double, sbyte>((a, b) => a * b, typeof(double));
            Handle<double, short>((a, b) => a * b, typeof(double));
            Handle<double, ushort>((a, b) => a * b, typeof(double));
            Handle<double, int>((a, b) => a * b, typeof(double));
            Handle<double, uint>((a, b) => a * b, typeof(double));
            Handle<double, long>((a, b) => a * b, typeof(double));
            Handle<double, ulong>((a, b) => a * b, typeof(double));
            Handle<double, float>((a, b) => a * b, typeof(double));
            //Handle<double, decimal>((a, b) => a * b, typeof(decimal));
            Handle<double, double>((a, b) => a * b, typeof(double));
		}

		public override string GetDescriptionFormat(Type leftType, Type rightType) => "Returns the product of {0}.";
	}
}