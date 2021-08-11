namespace Ludiq.PeekCore
{
	public interface ICollectionChild<TParent> where TParent : class
	{
		TParent parent { get; set; }
	}
}