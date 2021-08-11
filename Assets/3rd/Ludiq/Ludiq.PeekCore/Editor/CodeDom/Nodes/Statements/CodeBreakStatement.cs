namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeBreakStatement : CodeStatement
    {
        public CodeBreakStatement() {}

		public override bool IsTerminator => true;

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
        {
			generator.WriteBlankLineIfJustExitedBlock();
            generator.WriteLine(TokenType.Keyword, "break");
			generator.WriteStatementEnd(emitOptions);
        }
	}
}
