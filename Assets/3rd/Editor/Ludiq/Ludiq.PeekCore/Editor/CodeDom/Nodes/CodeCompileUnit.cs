// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeCompileUnit : CodeElement
    {
        public List<CodeDirective> StartDirectives { get; } = new List<CodeDirective>();
        public List<CodeDirective> EndDirectives { get; } = new List<CodeDirective>();
        public HashSet<CodeUsingImport> Usings { get; } = new HashSet<CodeUsingImport>();
        public List<CodeNamespace> Namespaces { get; } = new List<CodeNamespace>();
        public List<string> ReferencedAssemblies { get; } = new List<string>();
        public List<CodeAttributeDeclaration> AssemblyCustomAttributes { get; } = new List<CodeAttributeDeclaration>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in StartDirectives) yield return child;
				foreach (var child in EndDirectives) yield return child;
				foreach (var child in Namespaces) yield return child;
				foreach (var child in AssemblyCustomAttributes) yield return child;
			}
		}

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

			generator.WriteLine(TokenType.Comment, @"//--------------------------------------------------------------");
			generator.WriteLine(TokenType.Comment, @"//              _______      _____      __       ________       ");
			generator.WriteLine(TokenType.Comment, @"//             |  ___  \    /     \    |  |     |___  ___|      ");
			generator.WriteLine(TokenType.Comment, @"//             | |   \  |  /  ___  \   |  |         / /         ");
			generator.WriteLine(TokenType.Comment, @"//             | |___/ /  /  /   \  \  |  |        / /_         ");
			generator.WriteLine(TokenType.Comment, @"//             | |   \ \  \  \___/  /  |  |       /_  /         ");
			generator.WriteLine(TokenType.Comment, @"//             | |___/  |  \       /   |  |____    | /          ");
			generator.WriteLine(TokenType.Comment, @"//             |_______/    \_____/    |_______|   |/           ");
			generator.WriteLine(TokenType.Comment, @"//                                                              ");
			generator.WriteLine(TokenType.Comment, @"//                 V I S U A L    S C R I P T I N G             ");
			generator.WriteLine(TokenType.Comment, @"//--------------------------------------------------------------");
			generator.WriteLine(TokenType.Comment, @"//                                                              ");
			generator.WriteLine(TokenType.Comment, @"// THIS FILE IS AUTO-GENERATED.                                 ");
			generator.WriteLine(TokenType.Comment, @"//                                                              ");
			generator.WriteLine(TokenType.Comment, @"// ANY CHANGES WILL BE LOST NEXT TIME THIS SCRIPT IS GENERATED. ");
			generator.WriteLine(TokenType.Comment, @"//                                                              ");
			generator.WriteLine(TokenType.Comment, @"//--------------------------------------------------------------");

            StartDirectives.Generate(generator);
			if (StartDirectives.Count > 0) generator.WriteLine();

			Usings.Generate(generator);

            if (AssemblyCustomAttributes.Count > 0)
            {
                generator.GenerateAttributes(AssemblyCustomAttributes, "assembly");
                generator.WriteLine();
            }

			generator.PushUsingSet(Usings);
            Namespaces.Generate(generator);
			generator.PopUsingSet();

			if (EndDirectives.Count > 0) generator.WriteLine();
            EndDirectives.Generate(generator);

			generator.ExitElement();
		}
	}
}
