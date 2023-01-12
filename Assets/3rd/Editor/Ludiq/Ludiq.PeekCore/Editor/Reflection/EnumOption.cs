using System;
using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(Enum), typeof(EnumOption))]

namespace Ludiq.PeekCore
{
	public class EnumOption : DocumentedFuzzyOption<Enum>
	{
		public EnumOption(Enum @enum) : base(FuzzyOptionMode.Leaf)
		{
			value = @enum;
			label = @enum.HumanName();
			getIcon = @enum.Icon;
			documentation = new XmlFuzzyOptionDocumentation(@enum.Documentation());
			zoom = true;
		}
	}
}