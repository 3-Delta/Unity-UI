using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeBasicPropertyAccessor : CodeElement
	{
		public CodeBasicPropertyAccessor(CodeMemberModifiers modifiers)
		{
			Modifiers = modifiers;
		}

		public CodeMemberModifiers Modifiers { get; }
        public List<CodeComment> Comments { get; } = new List<CodeComment>();
        public List<CodeDirective> StartDirectives { get; } = new List<CodeDirective>();
        public List<CodeDirective> EndDirectives { get; } = new List<CodeDirective>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Comments) yield return child;
				foreach(var child in StartDirectives) yield return child;
				foreach(var child in EndDirectives) yield return child;
			}
		}

		public void Generate(CodeGenerator generator, string name, CodeBasicPropertyMember property, CodeCompositeTypeDeclaration enclosingType)
		{
			generator.EnterElement(this);

			StartDirectives.Generate(generator);
			Comments.Generate(generator);
			Modifiers.Generate(generator);
			GenerateInner(generator, name, property, enclosingType);
			EndDirectives.Generate(generator);

			generator.ExitElement();
		}

		protected abstract void GenerateInner(CodeGenerator generator, string name, CodeBasicPropertyMember property, CodeCompositeTypeDeclaration enclosingType);
	}
}
