namespace Ludiq.PeekCore
{
	public class FuzzyOptionProvider : SingleDecoratorProvider<object, IFuzzyOption, RegisterFuzzyOptionAttribute>
	{
		static FuzzyOptionProvider()
		{
			instance = new FuzzyOptionProvider();
		}

		public static FuzzyOptionProvider instance { get; private set; }

		protected override bool cache => false;
	}
}