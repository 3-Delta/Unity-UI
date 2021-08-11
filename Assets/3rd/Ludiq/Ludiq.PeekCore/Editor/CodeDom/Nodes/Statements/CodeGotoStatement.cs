// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeGotoStatement : CodeStatement
    {
        public CodeGotoStatement(string label)
        {
            Label = label;
        }

        public string Label { get; }

		public override bool IsTerminator => true;

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Write(TokenType.Keyword, "goto");
            generator.Write(TokenType.Space, ' ');
            generator.OutputIdentifier(TokenType.Identifier, Label);
			generator.WriteStatementEnd(emitOptions);
		}
	}
}
