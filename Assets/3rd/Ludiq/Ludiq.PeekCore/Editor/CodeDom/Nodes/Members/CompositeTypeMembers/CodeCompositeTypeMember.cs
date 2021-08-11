using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeCompositeTypeMember : CodeElement
	{
		public enum MemberCategory
		{
			Type,

			StaticField,
			StaticConstructor,
			StaticIndexer,
			StaticProperty,
			StaticMethod,

			Field,
			Constructor,
			Indexer,
			Property,
			Method,
		}

		public static bool ShouldLineSeparateIndividualMember(MemberCategory category)
		{
			switch (category)
			{
				case MemberCategory.Field:
					return false;
				default:
					return true;
			}
		}

		public CodeCompositeTypeMember(CodeMemberModifiers modifiers)
		{
			Modifiers = modifiers;
		}

		public CodeMemberModifiers Modifiers { get; }
        public List<CodeComment> Comments { get; } = new List<CodeComment>();
        public List<CodeDirective> StartDirectives { get; } = new List<CodeDirective>();
        public List<CodeDirective> EndDirectives { get; } = new List<CodeDirective>();
		public List<CodeAttributeDeclaration> CustomAttributes { get; } = new List<CodeAttributeDeclaration>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Comments) yield return child;
				foreach (var child in StartDirectives) yield return child;
				foreach (var child in EndDirectives) yield return child;
			}
		}

		public abstract MemberCategory Category { get; }

		public void Generate(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
			generator.EnterElement(this);

			StartDirectives.Generate(generator);
			Comments.Generate(generator);
            generator.GenerateAttributes(CustomAttributes);
			GenerateInner(generator, enclosingType);
			EndDirectives.Generate(generator);

			generator.ExitElement();
		}

		protected abstract void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType);
	}
}
