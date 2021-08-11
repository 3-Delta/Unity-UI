using System.IO;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class FileCodeWriterSystem : CodeWriterSystem<TextCodeWriter>
	{
		private readonly string directoryPath;

		public FileCodeWriterSystem(string directoryPath)
		{
			Ensure.That(nameof(directoryPath)).IsNotNull(directoryPath);

			this.directoryPath = directoryPath;
		}

		public override TextCodeWriter OpenWriter(string className)
		{
			return new TextCodeWriter(new StreamWriter(Path.Combine(directoryPath, className + ".cs")));
		}
	}
}
