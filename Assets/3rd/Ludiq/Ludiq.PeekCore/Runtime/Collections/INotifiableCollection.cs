namespace Ludiq.PeekCore
{
	public interface INotifiableCollection<TItem>
	{
		void RaiseBeforeAdd(TItem item);

		void RaiseAfterAdd(TItem item);

		void RaiseBeforeRemove(TItem item);

		void RaiseAfterRemove(TItem item);

		void RaiseBeforeSet(TItem previous, TItem next);

		void RaiseAfterSet(TItem previous, TItem next);
	}
}