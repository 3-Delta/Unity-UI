using System.Collections.Generic;
using System.Text;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class TokenCodeWriter : ICodeWriter
	{
		private bool needsFlush = false;
		private int tokenIndent;
		private TokenType tokenType;
		private StringBuilder tokenTextBuilder = new StringBuilder();
		private TokenPosition currentPosition;
		private CodeElement tokenDirectOwner;
		private List<CodeElement> tokenTransitiveOwners = new List<CodeElement>();
		private List<CodeElement> elementStack = new List<CodeElement>();

		public List<Token> tokens { get; } = new List<Token>();
		public Dictionary<CodeElement, List<Token>> tokensByElements { get; } = new Dictionary<CodeElement, List<Token>>();

		public int Indent { get; set; }
		
		public void Dispose()
		{ 
		}

		public void Flush()
		{
			if (needsFlush)
			{
				var token = new Token(tokenDirectOwner, tokenType, tokenTextBuilder.ToString(), tokenIndent);
				tokens.Add(token);

				foreach (var owner in tokenTransitiveOwners)
				{
					if (!tokensByElements.TryGetValue(owner, out var tokensByElement))
					{
						tokensByElement = new List<Token>();
						tokensByElements[owner] = tokensByElement;
					}

					tokensByElement.Add(token);
				}

				needsFlush = false;
			}
		}

		public void Write(TokenType type, char text)
		{
			Write(type, text.ToString());
		}

		public void Write(TokenType type, string text)
		{
			if (text.Length == 0)
			{
				return;
			}

			bool createToken = true;
			var endPosition = new TokenPosition(currentPosition.LineIndex, currentPosition.ColumnIndex + text.Length);

			var owner = elementStack.Count > 0 ? elementStack[elementStack.Count - 1] : null;
			
			/*if (justEnteredElement && type == TokenType.Newline && elementStack.Count > 1)
			{
				owner = elementStack[elementStack.Count - 2];
			}*/

			// If the last written token has the same type and is on the same line, merge the token contents together.
			if (tokens.Count > 0)
			{
				if (tokenDirectOwner == owner && tokenType == type)
				{
					tokenTextBuilder.Append(text);

					createToken = false;
				}
			}

			if (createToken)
			{
				Flush();

				tokenDirectOwner = owner;
				tokenTransitiveOwners.Clear();
				tokenTransitiveOwners.AddRange(elementStack);
				tokenType = type;
				tokenTextBuilder.Length = 0;
				tokenTextBuilder.Append(text);
				tokenIndent = Indent;

				needsFlush = true;
			}

			currentPosition = endPosition;
		}

		public void WriteLine()
		{
			Write(TokenType.Newline, '\n');
		}

		public void WriteLine(TokenType type, char text)
		{
			Write(type, text.ToString());
			WriteLine();
		}

		public void WriteLine(TokenType type, string text)
		{
			Write(type, text);
			WriteLine();
		}

		public void EnterElement(CodeElement element)
		{
			elementStack.Add(element);
		}

		public void ExitElement()
		{
			elementStack.RemoveAt(elementStack.Count - 1);
		}
	}
}
