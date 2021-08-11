// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeMethodMember : CodeBasicMethodMember
    {
		public CodeMethodMember(CodeMemberModifiers modifiers, CodeTypeReference returnType, string name, IEnumerable<CodeParameterDeclaration> parameters, IEnumerable<CodeStatement> statements)
			: base(modifiers, parameters, statements)
		{
			ReturnType = returnType;
			Name = name;
		}

        public CodeTypeReference ReturnType { get; }
        public string Name { get; }
        public CodeTypeReference ExplicitImplementationType { get; set; }
        public List<CodeAttributeDeclaration> ReturnTypeCustomAttributes { get; } = new List<CodeAttributeDeclaration>();
        public List<CodeTypeParameter> TypeParameters { get; } = new List<CodeTypeParameter>();

		public override MemberCategory Category => ((Modifiers & CodeMemberModifiers.ScopeMask) == CodeMemberModifiers.Static) ? MemberCategory.StaticMethod : MemberCategory.Method;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (ReturnType != null) yield return ReturnType;
				if (ExplicitImplementationType != null) yield return ExplicitImplementationType;
				foreach(var child in ReturnTypeCustomAttributes) yield return child;
				foreach(var child in TypeParameters) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeCompositeTypeDeclaration enclosingType)
		{
            generator.GenerateAttributes(ReturnTypeCustomAttributes, "return");

            if (ExplicitImplementationType == null)
            {
				Modifiers.Generate(generator);
            }

            ReturnType.Generate(generator);
            generator.Write(TokenType.Space, ' ');
            if (ExplicitImplementationType != null)
            {
                ExplicitImplementationType.Generate(generator);
                generator.Write(TokenType.Punctuation, '.');
            }
            generator.OutputIdentifier(TokenType.Identifier, Name);

            generator.OutputTypeParameters(TypeParameters);

            generator.Write(TokenType.Punctuation, '(');
			Parameters.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');

            foreach (var typeParameter in TypeParameters)
            {
                typeParameter.GenerateConstraints(generator);
            }

			if (enclosingType.IsInterface
			|| (Modifiers & CodeMemberModifiers.ScopeMask) == CodeMemberModifiers.Abstract)
			{
                generator.WriteLine(TokenType.Punctuation, ';');
			}
			else
            {
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
}
