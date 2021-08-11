using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(Namespace), typeof(NamespaceOption))]

namespace Ludiq.PeekCore
{
	public class NamespaceOption : FuzzyOption<Namespace>
	{
		public NamespaceOption(Namespace @namespace, FuzzyOptionMode mode) : base(mode)
		{
			value = @namespace;
			label = @namespace.DisplayName(false);
			getIcon = @namespace.Icon;
		}

		public override string haystack => value.DisplayName();
	}
}