using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public static class TokenUtility
	{
		public static bool IsWhitespace(this TokenType tokenType)
		{
			return tokenType == TokenType.Space
				|| tokenType == TokenType.Newline
				|| tokenType == TokenType.Indentation;
		}

		public static IEnumerable<Token> Trimmed(this IEnumerable<Token> tokens)
		{
			var result = tokens.ToList();

			for (int i = 0; i < result.Count;)
			{
				var token = result[i];
				if (token.Type.IsWhitespace())
				{
					result.RemoveAt(i);
				}
				else
				{
					break;
				}
			}

			for (int i = result.Count - 1; i >= 0; i--)
			{
				var token = result[i];
				if (token.Type.IsWhitespace())
				{
					result.RemoveAt(i);
				}
				else
				{
					break;
				}
			}

			return result;
		}
	}
}
