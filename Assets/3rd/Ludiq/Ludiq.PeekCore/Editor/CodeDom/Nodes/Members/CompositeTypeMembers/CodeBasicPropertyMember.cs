// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public abstract class CodeBasicPropertyMember : CodeCompositeTypeMember
    {
		public CodeBasicPropertyMember(CodeMemberModifiers modifiers, CodeTypeReference type, CodeBasicPropertyAccessor getter, CodeBasicPropertyAccessor setter)
			: base(modifiers)
		{
			Type = type;
			Getter = getter;
			Setter = setter;
		}

        public CodeTypeReference Type { get; }
        public CodeTypeReference ExplicitImplementationType { get; set; }
		public CodeBasicPropertyAccessor Getter { get; }
		public CodeBasicPropertyAccessor Setter { get; }

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Type != null) yield return Type;
				if (ExplicitImplementationType != null) yield return ExplicitImplementationType;
				if (Getter != null) yield return Getter;
				if (Setter != null) yield return Setter;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
            if (ExplicitImplementationType == null)
            {
                Modifiers.Generate(generator);
            }

            Type.Generate(generator);
            generator.Write(TokenType.Space, ' ');

            if (ExplicitImplementationType != null)
            {
                ExplicitImplementationType.Generate(generator);
                generator.Write(TokenType.Punctuation, '.');
            }

			GeneratePropertyName(generator);

			if (Getter != null
				&& Setter == null
				&& Getter is CodeUserPropertyAccessor userGetter
				&& userGetter.Statements.Count == 1
				&& userGetter.Statements[0] is CodeReturnStatement returnStatement
				&& returnStatement.Expression != null)
			{
				generator.Write(TokenType.Space, ' ');
				generator.Write(TokenType.Operator, "=>");
				generator.Write(TokenType.Space, ' ');
				returnStatement.Expression.Generate(generator);
				generator.WriteLine(TokenType.Punctuation, ";");
			}
			else
			{
				generator.WriteOpeningBrace();
				generator.Indent++;
				Getter?.Generate(generator, "get", this, enclosingType);
				Setter?.Generate(generator, "set", this, enclosingType);
				generator.Indent--;
				generator.WriteClosingBrace();
			}
		}

		protected abstract void GeneratePropertyName(CodeGenerator generator);
    }
}
