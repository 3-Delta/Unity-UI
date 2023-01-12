// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeRegionBeginDirective : CodeDirective
    {
        public CodeRegionBeginDirective(string regionText)
        {
            RegionText = regionText;
        }

        public string RegionText { get; }

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Directive, "#region ");
            generator.WriteLine(TokenType.Directive, RegionText);
		}
    }
}
