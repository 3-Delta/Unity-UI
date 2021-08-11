// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeFieldReferenceExpression : CodeExpression
    {
        public CodeFieldReferenceExpression(CodeExpression targetObject, string fieldName)
        {
			Ensure.That(nameof(targetObject)).IsNotNull(targetObject);
			Ensure.That(nameof(fieldName)).IsNotNull(fieldName);

            TargetObject = targetObject;
            FieldName = fieldName;
        }

        public CodeExpression TargetObject { get; }
        public string FieldName { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (TargetObject != null) yield return TargetObject;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			bool needsTarget = true;
			if (TargetObject is CodeThisReferenceExpression)
			{
				needsTarget = generator.ContainsLocalByName(FieldName);
			}

			if (needsTarget)
			{				
				if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, '(');
				TargetObject.Generate(generator);
				if (TargetObject.Precedence > PrecedenceGroup.Primary) generator.Write(TokenType.Punctuation, ')');
				generator.Write(TokenType.Punctuation, '.');
			}
            generator.OutputIdentifier(TokenType.Identifier, FieldName);
		}
	}
}
