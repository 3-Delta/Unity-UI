using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(LooseAssemblyName), typeof(LooseAssemblyNameOption))]

namespace Ludiq.PeekCore
{
	public class LooseAssemblyNameOption : FuzzyOption<LooseAssemblyName>
	{
		public LooseAssemblyNameOption(LooseAssemblyName looseAssemblyName) : base(FuzzyOptionMode.Leaf)
		{
			value = looseAssemblyName;
			label = value.name;
		}
	}
}