using System;
using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public abstract class ParentMergedCollection<TParent, TKey, TItem> :
		MergedKeyedCollection<TKey, TItem>,
		INotifyCollectionChanged, 
		IBulkCollection<TItem>
		where TParent : class
	{
		public event Action itemChanged;

		public event Action collectionChanged;

		private bool delayCollectionChanged;

		private bool collectionChangedDuringDelay;

		protected ParentMergedCollection() { }

		public override void Include<TSubItem>(IKeyedCollection<TKey, TSubItem> collection)
		{
			base.Include(collection);

			if (collection is INotifyCollectionChanged collectionNotifier)
			{
				collectionNotifier.itemChanged += OnItemChanged;
				collectionNotifier.collectionChanged += OnCollectionChanged;
			}
		}

		protected virtual void OnItemChanged()
		{
			itemChanged?.Invoke();
		}

		protected virtual void OnCollectionChanged()
		{
			if (delayCollectionChanged)
			{
				collectionChangedDuringDelay = true;
				return;
			}

			collectionChanged?.Invoke();
		}

		protected void BeginDelayCollectionChanged()
		{
			delayCollectionChanged = true;
			collectionChangedDuringDelay = false;
		}

		protected void EndDelayCollectionChanged()
		{
			delayCollectionChanged = false;

			if (collectionChangedDuringDelay)
			{
				OnCollectionChanged();
				collectionChangedDuringDelay = false;
			}
		}

		public virtual void Add(IEnumerable<TItem> items)
		{
			var _items = items.ToListPooled();
			
			try
			{
				BeginDelayCollectionChanged();

				foreach (var item in _items)
				{
					Add(item);
				}

				EndDelayCollectionChanged();
			}
			finally
			{
				_items.Free();
			}
		}

		public override void Clear()
		{
			BeginDelayCollectionChanged();
			base.Clear();
			EndDelayCollectionChanged();
		}
	}
}