using System;

namespace Ludiq.PeekCore.CodeDom
{
	public interface ICodeWriter : IDisposable
	{
		int Indent { get; set; }

		void Flush();
		void Write(TokenType type, char text);
		void Write(TokenType type, string text);
		void WriteLine();
		void WriteLine(TokenType type, char text);
		void WriteLine(TokenType type, string text);
		void EnterElement(CodeElement element);
		void ExitElement();
	}
}
