// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeGeneratorOptions
    {
		public enum BracingStyleOption
		{
			AddNewLine,
			OnSameLine,
		}

        public CodeGeneratorOptions(
			string indentString = "    ",
			BracingStyleOption bracingStyle = BracingStyleOption.AddNewLine,
			bool elseOnClosing = false,
			IEnumerable<CodePredeclaredType> predeclaredTypes = null)
		{
			IndentString = indentString;
			BracingStyle = bracingStyle;
			MiddleClosingBraceOnSameLine = elseOnClosing;
			PredeclaredTypes = predeclaredTypes != null
				? new List<CodePredeclaredType>(predeclaredTypes)
				: new List<CodePredeclaredType>();
		}

        public string IndentString { get; } = "    ";
        public BracingStyleOption BracingStyle { get; }
        public bool MiddleClosingBraceOnSameLine { get; }
		public List<CodePredeclaredType> PredeclaredTypes { get; }
    }
}
