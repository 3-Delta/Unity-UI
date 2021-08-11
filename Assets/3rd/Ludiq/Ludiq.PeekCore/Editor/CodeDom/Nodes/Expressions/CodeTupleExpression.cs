// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeTupleExpression : CodeExpression
    {
        public CodeTupleExpression(IEnumerable<CodeExpression> items)
        {
			Ensure.That(nameof(items)).IsNotNull(items);

            Items.AddRange(items);
        }

        public List<CodeExpression> Items { get; } = new List<CodeExpression>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Items) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Punctuation, '(');
            Items.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');
		}
	}
}
