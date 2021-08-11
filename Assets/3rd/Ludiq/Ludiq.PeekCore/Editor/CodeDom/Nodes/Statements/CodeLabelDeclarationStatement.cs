// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeLabelDeclarationStatement : CodeStatement
    {
        public CodeLabelDeclarationStatement(string label, CodeStatement statement)
        {
            Label = label;
        }

        public string Label { get; }

		public override bool IsTerminator => false;

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            generator.Indent--;
            generator.OutputIdentifier(TokenType.Identifier, Label);
			generator.Write(TokenType.Punctuation, ':');
            generator.Indent++;
		}
	}
}
