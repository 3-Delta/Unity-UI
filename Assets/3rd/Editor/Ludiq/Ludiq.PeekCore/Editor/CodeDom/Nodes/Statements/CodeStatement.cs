// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public abstract class CodeStatement : CodeElement
    {
        public List<CodeDirective> StartDirectives { get; } = new List<CodeDirective>();
        public List<CodeDirective> EndDirectives { get; } = new List<CodeDirective>();

		public void Generate(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
			generator.EnterElement(this);

            StartDirectives.Generate(generator);
			GenerateInner(generator, emitOptions);
            EndDirectives.Generate(generator);

			generator.ExitElement();
		}

		public abstract bool IsTerminator { get; }

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in StartDirectives) yield return child;
				foreach(var child in EndDirectives) yield return child;
			}
		}

		public virtual void ReserveLocals(CodeGenerator generator, CodeStatementEmitOptions emitOptions)
		{
		}

		protected abstract void GenerateInner(CodeGenerator generator, CodeStatementEmitOptions emitOptions);
	}
}
