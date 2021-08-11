// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeForeachStatement : CodeStatement
    {
        public CodeForeachStatement(CodeVariableDeclarationStatement declaration, CodeExpression expression, IEnumerable<CodeStatement> statements)
        {
			Debug.Assert(declaration.InitExpression == null, "foreach variable declaration cannot have initializer");
            Declaration = declaration;
            Expression = expression;
            Statements.AddRange(statements);
        }

		public CodeVariableDeclarationStatement Declaration { get; }
		public CodeExpression Expression { get; }
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Declaration != null) yield return Declaration;
				if (Expression != null) yield return Expression;
				foreach(var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineBeforeEnteringBlock();
			generator.EnterLocalScope();
			generator.Write(TokenType.Keyword, "foreach");
			generator.Write(TokenType.Space, ' ');
			generator.Write(TokenType.Punctuation, '(');
            Declaration.Type.Generate(generator);
			generator.Write(TokenType.Space, ' ');
			generator.OutputIdentifier(TokenType.Identifier, Declaration.Name);
			generator.Write(TokenType.Space, ' ');
			generator.Write(TokenType.Keyword, "in");
			generator.Write(TokenType.Space, ' ');
			Expression.Generate(generator);
			generator.Write(TokenType.Punctuation, ')');
			if (Statements.Count > 0)
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				generator.EnterLocalScope();
				Statements.ReserveLocals(generator, default(CodeStatementEmitOptions));
				Statements.Generate(generator, default(CodeStatementEmitOptions));
				generator.ExitLocalScope();
				generator.Indent--;
				generator.WriteClosingBrace();
			}
			else
			{
				generator.WriteEmptyBlock();
			}
			generator.ExitLocalScope();
		}
	}
}
