// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public abstract class CodeBasicMethodMember : CodeCompositeTypeMember
    {
		public CodeBasicMethodMember(CodeMemberModifiers modifiers, IEnumerable<CodeParameterDeclaration> parameters, IEnumerable<CodeStatement> statements)
			: base(modifiers)
		{
			Parameters.AddRange(parameters);
			Statements.AddRange(statements);
		}

        public List<CodeParameterDeclaration> Parameters { get; } = new List<CodeParameterDeclaration>();
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Parameters) yield return child;
				foreach(var child in Statements) yield return child;
			}
		}
    }
}
