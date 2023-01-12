using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public class SyntaxHighlightingResult
	{
		public SyntaxHighlightingResult(List<Token> tokens, Dictionary<Token, SyntaxHighlightingTokenEntry> tokenEntries, string rawText, string formattedText)
		{
			Tokens = tokens;
			TokenEntries = tokenEntries;
			RawText = rawText;
			FormattedText = formattedText;
		}

		public List<Token> Tokens { get; }
		public Dictionary<Token, SyntaxHighlightingTokenEntry> TokenEntries { get; }
		public string RawText { get; }
		public string FormattedText { get; }
	}
}
