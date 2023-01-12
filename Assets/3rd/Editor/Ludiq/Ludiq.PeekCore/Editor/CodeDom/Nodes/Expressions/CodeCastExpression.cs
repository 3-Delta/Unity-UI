// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeCastExpression : CodeExpression
    {
        public CodeCastExpression(CodeTypeReference targetType, CodeExpression expression)
        {
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(expression)).IsNotNull(expression);

            TargetType = targetType;
            Expression = expression;
        }

        public CodeTypeReference TargetType { get; }
        public CodeExpression Expression { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Unary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (TargetType != null) yield return TargetType;
				if (Expression != null) yield return Expression;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Punctuation, '(');
            TargetType.Generate(generator);
            generator.Write(TokenType.Punctuation, ')');

			bool parenthesized = Expression.Precedence > PrecedenceGroup.Unary;

			// CS0075: To cast a negative value, you must enclose the value in parentheses
			if (!parenthesized && Expression is CodePrimitiveExpression primitive && primitive.Value != null && primitive.Value.GetType().IsNumeric())
			{
				if (Convert.ToDouble(primitive.Value) < 0)
				{
					parenthesized = true;
				}
			}
			
			if (parenthesized) generator.Write(TokenType.Punctuation, '(');
            Expression.Generate(generator);
			if (parenthesized) generator.Write(TokenType.Punctuation, ')');
		}
	}
}
