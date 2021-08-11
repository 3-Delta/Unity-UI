using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeUserPropertyAccessor : CodeBasicPropertyAccessor
	{
		public CodeUserPropertyAccessor(CodeMemberModifiers modifiers, IEnumerable<CodeStatement> statements)
			: base(modifiers)
		{
			Statements.AddRange(statements);
		}

        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, string name, CodeBasicPropertyMember property, CodeCompositeTypeDeclaration enclosingType)
		{
			generator.Write(TokenType.Keyword, name);

			generator.EnterLocalScope();

			generator.IsInSetterProperty = name == "set";
			if (generator.IsInSetterProperty)
			{
				generator.ReserveLocal("value");
			}
			if (property is CodeIndexerMember indexer)
			{
				foreach (var parameter in indexer.Parameters)
				{
					generator.ReserveLocal(parameter.Name);
				}
			}

			generator.ExitLocalScope();

			if (Statements.Count == 1 && Statements[0] is CodeReturnStatement returnStatement && returnStatement.Expression != null)
			{
				generator.Write(TokenType.Space, ' ');
				generator.Write(TokenType.Operator, "=>");
				generator.Write(TokenType.Space, ' ');
				returnStatement.Expression.Generate(generator);
				generator.WriteLine(TokenType.Punctuation, ";");
			}
			else if (Statements.Count > 0)
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				Statements.Generate(generator, default(CodeStatementEmitOptions));
				generator.Indent--;
				generator.WriteClosingBrace();
				generator.IsInSetterProperty = false;
			}
			else
			{
				generator.WriteEmptyBlock();
			}
		}
	}
}
