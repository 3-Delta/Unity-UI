using System;
using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public class CodeConstructorInitializer : CodeElement
	{
		public enum InitializerKind
		{
			Base,
			This,
		}

		public CodeConstructorInitializer(InitializerKind kind, IEnumerable<CodeExpression> arguments)
		{
			Kind = kind;
			Arguments.AddRange(arguments);
		}

		public InitializerKind Kind { get; }
		public List<CodeExpression> Arguments { get; } = new List<CodeExpression>();

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach(var child in base.Children) yield return child;
				foreach(var child in Arguments) yield return child;
			}
		}

		public void Generate(CodeGenerator generator)
		{
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.Punctuation, ':');
            generator.Write(TokenType.Space, ' ');
			switch (Kind)
			{
				case InitializerKind.Base: generator.Write(TokenType.Keyword, "base"); break;
				case InitializerKind.This: generator.Write(TokenType.Keyword, "this"); break;
				default: throw new NotSupportedException();
			}
            generator.Write(TokenType.Punctuation, '(');
            Arguments.GenerateCommaSeparated(generator);
            generator.Write(TokenType.Punctuation, ')');
		}
	}
}
