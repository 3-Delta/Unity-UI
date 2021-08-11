using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore
{
	public sealed class XmlFuzzyOptionDocumentation : IFuzzyOptionDocumentation
	{
		private readonly XmlDocumentationTags tags;

		public XmlFuzzyOptionDocumentation(XmlDocumentationTags tags)
		{
			this.tags = tags;
		}
		
		public string summary => tags?.summary;

		public string returns => tags?.returns;

		public string remarks => tags?.remarks;

		public string returnType => tags?.returnType.DisplayName();

		public EditorTexture returnIcon => tags?.returnType.Icon();

		public IEnumerable<string> parameters => tags?.parameters.Keys ?? Enumerable.Empty<string>();

		public string GetParameterName(string parameter)
		{
			if (LudiqCore.Configuration.humanNaming)
			{
				return parameter.Prettify();
			}
			else
			{
				return parameter;
			}
		}

		public string GetParameterSummary(string parameter)
		{
			tags.parameters.TryGetValue(parameter, out var parameterSummary);

			return parameterSummary;
		}

		public string GetParameterType(string parameter)
		{
			if (tags.parameterTypes.TryGetValue(parameter, out var parameterType))
			{
				return parameterType.DisplayName();
			}
			else
			{
				return null;
			}
		}

		public EditorTexture GetParameterIcon(string parameter)
		{
			if (tags.parameterTypes.TryGetValue(parameter, out var parameterType))
			{
				return parameterType.Icon();
			}
			else
			{
				return null;
			}
		}
	}
}
