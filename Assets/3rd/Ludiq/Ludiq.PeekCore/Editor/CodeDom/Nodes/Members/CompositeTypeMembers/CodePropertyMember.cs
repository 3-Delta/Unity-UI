// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodePropertyMember : CodeBasicPropertyMember
    {
		public CodePropertyMember(CodeMemberModifiers modifiers, CodeTypeReference type, string name, CodeBasicPropertyAccessor getter, CodeBasicPropertyAccessor setter)
			: base(modifiers, type, getter, setter)
		{
			Name = name;
		}

		public override MemberCategory Category => ((Modifiers & CodeMemberModifiers.ScopeMask) == CodeMemberModifiers.Static) ? MemberCategory.StaticProperty : MemberCategory.Property;

        public string Name { get; }

		protected override void GeneratePropertyName(CodeGenerator generator)
		{
			generator.OutputIdentifier(TokenType.Identifier, Name);
		}
	}
}
