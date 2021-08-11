namespace Ludiq.PeekCore
{
	public interface INotifiedCollectionChild<TParent> : ICollectionChild<TParent> where TParent : class
	{
		void BeforeAdd(TParent parent);

		void AfterAdd(TParent parent);

		void BeforeRemove(TParent parent);

		void AfterRemove(TParent parent);
	}
}