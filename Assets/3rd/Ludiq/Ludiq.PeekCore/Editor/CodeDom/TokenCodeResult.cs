using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class TokenCodeResult
	{
		public List<Token> tokens { get; }

		public Dictionary<CodeElement, List<Token>> tokensByElements { get; }

		public TokenCodeResult(List<Token> tokens, Dictionary<CodeElement, List<Token>> tokensByElements)
		{
			this.tokens = tokens;
			this.tokensByElements = tokensByElements;
		}
	}
}
