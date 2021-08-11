// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeForStatement : CodeStatement
    {
        public CodeForStatement(CodeStatement initStatement, CodeExpression testExpression, CodeExpression incrementExpression, IEnumerable<CodeStatement> statements)
        {
            InitStatement = initStatement;
            TestExpression = testExpression;
            IncrementExpression = incrementExpression;
            Statements.AddRange(statements);
        }

        public CodeStatement InitStatement { get; }
        public CodeExpression TestExpression { get; }
        public CodeExpression IncrementExpression { get; }
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (InitStatement != null) yield return InitStatement;
				if (TestExpression != null) yield return TestExpression;
				if (IncrementExpression != null) yield return IncrementExpression;
				foreach(var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
        {
			generator.WriteBlankLineBeforeEnteringBlock();
			generator.EnterLocalScope();
            generator.Write(TokenType.Keyword, "for");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Punctuation, '(');
            InitStatement.Generate(generator, CodeStatementEmitOptions.OmitSemiColon);
            generator.Write(TokenType.Punctuation, ';');
            generator.Write(TokenType.Space, ' ');
            TestExpression.Generate(generator);
            generator.Write(TokenType.Punctuation, ';');
            generator.Write(TokenType.Space, ' ');
            IncrementExpression.Generate(generator);
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
