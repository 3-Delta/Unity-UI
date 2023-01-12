// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeStaticConstructorMember : CodeBasicMethodMember
    {
		public CodeStaticConstructorMember(IEnumerable<CodeStatement> statements)
			: base(default(CodeMemberModifiers), Enumerable.Empty<CodeParameterDeclaration>(), statements)
		{
		}

		public override MemberCategory Category => MemberCategory.StaticConstructor;

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
            generator.Write(TokenType.Keyword, "static");
			generator.Write(TokenType.Space, ' '); 
            generator.Write(TokenType.Identifier, enclosingType.Name);
            generator.Write(TokenType.Punctuation, "()");

			if (Statements.Count > 0)
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				generator.EnterLocalScope();
				foreach (var parameter in Parameters)
				{
					generator.ReserveLocal(parameter.Name);
				}
				Statements.Generate(generator, default(CodeStatementEmitOptions));
				generator.ExitLocalScope();
				generator.Indent--;
				generator.WriteClosingBrace();
			}
			else
			{
				generator.WriteEmptyBlock();
			}
		}
	}
}
