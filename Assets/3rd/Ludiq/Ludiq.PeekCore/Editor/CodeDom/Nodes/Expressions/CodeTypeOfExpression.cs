// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeTypeOfExpression : CodeExpression
    {
        public CodeTypeOfExpression(CodeTypeReference type)
        {
			Ensure.That(nameof(type)).IsNotNull(type);

            Type = type;
        }

        public CodeTypeReference Type { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Type != null) yield return Type;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Keyword, "typeof");
            generator.Write(TokenType.Punctuation, '(');
            Type.Generate(generator);
            generator.Write(TokenType.Punctuation, ')');
        }
	}
}
