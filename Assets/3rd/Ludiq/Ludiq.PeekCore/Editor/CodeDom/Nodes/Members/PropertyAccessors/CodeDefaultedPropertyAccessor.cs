namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeDefaultedPropertyAccessor : CodeBasicPropertyAccessor
	{
		public CodeDefaultedPropertyAccessor(CodeMemberModifiers modifiers)
			: base(modifiers)
		{
		}
		
		protected override void GenerateInner(CodeGenerator generator, string name, CodeBasicPropertyMember property, CodeCompositeTypeDeclaration enclosingType)
		{
			generator.Write(TokenType.Keyword, name);
			generator.WriteLine(TokenType.Punctuation, ";");
		}
	}
}
