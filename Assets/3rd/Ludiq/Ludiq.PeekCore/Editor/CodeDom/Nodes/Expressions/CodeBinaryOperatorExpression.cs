// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeBinaryOperatorExpression : CodeExpression
    {
        public CodeBinaryOperatorExpression(CodeExpression left, CodeBinaryOperatorType op, CodeExpression right)
        {
			Ensure.That(nameof(left)).IsNotNull(left);
			Ensure.That(nameof(right)).IsNotNull(right);

            Left = left;
            Operator = op;
            Right = right;
        }

        public CodeExpression Left { get; }
        public CodeBinaryOperatorType Operator { get; }
        public CodeExpression Right { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Binary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Left != null) yield return Left;
				if (Right != null) yield return Right;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			var thisBinaryPrecedence = CodeGeneratorUtility.BinaryOperatorPrecedences[Operator];

			bool leftParenthesized;
			if (Left is CodeBinaryOperatorExpression leftBinary)
			{
				var leftBinaryPrecedence = CodeGeneratorUtility.BinaryOperatorPrecedences[leftBinary.Operator];
				leftParenthesized = leftBinaryPrecedence > thisBinaryPrecedence || (leftBinaryPrecedence < thisBinaryPrecedence && leftBinaryPrecedence.ShouldParenthesizeWhenMixed());
			}
			else
			{
				leftParenthesized = Left.Precedence > PrecedenceGroup.Binary;
			}

			if (leftParenthesized) generator.Write(TokenType.Punctuation, "(");
			Left.Generate(generator);
			if (leftParenthesized) generator.Write(TokenType.Punctuation, ")");
            generator.Write(TokenType.Space, ' ');

            switch (Operator)
            {
                case CodeBinaryOperatorType.Add: generator.Write(TokenType.Operator, '+'); break;
                case CodeBinaryOperatorType.Subtract: generator.Write(TokenType.Operator, '-'); break;
                case CodeBinaryOperatorType.Multiply: generator.Write(TokenType.Operator, '*'); break;
                case CodeBinaryOperatorType.Divide: generator.Write(TokenType.Operator, '/'); break;
                case CodeBinaryOperatorType.Modulo: generator.Write(TokenType.Operator, '%'); break;
                case CodeBinaryOperatorType.Equality: generator.Write(TokenType.Operator, "=="); break;
                case CodeBinaryOperatorType.Inequality: generator.Write(TokenType.Operator, "!="); break;
                case CodeBinaryOperatorType.BitwiseOr: generator.Write(TokenType.Operator, '|'); break;
                case CodeBinaryOperatorType.BitwiseAnd: generator.Write(TokenType.Operator, '&'); break;
                case CodeBinaryOperatorType.BitwiseXor: generator.Write(TokenType.Operator, '^'); break;
                case CodeBinaryOperatorType.LogicalOr: generator.Write(TokenType.Operator, "||"); break;
                case CodeBinaryOperatorType.LogicalAnd: generator.Write(TokenType.Operator, "&&"); break;
                case CodeBinaryOperatorType.LessThan: generator.Write(TokenType.Operator, '<'); break;
                case CodeBinaryOperatorType.LessThanOrEqual: generator.Write(TokenType.Operator, "<="); break;
                case CodeBinaryOperatorType.GreaterThan: generator.Write(TokenType.Operator, '>'); break;
                case CodeBinaryOperatorType.GreaterThanOrEqual: generator.Write(TokenType.Operator, ">="); break;
                case CodeBinaryOperatorType.BitwiseShiftLeft: generator.Write(TokenType.Operator, "<<"); break;
                case CodeBinaryOperatorType.BitwiseShiftRight: generator.Write(TokenType.Operator, ">>"); break;
                case CodeBinaryOperatorType.Is: generator.Write(TokenType.Keyword, "is"); break;
                case CodeBinaryOperatorType.As: generator.Write(TokenType.Keyword, "as"); break;
                case CodeBinaryOperatorType.NullCoalesce: generator.Write(TokenType.Operator, "??"); break;
            }

			bool rightParenthesized;
			if (Right is CodeBinaryOperatorExpression rightBinary)
			{
				var rightBinaryPrecedence = CodeGeneratorUtility.BinaryOperatorPrecedences[rightBinary.Operator];
				rightParenthesized = rightBinaryPrecedence > thisBinaryPrecedence || (rightBinaryPrecedence < thisBinaryPrecedence && rightBinaryPrecedence.ShouldParenthesizeWhenMixed());
			}
			else
			{
				rightParenthesized = Right.Precedence > PrecedenceGroup.Binary;
			}

            generator.Write(TokenType.Space, ' ');
			if (rightParenthesized) generator.Write(TokenType.Punctuation, "(");
			Right.Generate(generator);
			if (rightParenthesized) generator.Write(TokenType.Punctuation, ")");
		}
	}
}
