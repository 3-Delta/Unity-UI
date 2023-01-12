// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeYieldBreakStatement : CodeStatement
    {
        public CodeYieldBreakStatement() {}

		public override bool IsTerminator => true;

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Write(TokenType.Keyword, "yield");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Keyword, "break");
			generator.WriteStatementEnd(emitOptions);
		}
    }
}
