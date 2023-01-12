// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeAttributeDeclaration : CodeElement
    {
        public CodeAttributeDeclaration(CodeTypeReference attributeType)
		{			
            AttributeType = attributeType;
		}

        public CodeAttributeDeclaration(CodeTypeReference attributeType, IEnumerable<CodeAttributeArgument> arguments)
			: this(attributeType)
        {
            Arguments.AddRange(arguments);
        }

        public CodeTypeReference AttributeType { get; }
        public List<CodeAttributeArgument> Arguments { get; } = new List<CodeAttributeArgument>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (AttributeType != null) yield return AttributeType;
				foreach(var child in Arguments) yield return child;
			}
		}
    }
}
