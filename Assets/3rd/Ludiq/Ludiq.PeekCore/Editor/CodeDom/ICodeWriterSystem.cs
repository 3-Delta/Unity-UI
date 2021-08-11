namespace Ludiq.PeekCore.CodeDom
{
	public interface ICodeWriterSystem
	{
		ICodeWriter OpenWriter(string className);
	}
}
