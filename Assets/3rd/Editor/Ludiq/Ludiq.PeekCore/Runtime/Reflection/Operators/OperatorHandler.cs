namespace Ludiq.PeekCore
{
	public abstract class OperatorHandler
	{
		protected OperatorHandler(OperatorCategory category, string name, string verb, string symbol, string fancySymbol, string customMethodName)
		{
			Ensure.That(nameof(name)).IsNotNull(name);
			Ensure.That(nameof(verb)).IsNotNull(verb);
			Ensure.That(nameof(symbol)).IsNotNull(symbol);

			this.category = category;
			this.name = name;
			this.verb = verb;
			this.symbol = symbol;
			this.fancySymbol = fancySymbol;
			this.customMethodName = customMethodName;
		}

		public OperatorCategory category { get; }
		public string name { get; }
		public string verb { get; }
		public string symbol { get; }
		public string fancySymbol { get; }
		public string customMethodName { get; }
	}
}