using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public interface IFuzzyOptionDocumentation
	{
		string summary { get; }

		string returns { get; }

		string remarks { get; }

		string returnType { get; }

		EditorTexture returnIcon { get; }

		IEnumerable<string> parameters { get; }

		string GetParameterName(string parameter);

		string GetParameterSummary(string parameter);

		string GetParameterType(string parameter);

		EditorTexture GetParameterIcon(string parameter);
	}
}