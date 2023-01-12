// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeUnaryOperatorExpression : CodeExpression
    {
        public CodeUnaryOperatorExpression(CodeUnaryOperatorType op, CodeExpression operand)
        {
			Ensure.That(nameof(operand)).IsNotNull(operand);

            Operator = op;
            Operand = operand;
        }

        public CodeUnaryOperatorType Operator { get; }
        public CodeExpression Operand { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Unary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Operand != null) yield return Operand;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            switch (Operator)
			{                    
				case CodeUnaryOperatorType.Positive: generator.Write(TokenType.Operator, '+'); break;
				case CodeUnaryOperatorType.Negative: generator.Write(TokenType.Operator, '-'); break;
				case CodeUnaryOperatorType.LogicalNot: generator.Write(TokenType.Operator, '!'); break;
				case CodeUnaryOperatorType.BitwiseNot: generator.Write(TokenType.Operator, '~'); break;
				case CodeUnaryOperatorType.PreIncrement: generator.Write(TokenType.Operator, "++"); break;
				case CodeUnaryOperatorType.PreDecrement: generator.Write(TokenType.Operator, "--"); break;
				case CodeUnaryOperatorType.AddressOf: generator.Write(TokenType.Operator, '&'); break;
				case CodeUnaryOperatorType.Dereference: generator.Write(TokenType.Operator, '*'); break;
			}
			
			bool parenthesized = Operand.Precedence >= PrecedenceGroup.Unary;
			if (parenthesized) generator.Write(TokenType.Punctuation, '(');
            Operand.Generate(generator);
			if (parenthesized) generator.Write(TokenType.Punctuation, ')');

			switch(Operator)
			{
				case CodeUnaryOperatorType.PostIncrement: generator.Write(TokenType.Operator, "++"); break;
				case CodeUnaryOperatorType.PostDecrement: generator.Write(TokenType.Operator, "--"); break;
			}
		}
	}
}
