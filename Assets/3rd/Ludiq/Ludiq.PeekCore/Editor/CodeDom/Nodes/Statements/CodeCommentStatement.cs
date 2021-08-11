// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed  class CodeCommentStatement : CodeStatement
    {
        public CodeCommentStatement(CodeComment comment)
        {
            Comment = comment;
        }

        public CodeComment Comment { get; set; }

		public override bool IsTerminator => false;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				if (Comment != null) yield return Comment;
			}
		}

		protected override void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.WriteBlankLineIfJustExitedBlock();
            Comment.Generate(generator);
			generator.WriteLine();
		}
    }
}
