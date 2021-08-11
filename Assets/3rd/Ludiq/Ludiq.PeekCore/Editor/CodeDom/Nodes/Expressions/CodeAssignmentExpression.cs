// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeAssignmentExpression : CodeExpression
    {
        public CodeAssignmentExpression() {}

        public CodeAssignmentExpression(CodeExpression left, CodeExpression right)
        {
			Ensure.That(nameof(left)).IsNotNull(left);
			Ensure.That(nameof(right)).IsNotNull(right);

            Left = left;
            Right = right;
        }

        public CodeExpression Left { get; }
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
            generator.Write(TokenType.Punctuation, '=');
            generator.Write(TokenType.Space, ' ');
            Right.Generate(generator);
		}
	}
}
