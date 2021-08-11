namespace Ludiq.PeekCore
{
	public abstract class KeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem>, IKeyedCollection<TKey, TItem>
	{
		public bool TryGetValue(TKey key, out TItem item)
		{
			Ensure.That(nameof(key)).IsNotNull(key);

			if (Dictionary != null)
			{
				return Dictionary.TryGetValue(key, out item);
			}

			foreach (var itemInItems in Items)
			{
				var keyInItems = GetKeyForItem(itemInItems);

				if (keyInItems != null && Comparer.Equals(key, keyInItems))
				{
					item = itemInItems;
					return true;
				}
			}

			item = default;
			return false;
		}

		// TODO: Make sure duck typing works even on base class
		public new NoAllocEnumerator<TItem> GetEnumerator()
		{
			return new NoAllocEnumerator<TItem>(this);
		}
	}
}