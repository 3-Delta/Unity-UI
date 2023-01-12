// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeYieldReturnStatement : CodeStatement
    {
        public CodeYieldReturnStatement(CodeExpression expression)
        {
            Expression = expression;
        }

        public CodeExpression Expression { get; }

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children => Expression.Yield<CodeElement>();

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Write(TokenType.Keyword, "yield");
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Keyword, "return");
			generator.Write(TokenType.Space, ' ');
            Expression.Generate(generator);
			generator.WriteStatementEnd(emitOptions);
		}
    }
}
