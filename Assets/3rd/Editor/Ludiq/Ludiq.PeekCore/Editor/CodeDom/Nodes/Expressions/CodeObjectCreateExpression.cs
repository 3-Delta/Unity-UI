// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeObjectCreateExpression : CodeExpression
    {
        public CodeObjectCreateExpression(CodeTypeReference createType, IEnumerable<CodeExpression> arguments)
        {
			Ensure.That(nameof(createType)).IsNotNull(createType);
			Ensure.That(nameof(arguments)).IsNotNull(arguments);

            CreateType = createType;
            Arguments.AddRange(arguments);
        }

        public CodeObjectCreateExpression(CodeTypeReference createType, IEnumerable<CodeExpression> arguments, IEnumerable<IEnumerable<CodeExpression>> collectionInitializerItems)
        {
			Ensure.That(nameof(createType)).IsNotNull(createType);
			Ensure.That(nameof(arguments)).IsNotNull(arguments);
			Ensure.That(nameof(collectionInitializerItems)).IsNotNull(collectionInitializerItems);

            CreateType = createType;
            Arguments.AddRange(arguments);
			foreach (var collectionInitializerItem in collectionInitializerItems)
			{
				CollectionInitializerItems.Add(new List<CodeExpression>(collectionInitializerItem));
			}
        }

        public CodeTypeReference CreateType { get; }
        public List<CodeExpression> Arguments { get; } = new List<CodeExpression>();
		public List<List<CodeExpression>> CollectionInitializerItems { get; } = new List<List<CodeExpression>>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (CreateType != null) yield return CreateType;
				foreach (var child in Arguments) yield return child;
				foreach (var collectionInitializerItem in CollectionInitializerItems)
				{
					foreach (var child in collectionInitializerItem) yield return child;
				}
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Keyword, "new");
            generator.Write(TokenType.Space, ' ');
            CreateType.Generate(generator);
            generator.Write(TokenType.Punctuation, '(');
            Arguments.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');

			if (CollectionInitializerItems.Count > 0)
			{
				generator.Write(TokenType.Space, ' ');
				generator.WriteLine(TokenType.Punctuation, '{');
				generator.Indent++;
				for (int i = 0; i < CollectionInitializerItems.Count; i++)
				{
					var collectionInitializerItem = CollectionInitializerItems[i];

					if (collectionInitializerItem.Count != 1)
					{
						generator.Write(TokenType.Punctuation, '{');
						collectionInitializerItem.GenerateCommaSeparated(generator);
						generator.Write(TokenType.Punctuation, '}');
					}
					else
					{
						collectionInitializerItem.First().Generate(generator);
					}

					generator.WriteLine(TokenType.Punctuation, i < CollectionInitializerItems.Count - 1 ? "," : "");
				}
				generator.Indent--;
				generator.Write(TokenType.Punctuation, '}');
			}
		}
	}
}
