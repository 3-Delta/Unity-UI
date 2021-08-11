// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeEnumMember : CodeElement
    {
        public CodeEnumMember(string name)
        {
            Name = name;
        }

        public CodeEnumMember(string name, CodeExpression initializer)
        {
            Name = name;
			Initializer = initializer;
        }

        public string Name { get; }
        public CodeExpression Initializer { get; }

        public List<CodeComment> Comments { get; } = new List<CodeComment>();
        public List<CodeDirective> StartDirectives { get; } = new List<CodeDirective>();
        public List<CodeDirective> EndDirectives { get; } = new List<CodeDirective>();
        public List<CodeAttributeDeclaration> CustomAttributes { get; } = new List<CodeAttributeDeclaration>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Initializer != null) yield return Initializer;
				foreach(var child in Comments) yield return child;
				foreach(var child in StartDirectives) yield return child;
				foreach(var child in EndDirectives) yield return child;
				foreach(var child in CustomAttributes) yield return child;
			}
		}

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

			StartDirectives.Generate(generator);
			Comments.Generate(generator);

			generator.GenerateAttributes(CustomAttributes);
			generator.OutputIdentifier(TokenType.Identifier, Name);
			if (Initializer != null)
			{
				generator.Write(TokenType.Space, ' ');
				generator.Write(TokenType.Punctuation, '=');
				generator.Write(TokenType.Space, ' ');
				Initializer.Generate(generator);
			}
			generator.WriteLine(TokenType.Punctuation, ',');

			EndDirectives.Generate(generator);

			generator.ExitElement();
		}
	}
}
