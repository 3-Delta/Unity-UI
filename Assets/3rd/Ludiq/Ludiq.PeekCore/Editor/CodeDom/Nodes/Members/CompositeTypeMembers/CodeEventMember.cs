// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeEventMember : CodeCompositeTypeMember
    {
        public CodeEventMember(CodeMemberModifiers modifiers, CodeTypeReference type, string name)
			: base(modifiers)
		{
			Type = type;
			Name = name;
		}

        public CodeTypeReference Type { get; }
        public string Name { get; }
        public CodeTypeReference ExplicitImplementationType { get; set; }

		public override MemberCategory Category => MemberCategory.Field;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Type != null) yield return Type;
				if (ExplicitImplementationType != null) yield return ExplicitImplementationType;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
            if (ExplicitImplementationType == null)
            {
				Modifiers.Generate(generator);
            }
            generator.Write(TokenType.Keyword, "event");
            generator.Write(TokenType.Space, ' ');

            Type.Generate(generator);
            if (ExplicitImplementationType != null)
			{
                ExplicitImplementationType.Generate(generator);
				generator.Write(TokenType.Punctuation, '.');
            }
            generator.OutputIdentifier(TokenType.Identifier, Name);
            generator.WriteLine(TokenType.Punctuation, ';');
		}
	}
}
