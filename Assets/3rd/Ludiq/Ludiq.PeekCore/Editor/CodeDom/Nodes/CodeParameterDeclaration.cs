// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeParameterDeclaration : CodeElement
    {
        public CodeParameterDeclaration(CodeTypeReference type, string name)
        {
            Type = type;
            Name = name;
        }

        public CodeParameterDeclaration(CodeParameterDirection direction, CodeTypeReference type, string name)
        {
			Direction = direction;
            Type = type;
            Name = name;
        }

        public List<CodeAttributeDeclaration> CustomAttributes { get; } = new List<CodeAttributeDeclaration>();
        public CodeParameterDirection Direction { get; }
        public CodeTypeReference Type { get; }
        public string Name { get; }

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in CustomAttributes) yield return child;
				if (Type != null) yield return Type;
			}
		}

        public void Generate(CodeGenerator generator)
        {
			generator.EnterElement(this);

            if (CustomAttributes.Count > 0)
            {
                generator.GenerateAttributes(CustomAttributes, null, true);
            }

            Direction.Generate(generator);
            Type.Generate(generator);
			generator.Write(TokenType.Space, ' ');
			generator.OutputIdentifier(TokenType.Identifier, Name);

			generator.ExitElement();
        }
    }
}
