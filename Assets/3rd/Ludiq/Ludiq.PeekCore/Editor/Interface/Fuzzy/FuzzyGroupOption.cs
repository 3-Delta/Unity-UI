using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(FuzzyGroup), typeof(FuzzyGroupOption))]

namespace Ludiq.PeekCore
{
	public class FuzzyGroupOption : FuzzyOption<object>
	{
		public FuzzyGroupOption(FuzzyGroup group) : base(FuzzyOptionMode.Branch)
		{
			value = group;
			label = group.label;
			icon = group.icon;
		}
	}
}