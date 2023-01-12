namespace Ludiq.PeekCore
{
	public sealed class FuzzySeparator : FuzzyOption<object>
	{
		public FuzzySeparator(string label) : base(FuzzyOptionMode.Leaf)
		{
			this.label = label.ToUpper();
			value = this;
		}

		public override string haystack => null;
	}
}