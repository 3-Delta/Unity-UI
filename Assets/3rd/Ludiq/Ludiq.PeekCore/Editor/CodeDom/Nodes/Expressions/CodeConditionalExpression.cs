// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeConditionalExpression : CodeExpression
    {
        public CodeConditionalExpression(CodeExpression condition, CodeExpression trueExpression, CodeExpression falseExpression)
        {
			Ensure.That(nameof(condition)).IsNotNull(condition);
			Ensure.That(nameof(trueExpression)).IsNotNull(trueExpression);
			Ensure.That(nameof(falseExpression)).IsNotNull(falseExpression);

            Condition = condition;
			TrueExpression = trueExpression;
			FalseExpression = falseExpression;
        }

        public CodeExpression Condition { get; }
        public CodeExpression TrueExpression { get; }
        public CodeExpression FalseExpression { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Ternary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Condition != null) yield return Condition;
				if (TrueExpression != null) yield return TrueExpression;
				if (FalseExpression != null) yield return FalseExpression;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			if (Condition.Precedence > PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, '(');
			Condition.Generate(generator);
			if (Condition.Precedence > PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, ')');

			generator.Write(TokenType.Space, ' ');
			generator.Write(TokenType.Punctuation, '?');
			generator.Write(TokenType.Space, ' ');

			if (TrueExpression.Precedence >= PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, '(');
			TrueExpression.Generate(generator);
			if (TrueExpression.Precedence >= PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, ')');

			generator.Write(TokenType.Space, ' ');
			generator.Write(TokenType.Punctuation, ':');
			generator.Write(TokenType.Space, ' ');

			if (FalseExpression.Precedence >= PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, '(');
			FalseExpression.Generate(generator);
			if (FalseExpression.Precedence >= PrecedenceGroup.Ternary) generator.Write(TokenType.Punctuation, ')');
		}
	}
}
