namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeStructTypeDeclaration : CodeCompositeTypeDeclaration
	{
		public CodeStructTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers, name)
		{
		}

		public override bool IsInterface => false;
		public override string Keyword => "struct";
	}
}
