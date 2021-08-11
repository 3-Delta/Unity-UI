// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeThrowStatement : CodeStatement
    {
        public CodeThrowStatement(CodeExpression toThrow)
        {
            ToThrow = toThrow;
        }

        public CodeExpression ToThrow { get; }

		public override bool IsTerminator => true;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (ToThrow != null) yield return ToThrow;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Write(TokenType.Keyword, "throw");
            if (ToThrow != null)
            {
                generator.Write(TokenType.Space, ' ');
                ToThrow.Generate(generator);
            }
			generator.WriteStatementEnd(emitOptions);
		}
    }
}
