using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeWhileStatement : CodeStatement
	{
        public CodeWhileStatement(CodeExpression condition, IEnumerable<CodeStatement> statements)
        {
            Condition = condition;
            Statements.AddRange(statements);
        }

        public CodeExpression Condition { get; }
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Condition != null) yield return Condition;
				foreach (var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineBeforeEnteringBlock();
            generator.Write(TokenType.Keyword, "while");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Punctuation, '(');
            Condition.Generate(generator);
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
		}
	}
}
