namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeClassTypeDeclaration : CodeCompositeTypeDeclaration
	{
		public CodeClassTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers, name)
		{
		}

		public override bool IsInterface => false;
		public override string Keyword => "class";
	}
}
