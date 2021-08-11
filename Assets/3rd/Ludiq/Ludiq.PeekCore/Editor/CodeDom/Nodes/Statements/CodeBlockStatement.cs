using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeBlockStatement : CodeStatement
    {
        public CodeBlockStatement(IEnumerable<CodeStatement> statements)
        {
            Statements.AddRange(statements);
        }

        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

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
