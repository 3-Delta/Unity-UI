// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeFieldMember : CodeCompositeTypeMember
    {
        public CodeFieldMember(CodeMemberModifiers modifiers, CodeTypeReference type, string name)
			: base(modifiers)
        {
            Type = type;
            Name = name;
        }

        public CodeFieldMember(CodeMemberModifiers modifiers, CodeTypeReference type, string name, CodeExpression initializer)
			: base(modifiers)
        {
            Type = type;
            Name = name;
			Initializer = initializer;
        }

        public CodeTypeReference Type { get; }
        public string Name { get; }
        public CodeExpression Initializer { get; }

		public override MemberCategory Category => MemberCategory.Field;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Type != null) yield return Type;
				if (Initializer != null) yield return Initializer;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
			Modifiers.Generate(generator);
            Type.Generate(generator);
			generator.Write(TokenType.Space, ' ');
			generator.OutputIdentifier(TokenType.Identifier, Name);

            if (Initializer != null)
            {
                generator.Write(TokenType.Space, ' ');
                generator.Write(TokenType.Punctuation, '=');
                generator.Write(TokenType.Space, ' ');
                Initializer.Generate(generator);
            }

            generator.WriteLine(TokenType.Punctuation, ';');
		}
	}
}
