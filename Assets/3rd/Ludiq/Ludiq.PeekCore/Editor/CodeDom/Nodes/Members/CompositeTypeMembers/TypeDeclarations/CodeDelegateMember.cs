using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeDelegateTypeDeclaration : CodeBasicTypeDeclaration
    {
        public CodeDelegateTypeDeclaration(CodeMemberModifiers modifiers, CodeTypeReference returnType, string name, IEnumerable<CodeParameterDeclaration> parameters)
			: base(modifiers, name)
		{
			ReturnType = returnType;
			Parameters.AddRange(parameters);
		}

        public CodeTypeReference ReturnType { get; }
        public List<CodeParameterDeclaration> Parameters { get; } = new List<CodeParameterDeclaration>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (ReturnType != null) yield return ReturnType;
				foreach(var child in Parameters) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			generator.Write(TokenType.Keyword, "delegate");
            generator.Write(TokenType.Space, ' ');
			ReturnType.Generate(generator);
			generator.Write(TokenType.Space, ' ');
			generator.OutputIdentifier(TokenType.TypeIdentifier, Name);
			generator.Write(TokenType.Punctuation, '(');
			Parameters.GenerateCommaSeparated(generator);
			generator.WriteLine(TokenType.Punctuation, ");");
		}
	}
}
