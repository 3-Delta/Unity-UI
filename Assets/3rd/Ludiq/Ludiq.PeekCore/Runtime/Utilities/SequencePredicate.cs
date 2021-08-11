namespace Ludiq.PeekCore
{
	public enum SequencePredicate
	{
		Any,

		All,

		None,

		NotAll
	}

	public static class XSequencePredicate
	{
		public static SequencePredicate Positive(this SequencePredicate predicate)
		{
			switch (predicate)
			{
				case SequencePredicate.Any: return SequencePredicate.Any;
				case SequencePredicate.All: return SequencePredicate.All;
				case SequencePredicate.None: return SequencePredicate.Any;
				case SequencePredicate.NotAll: return SequencePredicate.All;
				default: throw predicate.Unexpected();
			}
		}
		
		public static SequencePredicate Negative(this SequencePredicate predicate)
		{
			switch (predicate)
			{
				case SequencePredicate.Any: return SequencePredicate.None;
				case SequencePredicate.All: return SequencePredicate.NotAll;
				case SequencePredicate.None: return SequencePredicate.None;
				case SequencePredicate.NotAll: return SequencePredicate.NotAll;
				default: throw predicate.Unexpected();
			}
		}

		public static bool IsPositive(this SequencePredicate predicate)
		{
			switch (predicate)
			{
				case SequencePredicate.Any:
				case SequencePredicate.All:
					return true;

				case SequencePredicate.None:
				case SequencePredicate.NotAll:
					return false;

				default: throw predicate.Unexpected();
			}
		}

		public static bool IsNegative(this SequencePredicate predicate)
		{
			return !predicate.IsPositive();
		}

		public static bool IsHollistic(this SequencePredicate predicate)
		{
			switch (predicate)
			{
				case SequencePredicate.Any:
				case SequencePredicate.NotAll:
					return false;

				case SequencePredicate.All:
				case SequencePredicate.None:
					return true;

				default: throw predicate.Unexpected();
			}
		}

		public static bool IsIndividual(this SequencePredicate predicate)
		{
			return !predicate.IsHollistic();
		}
	}
}