// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeNamedArgumentExpression : CodeExpression
    {
        public CodeNamedArgumentExpression(string argumentName, CodeExpression value)
        {
			Ensure.That(nameof(argumentName)).IsNotNullOrEmpty(argumentName);
			Ensure.That(nameof(value)).IsNotNull(value);

            ArgumentName = argumentName;
			Value = value;
        }

        public string ArgumentName { get; }
        public CodeExpression Value { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Value != null) yield return Value;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.OutputIdentifier(TokenType.Identifier, ArgumentName);
            generator.Write(TokenType.Punctuation, ':');
            generator.Write(TokenType.Space, ' ');
            Value.Generate(generator);
		}
	}
}
