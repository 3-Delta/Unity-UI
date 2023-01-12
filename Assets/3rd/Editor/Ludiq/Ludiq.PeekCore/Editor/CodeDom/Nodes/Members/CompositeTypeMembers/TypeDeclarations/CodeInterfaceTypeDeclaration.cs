namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeInterfaceTypeDeclaration : CodeCompositeTypeDeclaration
	{
		public CodeInterfaceTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers, name)
		{
		}

		public override bool IsInterface => true;
		public override string Keyword => "interface";
	}
}
