using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeEnumTypeDeclaration : CodeBasicTypeDeclaration
	{
		public CodeEnumTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers, name)
		{
		}

		public CodeEnumTypeDeclaration(CodeMemberModifiers modifiers, string name, CodeTypeReference underlyingType)
			: this(modifiers, name)
		{
			UnderlyingType = underlyingType;
		}

        public CodeTypeReference UnderlyingType { get; set; }
        public List<CodeEnumMember> Members { get; } = new List<CodeEnumMember>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (UnderlyingType != null) yield return UnderlyingType;
				foreach(var child in Members) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			Modifiers.Generate(generator);

            generator.Write(TokenType.Keyword, "enum");
            generator.Write(TokenType.Space, ' ');
            generator.OutputIdentifier(TokenType.TypeIdentifier, Name);

			if (UnderlyingType != null)
			{
                generator.Write(TokenType.Space, ' ');
                generator.Write(TokenType.Punctuation, ':');
                generator.Write(TokenType.Space, ' ');
                UnderlyingType.Generate(generator);
			}

			if (Members.Count > 0)
			{
				generator.WriteOpeningBrace();
				generator.Indent++;

				foreach (var member in Members)
				{
					member.Generate(generator);
				}

				generator.Indent--;
				generator.WriteClosingBrace();
			}
			else
			{
				generator.WriteEmptyBlock();
			}
		}
	}
}
