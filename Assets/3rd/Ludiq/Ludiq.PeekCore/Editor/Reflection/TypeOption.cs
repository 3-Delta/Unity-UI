using System;
using Ludiq.PeekCore;

[assembly: RegisterFuzzyOption(typeof(Type), typeof(TypeOption))]

namespace Ludiq.PeekCore
{
	public class TypeOption : DocumentedFuzzyOption<Type>
	{
		public TypeOption(Type type, FuzzyOptionMode mode) : base(mode)
		{
			value = type;
			label = type.DisplayName();
			getIcon = type.Icon;
			documentation = new XmlFuzzyOptionDocumentation(type.Documentation());
			zoom = true;
		}

		public override string SearchResultLabel(string query)
		{
			return $"{SearchUtility.HighlightQuery(haystack, query)} <color=#{ColorPalette.unityForegroundDim.ToHexString()}>(in {value.Namespace().DisplayName()})</color>";
		}
	}
}