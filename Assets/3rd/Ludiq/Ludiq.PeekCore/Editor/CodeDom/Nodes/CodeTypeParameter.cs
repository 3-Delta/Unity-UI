// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeTypeParameter : CodeElement
    {
        public CodeTypeParameter(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public List<CodeTypeReference> Constraints { get; } = new List<CodeTypeReference>();
        public List<CodeAttributeDeclaration> CustomAttributes { get; } = new List<CodeAttributeDeclaration>();
        public bool HasConstructorConstraint { get; set; }

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Constraints) yield return child;
				foreach (var child in CustomAttributes) yield return child;
			}
		}

		public void GenerateDeclaration(CodeGenerator generator)
		{
            generator.GenerateAttributes(CustomAttributes, null, true);
            generator.Write(TokenType.GenericTypeParameter, Name);
		}

		public void GenerateConstraints(CodeGenerator generator)
		{
			generator.EnterElement(this);

            generator.WriteLine();
            generator.Indent++;

            bool first = true;
            foreach (CodeTypeReference constraint in Constraints)
            {
                if (first)
                {
                    generator.Write(TokenType.Keyword, "where");
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.GenericTypeParameter, Name);
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.Punctuation, ':');
                    generator.Write(TokenType.Space, ' ');
                    first = false;
                }
                else
                {
                    generator.Write(TokenType.Punctuation, ',');
                    generator.Write(TokenType.Space, ' ');
                }
                constraint.Generate(generator);
            }

            if (HasConstructorConstraint)
            {
                if (first)
                {
                    generator.Write(TokenType.Keyword, "where");
                    generator.Write(TokenType.GenericTypeParameter, Name);
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.Punctuation, ':');
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.Keyword, "new");
                    generator.Write(TokenType.Punctuation, "()");
                    first = false;
                }
                else
                {
                    generator.Write(TokenType.Punctuation, ',');
                    generator.Write(TokenType.Space, ' ');
                    generator.Write(TokenType.Keyword, "new");
                    generator.Write(TokenType.Punctuation, "()");
                }
            }

            generator.Indent--;

			generator.ExitElement();
		}
	}
}


