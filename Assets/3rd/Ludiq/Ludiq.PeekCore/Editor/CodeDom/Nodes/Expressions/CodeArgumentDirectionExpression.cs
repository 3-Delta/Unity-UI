// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeArgumentDirectionExpression : CodeExpression
    {
        public CodeArgumentDirectionExpression(CodeParameterDirection direction, CodeExpression expression)
        {
			Ensure.That(nameof(expression)).IsNotNull(expression);

            Direction = direction;
            Expression = expression;
        }

        public CodeParameterDirection Direction { get; }
        public CodeExpression Expression { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Expression != null) yield return Expression;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            Direction.Generate(generator);
            Expression.Generate(generator);
		}
	}
}
