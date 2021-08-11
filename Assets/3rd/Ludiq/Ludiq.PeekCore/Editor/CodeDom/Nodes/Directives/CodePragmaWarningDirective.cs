using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#pragma warning disable 1234, 1234

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodePragmaWarningDirective : CodeDirective
	{
        public CodePragmaWarningDirective(CodePragmaWarningSetting setting, IEnumerable<int> warnings)
        {
			Setting = setting;
            Warnings.AddRange(warnings);
        }

		public CodePragmaWarningSetting Setting { get; }
        public List<int> Warnings { get; } = new List<int>();

		protected override void GenerateInner(CodeGenerator generator)
		{
            if (Warnings != null && Warnings.Any())
			{
				generator.Write(TokenType.Directive, "#pragma warning ");

				switch (Setting)
				{
					case CodePragmaWarningSetting.Disable: generator.Write(TokenType.Directive, "disable "); break;
					case CodePragmaWarningSetting.Restore: generator.Write(TokenType.Directive, "restore "); break;
				}

				bool first = true;
                foreach (var warning in Warnings)
                {
					if (first)
					{
						first = false;
					}
					else
					{
						generator.Write(TokenType.Punctuation, ", ");
					}

					generator.Write(TokenType.IntLiteral, warning.ToString(CultureInfo.InvariantCulture));
                }
				generator.WriteLine();
            }
		}
	}
}
