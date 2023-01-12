// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeArrayCreateExpression : CodeExpression
    {
        public CodeArrayCreateExpression(CodeTypeReference type, IEnumerable<CodeExpression> lengths, IEnumerable<CodeExpression> initializer)
        {
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(lengths)).IsNotNull(lengths);
			Ensure.That(nameof(initializer)).IsNotNull(initializer);

			CreateType = type;
			Lengths.AddRange(lengths);
			Initializer.AddRange(initializer);
        }

        public CodeTypeReference CreateType { get; }
        public List<CodeExpression> Lengths { get; } = new List<CodeExpression>();
        public List<CodeExpression> Initializer { get; } = new List<CodeExpression>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (CreateType != null) yield return CreateType;
				foreach (var child in Lengths) yield return child;
				foreach (var child in Initializer) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Keyword, "new");
            generator.Write(TokenType.Space, ' ');

			if (Lengths.Count > 0)
			{
                CreateType.NestedElementType.Generate(generator);
                generator.Write(TokenType.Punctuation, '[');
                Lengths.GenerateCommaSeparated(generator);
                generator.Write(TokenType.Punctuation, ']');
				generator.Write(TokenType.Punctuation, CreateType.ArrayElementType.CompleteArraySuffix);
			}
			else
			{
                CreateType.Generate(generator);
			}

            if (Initializer.Count > 0 || Lengths.Count == 0)
            {
				generator.Write(TokenType.Punctuation, '{');
				Initializer.GenerateCommaSeparated(generator);
				generator.Write(TokenType.Punctuation, '}');
            }
		}
	}
}
