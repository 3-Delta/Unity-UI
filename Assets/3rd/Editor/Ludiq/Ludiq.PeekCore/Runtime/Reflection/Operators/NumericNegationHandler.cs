using System;

namespace Ludiq.PeekCore
{
	public sealed class NumericNegationHandler : UnaryOperatorHandler
	{
		public NumericNegationHandler() : base(OperatorCategory.Math, UnaryOperator.NumericNegation, "Numeric Negation", "Negate", "-", "-", "op_UnaryNegation")
		{
			Handle<byte>(a => -a);
			Handle<sbyte>(a => -a);
			Handle<short>(a => -a);
			Handle<ushort>(a => -a);
			Handle<int>(a => -a);
			Handle<uint>(a => -a);
			Handle<long>(a => -a);
			//Handle<ulong>(a => -a);
			Handle<float>(a => -a);
			Handle<decimal>(a => -a);
			Handle<double>(a => -a);
		}

		public override string GetDescriptionFormat(Type type) => "Returns the numeric negation of {0}.";
	}
}