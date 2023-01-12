using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public class SyntaxHighlightingRule
	{
		public SyntaxHighlightingRule(IEnumerable<TokenType> tokenTypes, string startMarkup, string endMarkup)
		{
			TokenTypes = tokenTypes.ToList();
			StartMarkup = startMarkup;
			EndMarkup = endMarkup;
		}

		public SyntaxHighlightingRule(TokenType tokenType, string startMarkup, string endMarkup)
			: this(tokenType.Yield(), startMarkup, endMarkup)
		{
		}

		public SyntaxHighlightingRule(IEnumerable<TokenType> tokenTypes, SkinnedColor color, bool bold = false) 
			: this(tokenTypes, $"{(bold ? "<b>" : "")}<color=#{color.ToHexString()}>", $"</color>{(bold ? "</b>" : "")}")
		{

		}

		public SyntaxHighlightingRule(TokenType tokenType, SkinnedColor color, bool bold = false) 
			: this(tokenType.Yield(), color, bold)
		{

		}

		public IEnumerable<TokenType> TokenTypes { get; }
		public string StartMarkup { get; }
		public string EndMarkup { get; }
	}
}
