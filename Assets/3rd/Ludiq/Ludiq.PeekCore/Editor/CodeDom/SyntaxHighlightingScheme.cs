using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ludiq.PeekCore.CodeDom
{
	public class SyntaxHighlightingScheme
	{
		public static readonly SyntaxHighlightingScheme DefaultScheme = new SyntaxHighlightingScheme(new[] 
		{
			new SyntaxHighlightingRule(TokenType.Comment, ColorPalette.green),
			new SyntaxHighlightingRule(new[] { TokenType.Keyword, TokenType.NullLiteral, TokenType.BoolLiteral }, ColorPalette.blue),
			new SyntaxHighlightingRule(TokenType.IntLiteral, typeof(int).Color()),
			new SyntaxHighlightingRule(TokenType.FloatLiteral, typeof(float).Color()),
			new SyntaxHighlightingRule(TokenType.StringLiteral, typeof(string).Color()),
			new SyntaxHighlightingRule(TokenType.TypeIdentifier, ColorPalette.teal),
			new SyntaxHighlightingRule(TokenType.GenericTypeParameter, ColorPalette.yellow),
			new SyntaxHighlightingRule(TokenType.Error, ColorPalette.red),
		});
		
		private Dictionary<TokenType, SyntaxHighlightingRule> rulesByTokenTypes = new Dictionary<TokenType, SyntaxHighlightingRule>();

		public SyntaxHighlightingScheme(IEnumerable<SyntaxHighlightingRule> rules)
		{
			Rules = rules.ToList();

			foreach (var rule in Rules)
			{
				foreach (var tokenType in rule.TokenTypes)
				{
					rulesByTokenTypes[tokenType] = rule;
				}
			}
		}

		public IEnumerable<SyntaxHighlightingRule> Rules { get; }

		public bool TryGetRule(TokenType tokenType, out SyntaxHighlightingRule rule)
		{
			return rulesByTokenTypes.TryGetValue(tokenType, out rule);
		}

		public SyntaxHighlightingResult FormatTokens(IEnumerable<Token> tokens, int unindentLevel, int maxWidth)
		{
			var tokensList = tokens.ToList();
			var tokenEntries = new Dictionary<Token, SyntaxHighlightingTokenEntry>();
			var rawText = new StringBuilder();
			var formattedText = new StringBuilder();
			var position = new TokenPosition();
			var tokenIndex = 0;

			var indentationText = "    ";

			while (tokenIndex < tokensList.Count)
			{
				var token = tokensList[tokenIndex];

				int offsetBefore = rawText.Length;
				TokenPosition nextPosition = position;

				if (token.Type == TokenType.Indentation)
				{
					var tokenIndent = token.Indent;
					if (tokenIndent != 0)
					{
						var rawTokenText = token.Text;
						var indent = Math.Max(tokenIndent - unindentLevel, 0);

						var rawLengthBefore = rawText.Length;
						for (int i = 0; i < indent; i++)
						{
							rawText.Append(indentationText);
							formattedText.Append(indentationText);
						}

						nextPosition = new TokenPosition(position.LineIndex, position.ColumnIndex + (rawText.Length - rawLengthBefore));
					}
				}
				else if (token.Type == TokenType.Newline)
				{
					nextPosition = new TokenPosition(position.LineIndex + 1, 0);
					rawText.Append(Environment.NewLine);
					formattedText.Append(Environment.NewLine);
				}
				else
				{
					var rawTokenText = token.Text;

					if (position.ColumnIndex + rawTokenText.Length >= maxWidth)
					{
						if (tokenIndex > 0 && tokensList[tokenIndex - 1].Type == TokenType.Indentation)
						{					
							// TODO: if the token still doesn't fit in one line after line breaking and indent, word-wrap or character-wrap the token text.
						}
						else
						{
							tokensList.Insert(tokenIndex, new Token(null, TokenType.Newline, Environment.NewLine, token.Indent));

							string s = "";
							for (int i = 0; i < token.Indent; i++)
							{
								s += indentationText;
							}

							tokensList.Insert(tokenIndex + 1, new Token(null, TokenType.Indentation, s, token.Indent));

							if (token.Type == TokenType.Space)
							{
								tokensList.RemoveAt(tokenIndex + 2);
							}

							continue;
						}
					}

					var escapedTokenText = LudiqGUIUtility.EscapeRichText(rawTokenText);

					rawText.Append(rawTokenText);

					nextPosition = new TokenPosition(position.LineIndex, position.ColumnIndex + rawTokenText.Length);

					if (TryGetRule(token.Type, out var rule))
					{
						formattedText.Append(rule.StartMarkup).Append(escapedTokenText).Append(rule.EndMarkup);
					}
					else
					{
						formattedText.Append(escapedTokenText);
					}
				}

				tokenEntries[token] = new SyntaxHighlightingTokenEntry(offsetBefore, rawText.Length, position, nextPosition);
				position = nextPosition;

				++tokenIndex;
			}

			if (formattedText.Length > 0 && formattedText[formattedText.Length - 1] != '\n')
			{
				rawText.Append(Environment.NewLine);
				formattedText.Append(Environment.NewLine);
			}

			return new SyntaxHighlightingResult(tokensList, tokenEntries, rawText.ToString(), formattedText.ToString());
		}
	}
}
