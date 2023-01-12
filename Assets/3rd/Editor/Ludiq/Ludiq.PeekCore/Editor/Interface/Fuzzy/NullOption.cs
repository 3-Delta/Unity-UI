namespace Ludiq.PeekCore
{
	public sealed class NullOption : FuzzyOption<object>
	{
		public NullOption() : base(FuzzyOptionMode.Leaf)
		{
			label = "(None)";
			value = null;
		}
	}
}