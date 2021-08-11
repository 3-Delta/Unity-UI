using System;

namespace Ludiq.PeekCore
{
	public sealed class BitwiseNegationHandler : UnaryOperatorHandler
	{
		public BitwiseNegationHandler() : base(OperatorCategory.Bitwise, UnaryOperator.BitwiseNegation, "Bitwise Negation", "Bitwise Not", "~", "~", "op_OnesComplement")
		{
			Handle<bool>(a => !a);
			Handle<byte>(a => ~a);
			Handle<sbyte>(a => ~a);
			Handle<short>(a => ~a);
			Handle<ushort>(a => ~a);
			Handle<int>(a => ~a);
			Handle<uint>(a => ~a);
			Handle<long>(a => ~a);
			Handle<ulong>(a => ~a);
		}

		public override string GetDescriptionFormat(Type type) => "Returns the bitwise complement of {0}.";

		protected override object CustomHandling(object operand)
		{
			if (operand.GetType() is var type && type.IsPseudoFlagsEnum())
			{
				return Enum.ToObject(type, ~(ulong) Convert.ChangeType(type, typeof(ulong)));
			}

			return base.CustomHandling(operand);
		}

		protected override bool HasCustomHandling(Type type)
		{
			return type.IsPseudoFlagsEnum();
		}
	}
}