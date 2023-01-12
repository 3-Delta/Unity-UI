using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore
{
	public sealed class LambdaFuzzyOptionDocumentation : IFuzzyOptionDocumentation
	{
		public LambdaFuzzyOptionDocumentation() { }

		public LambdaFuzzyOptionDocumentation(string summary)
		{
			this.summary = summary;
		}

		public string summary { get; set; }

		public string returns { get; set; }

		public string remarks { get; set; }

		public string returnType { get; set; }

		public EditorTexture returnIcon { get; set; }

		public IEnumerable<string> parameters => Enumerable.Empty<string>();

		public string GetParameterName(string parameter) => parameter;

		public string GetParameterSummary(string parameter) => null;

		public string GetParameterType(string parameter) => null;

		public EditorTexture GetParameterIcon(string parameter) => null;
	}
}