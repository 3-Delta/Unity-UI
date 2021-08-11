// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeVariableReferenceExpression : CodeExpression
    {
        public CodeVariableReferenceExpression(string variableName)
        {
			Ensure.That(nameof(variableName)).IsNotNullOrEmpty(variableName);

            VariableName = variableName;
        }

        public string VariableName { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		protected override void GenerateInner(CodeGenerator generator)
		{
			if (VariableName == "value" && generator.IsInSetterProperty)
			{
				generator.OutputIdentifier(TokenType.Keyword, VariableName);
			}
			else
			{
				generator.OutputIdentifier(TokenType.Identifier, VariableName);
			}
		}
	}
}
