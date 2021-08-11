using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public class CodeBracedInitializerExpression : CodeExpression
	{
		public CodeBracedInitializerExpression(IEnumerable<CodeExpression> arguments)
		{
			Ensure.That(nameof(arguments)).IsNotNull(arguments);

			Arguments.AddRange(arguments);
		}

		public List<CodeExpression> Arguments { get; } = new List<CodeExpression>();

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children) yield return child;
				foreach (var child in Arguments) yield return child;
			}
		}

		protected override void GenerateInner(CodeGenerator generator)
		{
			generator.WriteLine(TokenType.Punctuation, '{');
			Arguments.GenerateCommaSeparated(generator);
			generator.Write(TokenType.Punctuation, '}');
		}
	}
}
