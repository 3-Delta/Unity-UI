namespace Ludiq.PeekCore.CodeDom
{
	public class Token
	{
		public Token(CodeElement owner, TokenType type, string text, int indent)
		{
			Owner = owner;
			Type = type;
			Text = text;
			Indent = indent;
		}

		public CodeElement Owner { get; }
		public TokenType Type { get; }
		public string Text { get; }
		public int Indent { get; }
	}
}
