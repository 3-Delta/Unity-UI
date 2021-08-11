namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeContinueStatement : CodeStatement
    {
        public CodeContinueStatement() {}

		public override bool IsTerminator => true;

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
        {
			generator.WriteBlankLineIfJustExitedBlock();
            generator.WriteLine(TokenType.Keyword, "continue");
			generator.WriteStatementEnd(emitOptions);
        }
	}
}