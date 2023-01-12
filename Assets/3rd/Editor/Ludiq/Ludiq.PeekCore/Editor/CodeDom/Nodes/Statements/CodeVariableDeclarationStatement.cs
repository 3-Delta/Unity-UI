// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeVariableDeclarationStatement : CodeStatement
    {
        public CodeVariableDeclarationStatement(CodeTypeReference type, string name, CodeExpression initExpression = null)
        {
            Type = type;
            Name = name;
			InitExpression = initExpression;
        }

        public CodeTypeReference Type { get; }
        public string Name { get; } 
        public CodeExpression InitExpression { get; }

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Type != null) yield return Type;
				if (InitExpression != null) yield return InitExpression;
			}
		}

		public override void ReserveLocals(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.ReserveLocal(Name);
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			if (!generator.JustWroteOpeningBrace && !generator.JustWroteVariableDeclaration && (emitOptions & CodeStatementEmitOptions.OmitSemiColon) == 0)
			{
				generator.WriteLine();
			}
			generator.WriteBlankLineIfJustExitedBlock();

            Type.Generate(generator);
			generator.Write(TokenType.Space, ' ');
			generator.OutputIdentifier(TokenType.Identifier, Name);
            if (InitExpression != null)
            {
                generator.Write(TokenType.Space, ' ');
                generator.Write(TokenType.Punctuation, '=');
                generator.Write(TokenType.Space, ' ');
                InitExpression.Generate(generator);
            }
			generator.WriteStatementEnd(emitOptions);
			generator.JustWroteVariableDeclaration = true;
		}
	}
}
