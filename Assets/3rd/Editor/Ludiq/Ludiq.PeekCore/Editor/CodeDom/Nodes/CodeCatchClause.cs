// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeCatchClause : CodeElement
    {
        public CodeCatchClause(string localName, CodeTypeReference catchExceptionType, IEnumerable<CodeStatement> statements)
        {
			LocalName = localName;
			CatchExceptionType = catchExceptionType;
            Statements.AddRange(statements);
        }

        public string LocalName { get; }
        public CodeTypeReference CatchExceptionType { get; }
        public List<CodeStatement> Statements { get; } = new List<CodeStatement>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				if (CatchExceptionType != null) yield return CatchExceptionType;
				foreach (var child in Statements) yield return child;
			}
		}
    }
}
