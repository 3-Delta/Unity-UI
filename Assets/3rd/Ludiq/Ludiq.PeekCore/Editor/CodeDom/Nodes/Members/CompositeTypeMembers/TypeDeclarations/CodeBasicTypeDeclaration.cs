namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeBasicTypeDeclaration : CodeCompositeTypeMember
	{
		public CodeBasicTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers)
		{
			Name = name;
		}

        public string Name { get; }

		public override MemberCategory Category => MemberCategory.Type;

		public void Generate(CodeGenerator generator)
		{
			Generate(generator, null);
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
			GenerateInner(generator);
		}

		protected abstract void GenerateInner(CodeGenerator generator);		
	}
}
