namespace Ludiq.PeekCore.CodeDom
{
	public enum TokenType
	{
		Invalid,

		Keyword,
		CharLiteral,
		StringLiteral,
		FloatLiteral,
		IntLiteral,
		BoolLiteral,
		NullLiteral,
		Comment,

		Space,
		Newline,
		Indentation,

		Directive,
		Punctuation,
		Operator,
		Identifier,
		TypeIdentifier,
		GenericTypeParameter,

		Error,
	}
}
