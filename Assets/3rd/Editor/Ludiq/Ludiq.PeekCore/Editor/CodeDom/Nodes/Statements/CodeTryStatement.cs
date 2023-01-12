// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeTryStatement : CodeStatement
    {
        public CodeTryStatement(IEnumerable<CodeStatement> tryStatements, IEnumerable<CodeCatchClause> catchClauses)
        {
            TryStatements.AddRange(tryStatements);
            CatchClauses.AddRange(catchClauses);
        }

        public CodeTryStatement(IEnumerable<CodeStatement> tryStatements, IEnumerable<CodeCatchClause> catchClauses, IEnumerable<CodeStatement> finallyStatements)
        {
            TryStatements.AddRange(tryStatements);
            CatchClauses.AddRange(catchClauses);
            FinallyStatements.AddRange(finallyStatements);
        }

        public List<CodeStatement> TryStatements { get; } = new List<CodeStatement>();
        public List<CodeCatchClause> CatchClauses { get; } = new List<CodeCatchClause>();
        public List<CodeStatement> FinallyStatements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in TryStatements) yield return child;
				foreach (var child in CatchClauses) yield return child;
				foreach (var child in FinallyStatements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineBeforeEnteringBlock();
            generator.Write(TokenType.Keyword, "try");
            generator.WriteOpeningBrace();
            generator.Indent++;
			generator.EnterLocalScope();
            TryStatements.ReserveLocals(generator, default(CodeStatementEmitOptions));
            TryStatements.Generate(generator, default(CodeStatementEmitOptions));
			generator.ExitLocalScope();
            generator.Indent--;

            foreach (var catchClause in CatchClauses)
            {
				generator.WriteMiddleClosingBrace();
                generator.Write(TokenType.Keyword, "catch");
                generator.Write(TokenType.Space, ' ');
                generator.Write(TokenType.Punctuation, '(');
                catchClause.CatchExceptionType.Generate(generator);
                generator.Write(TokenType.Space, ' ');
                generator.OutputIdentifier(TokenType.Identifier, catchClause.LocalName);
                generator.Write(TokenType.Punctuation, ')');
                generator.WriteOpeningBrace();
                generator.Indent++;
				generator.EnterLocalScope();
				generator.ReserveLocal(catchClause.LocalName);
				catchClause.Statements.ReserveLocals(generator, default(CodeStatementEmitOptions));
                catchClause.Statements.Generate(generator, default(CodeStatementEmitOptions));
				generator.ExitLocalScope();
                generator.Indent--;
            }

            if (FinallyStatements.Count > 0)
            {
				generator.WriteMiddleClosingBrace();
                generator.Write(TokenType.Keyword, "finally");
                generator.WriteOpeningBrace();
                generator.Indent++;
				generator.EnterLocalScope();
				FinallyStatements.ReserveLocals(generator, default(CodeStatementEmitOptions));
                FinallyStatements.Generate(generator, default(CodeStatementEmitOptions));
				generator.ExitLocalScope();
                generator.Indent--;
            }
            generator.WriteClosingBrace();
		}
	}
}
