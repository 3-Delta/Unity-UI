using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeUsingStatement : CodeStatement
    {
        public CodeUsingStatement(CodeStatement initStatement, IEnumerable<CodeStatement> statements)
        {
			InitStatement = initStatement;
            Statements.AddRange(statements);
        }

        public CodeStatement InitStatement { get; }
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (InitStatement != null) yield return InitStatement;
				foreach (var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineBeforeEnteringBlock();
			generator.EnterLocalScope();
            generator.Write(TokenType.Keyword, "using");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Punctuation, '(');
            InitStatement.Generate(generator, CodeStatementEmitOptions.OmitSemiColon);
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
