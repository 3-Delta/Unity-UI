// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeMethodReferenceExpression : CodeExpression
    {
        public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName, IEnumerable<CodeTypeReference> typeArguments)
        {
			Ensure.That(nameof(targetObject)).IsNotNull(targetObject);
			Ensure.That(nameof(methodName)).IsNotNullOrEmpty(methodName);
			Ensure.That(nameof(typeArguments)).IsNotNull(typeArguments);

            TargetObject = targetObject;
            MethodName = methodName;
            TypeArguments.AddRange(typeArguments);
        }

        public CodeExpression TargetObject { get; }
        public string MethodName { get; }
        public List<CodeTypeReference> TypeArguments { get; } = new List<CodeTypeReference>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (TargetObject != null) yield return TargetObject;
				foreach (var child in TypeArguments) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			bool needsTarget = true;
			if (TargetObject is CodeThisReferenceExpression)
			{
				needsTarget = generator.ContainsLocalByName(MethodName);
			}

			if (needsTarget)
			{
				if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, '(');
				TargetObject.Generate(generator);
				if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, ')');
				generator.Write(TokenType.Punctuation, '.');
			}

            generator.OutputIdentifier(TokenType.Identifier, MethodName);

            if (TypeArguments.Count > 0)
            {
				generator.Write(TokenType.Punctuation, '<');

				bool first = true;
				foreach (var typeArgument in TypeArguments)
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
					typeArgument.Generate(generator);
				}

				generator.Write(TokenType.Punctuation, '>');
            }
		}
	}
}
