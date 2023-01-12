using System.IO;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class TextCodeWriter : ICodeWriter
	{
		private readonly TextWriter writer;

		public TextCodeWriter(TextWriter writer)
		{
			Ensure.That(nameof(writer)).IsNotNull(writer);

			this.writer = writer;
		}

		public void Dispose()
		{
			writer.Dispose();
		}

		public int Indent { get; set; }
		public void Flush() => writer.Flush();
		public void Write(TokenType type, char text) => writer.Write(text);
		public void Write(TokenType type, string text) => writer.Write(text);
		public void WriteLine() => writer.WriteLine();
		public void WriteLine(TokenType type, char text) => writer.WriteLine(text);
		public void WriteLine(TokenType type, string text) => writer.WriteLine(text);
		public void EnterElement(CodeElement element) {}
		public void ExitElement() {}
	}
}
