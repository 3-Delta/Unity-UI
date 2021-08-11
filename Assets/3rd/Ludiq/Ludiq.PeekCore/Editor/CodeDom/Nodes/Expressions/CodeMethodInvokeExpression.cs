// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeMethodInvokeExpression : CodeExpression
    {
        public CodeMethodInvokeExpression(CodeMethodReferenceExpression method, IEnumerable<CodeExpression> arguments)
        {
			Ensure.That(nameof(method)).IsNotNull(method);
			Ensure.That(nameof(arguments)).IsNotNull(arguments);

            Method = method;
            Arguments.AddRange(arguments);
        }

        public CodeMethodReferenceExpression Method { get; }
        public List<CodeExpression> Arguments { get; } = new List<CodeExpression>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (Method != null) yield return Method;
				foreach (var child in Arguments) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            Method.Generate(generator);
            generator.Write(TokenType.Punctuation, '(');
            Arguments.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');
		}
	}
}
