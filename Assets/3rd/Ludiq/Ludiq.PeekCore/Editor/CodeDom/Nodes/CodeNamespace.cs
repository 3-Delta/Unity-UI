// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeNamespace : CodeElement
    {
        public CodeNamespace(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public List<CodeBasicTypeDeclaration> Types { get; } = new List<CodeBasicTypeDeclaration>();
        public HashSet<CodeUsingImport> Usings { get; } = new HashSet<CodeUsingImport>();
        public List<CodeComment> Comments { get; } = new List<CodeComment>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Types) yield return child;
				foreach (var child in Comments) yield return child;
			}
		}

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

            Comments.Generate(generator);
            
            generator.Write(TokenType.Keyword, "namespace");
            generator.Write(TokenType.Space, ' ');
            var names = Name.Split('.');
            generator.OutputIdentifier(TokenType.Identifier, names[0]);
            for (int i = 1; i < names.Length; i++)
            {
                generator.Write(TokenType.Punctuation, '.');
                generator.OutputIdentifier(TokenType.Identifier, names[i]);
            }
            generator.WriteOpeningBrace();
            generator.Indent++;

			Usings.Generate(generator);

			Usings.Add(new CodeUsingImport(Name));
			generator.PushUsingSet(Usings);

			bool needsBlankLine = false;
            foreach (var type in Types)
            {
                if (needsBlankLine)
                {
                    generator.WriteLine();
                }

                type.Generate(generator);
				needsBlankLine = true;
            }

			generator.PopUsingSet();

            generator.Indent--;
            generator.WriteClosingBrace();

			generator.ExitElement();
		}
    }
}
