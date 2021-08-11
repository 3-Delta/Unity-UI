using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeCompositeTypeDeclaration : CodeBasicTypeDeclaration
	{
		public CodeCompositeTypeDeclaration(CodeMemberModifiers modifiers, string name)
			: base(modifiers, name)
		{
		}

		public abstract string Keyword { get; }
		public abstract bool IsInterface { get; }

        public bool IsPartial { get; set; }
        public List<CodeTypeReference> BaseTypes { get; } = new List<CodeTypeReference>();
        public List<CodeTypeParameter> TypeParameters { get; } = new List<CodeTypeParameter>();
        public List<CodeCompositeTypeMember> Members { get; } = new List<CodeCompositeTypeMember>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in BaseTypes) yield return child;
				foreach(var child in TypeParameters) yield return child;
				foreach(var child in Members) yield return child;
			}
		}

		private List<CodeCompositeTypeMember> GetMembersSortedByCategory()
		{
			return Members.OrderBy(x => x.Category).ToList();
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			Modifiers.Generate(generator);

			if (IsPartial)
			{
                generator.Write(TokenType.Keyword, "partial");
                generator.Write(TokenType.Space, ' ');
			}

			generator.Write(TokenType.Keyword, Keyword);
            generator.Write(TokenType.Space, ' ');
            generator.OutputIdentifier(TokenType.TypeIdentifier, Name);

            generator.OutputTypeParameters(TypeParameters);

            var firstTypeRef = true;
            foreach (CodeTypeReference typeRef in BaseTypes)
            {
                if (firstTypeRef)
                {
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.Punctuation, ':');
                    generator.Write(TokenType.Space, ' ');
                    firstTypeRef = false;
                }
                else
                {
                    generator.Write(TokenType.Punctuation, ',');
                    generator.Write(TokenType.Space, ' ');
                }
                typeRef.Generate(generator);
            }

            foreach (var typeParameter in TypeParameters)
            {
                typeParameter.GenerateConstraints(generator);
            }

			if (Members.Count > 0)
			{
				generator.WriteOpeningBrace();
				generator.Indent++;

				CodeCompositeTypeMember previousMember = null;
				var sortedMembers = GetMembersSortedByCategory();

				foreach (var member in sortedMembers)
				{
					if (previousMember != null && (previousMember.Category != member.Category || ShouldLineSeparateIndividualMember(previousMember.Category)))
					{
						generator.WriteLine();
					}

					member.Generate(generator, this);
					previousMember = member;
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
