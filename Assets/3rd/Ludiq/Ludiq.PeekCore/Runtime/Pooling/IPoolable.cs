namespace Ludiq.PeekCore
{
	public interface IPoolable
	{
		void New();
		void Free();
	}
}