using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeDefaultStatement : CodeStatement
    {
        public CodeDefaultStatement(bool fallthrough)
		{
			Fallthrough = fallthrough;
		}

        public CodeDefaultStatement(IEnumerable<CodeStatement> statements)
		{
			Statements.AddRange(statements);
			Fallthrough = false;
		}

        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();
		public bool Fallthrough { get; }

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Statements) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
        {
            generator.Indent--;
            generator.Write(TokenType.Keyword, "default");
            generator.WriteLine(TokenType.Punctuation, ':');
            generator.Indent++;

			if (!Fallthrough)
			{
				generator.EnterLocalScope();
				Statements.ReserveLocals(generator, default(CodeStatementEmitOptions));
			}

            Statements.Generate(generator, default(CodeStatementEmitOptions));

            if (!Fallthrough)
			{
				bool needsBreak = true;
				if (Statements.Count > 0)
				{
					var lastStatement = Statements[Statements.Count - 1];
					needsBreak = !lastStatement.IsTerminator;
				}

				if (needsBreak)
				{
					generator.Write(TokenType.Keyword, "break");
					generator.WriteLine(TokenType.Punctuation, ';');
				}
				generator.ExitLocalScope();
			}
        }
	}
}