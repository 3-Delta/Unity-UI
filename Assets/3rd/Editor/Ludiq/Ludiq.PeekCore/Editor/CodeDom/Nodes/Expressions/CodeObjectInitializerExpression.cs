// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeObjectInitializerExpression : CodeExpression
    {
        public CodeObjectInitializerExpression(CodeTypeReference createType, IEnumerable<KeyValuePair<string, CodeExpression>> members)
        {
			Ensure.That(nameof(createType)).IsNotNull(createType);
			Ensure.That(nameof(members)).IsNotNull(members);

            CreateType = createType;
            Members.AddRange(members);
        }

        public CodeTypeReference CreateType { get; }
        public List<KeyValuePair<string, CodeExpression>> Members { get; } = new List<KeyValuePair<string, CodeExpression>>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (CreateType != null) yield return CreateType;
				foreach (var member in Members) yield return member.Value;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Keyword, "new");
            generator.Write(TokenType.Space, ' ');
			if (CreateType != null)
			{
				CreateType.Generate(generator);
			}
			generator.Write(TokenType.Space, ' ');
			generator.WriteLine(TokenType.Punctuation, '{');

			generator.Indent++;
			for (int i = 0; i < Members.Count; i++)
			{
				var member = Members[i];
				generator.OutputIdentifier(TokenType.Identifier, member.Key);
				generator.Write(TokenType.Space, ' ');
				generator.Write(TokenType.Operator, '=');
				generator.Write(TokenType.Space, ' ');
				member.Value.Generate(generator);
				generator.WriteLine(TokenType.Punctuation, i < Members.Count - 1 ? "," : "");
			}
			generator.Indent--;

			generator.Write(TokenType.Punctuation, '}');
		}
	}
}
