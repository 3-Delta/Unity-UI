// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeIndexExpression : CodeExpression
    {
        public CodeIndexExpression(CodeExpression targetObject, IEnumerable<CodeExpression> indices)
        {
			Ensure.That(nameof(targetObject)).IsNotNull(targetObject);
			Ensure.That(nameof(indices)).IsNotNull(indices);

            TargetObject = targetObject;
            Indices.AddRange(indices);
        }

        public CodeExpression TargetObject { get; }
        public List<CodeExpression> Indices { get; } = new List<CodeExpression>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (TargetObject != null) yield return TargetObject;
				foreach (var child in Indices) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, '(');
            TargetObject.Generate(generator);
			if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, ')');

            generator.Write(TokenType.Punctuation, '[');

            bool first = true;
            foreach (CodeExpression index in Indices)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    generator.Write(TokenType.Punctuation, ',');
					generator.Write(TokenType.Space, ' ');
                }
                index.Generate(generator);
            }

            generator.Write(TokenType.Punctuation, ']');
		}
	}
}
