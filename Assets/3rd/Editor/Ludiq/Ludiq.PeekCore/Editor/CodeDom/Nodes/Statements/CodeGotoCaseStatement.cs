using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeGotoCaseStatement : CodeStatement
    {
        public CodeGotoCaseStatement(CodeExpression value) {}

        public CodeExpression Value { get; }

		public override bool IsTerminator => true;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Value != null) yield return Value;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Write(TokenType.Keyword, "goto");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Keyword, "case");
            generator.Write(TokenType.Space, ' ');
            Value.Generate(generator);
			generator.WriteStatementEnd(emitOptions);
		}
	}
}