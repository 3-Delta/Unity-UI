using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public class CodeLambdaExpression : CodeExpression
	{
		public CodeLambdaExpression(IEnumerable<CodeParameterDeclaration> parameters, CodeExpression expressionBody)
		{
			Ensure.That(nameof(parameters)).IsNotNull(parameters);
			Ensure.That(nameof(expressionBody)).IsNotNull(expressionBody);

			Parameters = new List<CodeParameterDeclaration>(parameters);
			ExpressionBody = expressionBody;
		}

		public CodeLambdaExpression(IEnumerable<CodeParameterDeclaration> parameters, IEnumerable<CodeStatement> statements)
		{
			Ensure.That(nameof(parameters)).IsNotNull(parameters);
			Ensure.That(nameof(statements)).IsNotNull(statements);

			Parameters = new List<CodeParameterDeclaration>(parameters);
			Statements.AddRange(statements);
		}

		List<CodeParameterDeclaration> Parameters { get; } = new List<CodeParameterDeclaration>();
		CodeExpression ExpressionBody { get; }
		List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Assignment;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Parameters) yield return child;
				if (ExpressionBody != null) yield return ExpressionBody;
				foreach (var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			bool implicitlyTyped = true;

			foreach (var parameter in Parameters)
			{
				if (parameter.Type.RawName != "var")
				{
					implicitlyTyped = false;
				}
			}

			generator.EnterLocalScope();

			if (implicitlyTyped)
			{
				if (Parameters.Count == 1)
				{
					generator.OutputIdentifier(TokenType.Identifier, Parameters[0].Name);
				}
				else
				{
					generator.Write(TokenType.Punctuation, '(');
					bool first = true;
					foreach (var parameter in Parameters)
					{
						if (first)
						{
							first = false;
						}
						else
						{
							generator.Write(TokenType.Punctuation, ',');
							generator.Write(TokenType.Space, ' ');
						}

						generator.OutputIdentifier(TokenType.Identifier, parameter.Name);
					}
					generator.Write(TokenType.Punctuation, ')');
				}
			}
			else
			{
				generator.Write(TokenType.Punctuation, '(');
				Parameters.GenerateCommaSeparated(generator);
				generator.Write(TokenType.Punctuation, ')');
			}

			generator.Write(TokenType.Space, ' ');
			generator.Write(TokenType.Operator, "=>");
			generator.Write(TokenType.Space, ' ');

			if (ExpressionBody != null)
			{
				ExpressionBody.Generate(generator);				
			}
			else
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				Statements.Generate(generator, default(CodeStatementEmitOptions));
				generator.Indent--;
				generator.WriteClosingBrace(true);
			}

			generator.ExitLocalScope();
		}
	}
}
