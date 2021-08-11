// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeConstructorMember : CodeBasicMethodMember
    {
		public CodeConstructorMember(CodeMemberModifiers modifiers, IEnumerable<CodeParameterDeclaration> parameters, IEnumerable<CodeStatement> statements)
			: this(modifiers, parameters, null, statements)
		{
		}

		public CodeConstructorMember(CodeMemberModifiers modifiers, IEnumerable<CodeParameterDeclaration> parameters, CodeConstructorInitializer initializer, IEnumerable<CodeStatement> statements)
			: base(modifiers, parameters, statements)
		{
			Initializer = initializer;
		}

        public CodeConstructorInitializer Initializer { get; }

		public override MemberCategory Category => MemberCategory.Constructor;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Initializer != null) yield return Initializer;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
            Modifiers.Generate(generator);

            generator.OutputIdentifier(TokenType.Identifier, enclosingType.Name);
            generator.Write(TokenType.Punctuation, '(');
            Parameters.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');

            if (Initializer != null)
            {
				Initializer.Generate(generator);
            }
			
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
