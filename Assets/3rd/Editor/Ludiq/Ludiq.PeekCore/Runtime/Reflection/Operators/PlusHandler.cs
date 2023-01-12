using System;

namespace Ludiq.PeekCore
{
	public sealed class PlusHandler : UnaryOperatorHandler
	{
		public PlusHandler() : base(OperatorCategory.Math, UnaryOperator.Plus, "Plus", "Plus", "+", "+", "op_UnaryPlus")
		{
			Handle<byte>(a => +a);
			Handle<sbyte>(a => +a);
			Handle<short>(a => +a);
			Handle<ushort>(a => +a);
			Handle<int>(a => +a);
			Handle<uint>(a => +a);
			Handle<long>(a => +a);
			Handle<ulong>(a => +a);
			Handle<float>(a => +a);
			Handle<decimal>(a => +a);
			Handle<double>(a => +a);
		}

		public override string GetDescriptionFormat(Type type) => "Applies the unary plus operator to {0}.";
	}
}