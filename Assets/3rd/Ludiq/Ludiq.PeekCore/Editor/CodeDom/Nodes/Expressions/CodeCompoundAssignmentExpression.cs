// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeCompoundAssignmentExpression : CodeExpression
    {
        public CodeCompoundAssignmentExpression(CodeExpression left, CodeCompoundAssignmentOperatorType op, CodeExpression right)
        {
			Ensure.That(nameof(left)).IsNotNull(left);
			Ensure.That(nameof(right)).IsNotNull(right);

            Left = left;
			Operator = op;
            Right = right;
        }

        public CodeExpression Left { get; }
		public CodeCompoundAssignmentOperatorType Operator { get; set; }
        public CodeExpression Right { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Assignment;

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
			Left.Generate(generator);
			
			generator.Write(TokenType.Space, ' ');

            switch (Operator)
            {
                case CodeCompoundAssignmentOperatorType.Add: generator.Write(TokenType.Operator, "+="); break;
                case CodeCompoundAssignmentOperatorType.Subtract: generator.Write(TokenType.Operator, "-="); break;
                case CodeCompoundAssignmentOperatorType.Multiply: generator.Write(TokenType.Operator, "*="); break;
                case CodeCompoundAssignmentOperatorType.Divide: generator.Write(TokenType.Operator, "/="); break;
                case CodeCompoundAssignmentOperatorType.Modulo: generator.Write(TokenType.Operator, "%="); break;
                case CodeCompoundAssignmentOperatorType.BitwiseOr: generator.Write(TokenType.Operator, "|="); break;
                case CodeCompoundAssignmentOperatorType.BitwiseAnd: generator.Write(TokenType.Operator, "&="); break;
                case CodeCompoundAssignmentOperatorType.BitwiseXor: generator.Write(TokenType.Operator, "^="); break;
                case CodeCompoundAssignmentOperatorType.BitwiseShiftLeft: generator.Write(TokenType.Operator, "<<="); break;
                case CodeCompoundAssignmentOperatorType.BitwiseShiftRight: generator.Write(TokenType.Operator, ">>="); break;
            }

            generator.Write(TokenType.Space, ' ');

            Right.Generate(generator);
		}
	}
}
