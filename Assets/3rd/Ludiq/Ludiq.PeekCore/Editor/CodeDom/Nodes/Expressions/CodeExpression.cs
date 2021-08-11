// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public abstract class CodeExpression : CodeElement
    {
		public enum PrecedenceGroup
		{
			Primary,
			Unary,
			Binary,
			Ternary,
			Assignment,
		}

		public abstract PrecedenceGroup Precedence { get; }

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

			GenerateInner(generator);

			generator.ExitElement();
		}

		protected abstract void GenerateInner(CodeGenerator generator);
    }
}
