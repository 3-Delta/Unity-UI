using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(Member), typeof(MemberOption))]

namespace Ludiq.PeekCore
{
	public sealed class MemberOption : DocumentedFuzzyOption<Member>
	{
		public MemberOption(Member member) : this(member, MemberAction.None, false) { }

		public MemberOption(Member member, MemberAction action, bool expectingBoolean) : base(FuzzyOptionMode.Leaf)
		{
			Ensure.That(nameof(member)).IsNotNull(member);

			value = member;

			documentation = new XmlFuzzyOptionDocumentation(member.info.Documentation());

			getIcon = () => member.pseudoDeclaringType.Icon();

			if (member.isPseudoInherited)
			{
				dim = true;
			}

			if (member.isInvocable)
			{
				label = $"{member.info.DisplayName(action, expectingBoolean)} ({member.methodBase.DisplayParameterString()})";
			}
			else
			{
				label = member.info.DisplayName(action, expectingBoolean);
			}
		}

		public string Haystack(MemberAction action, bool expectingBoolean)
		{
			return MemberNameWithTargetType(action, expectingBoolean);
		}

		private string MemberNameWithTargetType(MemberAction action, bool expectingBoolean)
		{
			return $"{value.targetType.DisplayName()}{(LudiqCore.Configuration.humanNaming ? ": " : ".")}{value.info.DisplayName(action, expectingBoolean)}";
		}

		public string SearchResultLabel(string query, MemberAction action, bool expectingBoolean)
		{
			var label = SearchUtility.HighlightQuery(Haystack(action, expectingBoolean), query);

			if (value.isInvocable)
			{
				label += $" ({value.methodBase.DisplayParameterString()})";
			}

			return label;
		}
	}
}