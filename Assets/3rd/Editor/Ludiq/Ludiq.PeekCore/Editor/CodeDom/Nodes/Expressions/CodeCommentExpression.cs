// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeCommentExpression : CodeExpression
    {
        public CodeCommentExpression(CodeComment comment, CodeExpression expression = null)
		{
			Comment = comment;
			Expression = Expression;
		}

        public CodeComment Comment { get; }
        public CodeCommentExpression Expression { get; }

		public override PrecedenceGroup Precedence => Expression != null ? Expression.Precedence : PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Comment != null) yield return Comment;
				if (Expression != null) yield return Expression;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			Comment.Generate(generator);
			if (Expression != null)
			{
				generator.Write(TokenType.Space, ' ');
				Expression.Generate(generator);
			}
		}
	}
}
