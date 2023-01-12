// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeIndexerMember : CodeBasicPropertyMember
    {
		public CodeIndexerMember(
			CodeMemberModifiers modifiers,
			CodeTypeReference type,
			IEnumerable<CodeParameterDeclaration> parameters,
			CodeBasicPropertyAccessor getter,
			CodeBasicPropertyAccessor setter) : base(modifiers, type, getter, setter)
		{
			Parameters.AddRange(parameters);
		}

		public override MemberCategory Category => ((Modifiers & CodeMemberModifiers.ScopeMask) == CodeMemberModifiers.Static) ? MemberCategory.StaticIndexer : MemberCategory.Indexer;

        public List<CodeParameterDeclaration> Parameters { get; } = new List<CodeParameterDeclaration>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Parameters) yield return child;
			}
		}

		protected override void GeneratePropertyName(CodeGenerator generator)
		{						
			generator.Write(TokenType.Keyword, "this");
			generator.Write(TokenType.Punctuation, '[');
			Parameters.GenerateCommaSeparated(generator);
			generator.Write(TokenType.Punctuation, ']');
		}
	}
}
