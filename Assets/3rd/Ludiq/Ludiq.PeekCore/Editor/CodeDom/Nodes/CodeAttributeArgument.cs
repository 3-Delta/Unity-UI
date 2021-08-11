// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeAttributeArgument : CodeElement
    {
        public CodeAttributeArgument(CodeExpression value)
        {
            Value = value;
        }

        public CodeAttributeArgument(string name, CodeExpression value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public CodeExpression Value { get; }

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

            if (!string.IsNullOrEmpty(Name))
            {
                generator.OutputIdentifier(TokenType.Identifier, Name);
				generator.Write(TokenType.Space, ' ');
                generator.Write(TokenType.Punctuation, '=');
				generator.Write(TokenType.Space, ' ');
            }
            Value.Generate(generator);

			generator.ExitElement();
		}
	}
}
