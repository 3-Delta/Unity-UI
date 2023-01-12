// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeIfStatement : CodeStatement
    {
        public CodeIfStatement(CodeExpression condition, IEnumerable<CodeStatement> trueStatements)
        {
            Condition = condition;
            TrueStatements.AddRange(trueStatements);
        }

        public CodeIfStatement(CodeExpression condition, IEnumerable<CodeStatement> trueStatements, IEnumerable<CodeStatement> falseStatements)
        {
            Condition = condition;
            TrueStatements.AddRange(trueStatements);
            FalseStatements.AddRange(falseStatements);
        }

        public CodeExpression Condition { get; }
        public List<CodeStatement> TrueStatements { get; } = new List<CodeStatement>();
        public List<CodeStatement> FalseStatements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Condition != null) yield return Condition;
				foreach(var child in TrueStatements) yield return child;
				foreach(var child in FalseStatements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineBeforeEnteringBlock();
            generator.Write(TokenType.Keyword, "if");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Punctuation, '(');
            Condition.Generate(generator);
            generator.Write(TokenType.Punctuation, ')');
			if (TrueStatements.Count == 0 && FalseStatements.Count == 0)
			{
				generator.WriteEmptyBlock();
			}
			else
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				generator.EnterLocalScope();
				TrueStatements.ReserveLocals(generator, default(CodeStatementEmitOptions));
				TrueStatements.Generate(generator, default(CodeStatementEmitOptions));
				generator.ExitLocalScope();
				generator.Indent--;

				if (FalseStatements.Count > 0)
				{
					generator.WriteMiddleClosingBrace();
					generator.Write(TokenType.Keyword, "else");
					generator.WriteOpeningBrace();
					generator.Indent++;
					generator.EnterLocalScope();
					FalseStatements.ReserveLocals(generator, default(CodeStatementEmitOptions));
					FalseStatements.Generate(generator, default(CodeStatementEmitOptions));
					generator.ExitLocalScope();
					generator.Indent--;
				}
				generator.WriteClosingBrace();
			}
		}
	}
}
