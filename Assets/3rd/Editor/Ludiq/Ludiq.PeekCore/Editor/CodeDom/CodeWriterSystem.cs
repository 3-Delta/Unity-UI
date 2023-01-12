namespace Ludiq.PeekCore.CodeDom
{
	public abstract class CodeWriterSystem<TWriter> : ICodeWriterSystem where TWriter : ICodeWriter
	{
		ICodeWriter ICodeWriterSystem.OpenWriter(string className) => OpenWriter(className);

		public abstract TWriter OpenWriter(string className);
	}
}
