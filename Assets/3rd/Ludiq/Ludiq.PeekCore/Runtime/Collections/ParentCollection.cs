using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Ludiq.PeekCore
{
	// Requirements:
	// - Assign parent references to child
	// - Notify external watchers of collection change events
	// - Notify internal items of collection insertion process
	// - Notify external watchers of item property changes
	// - Allow another parent collection to raise events manually, in case this needs to be nested
	public abstract class ParentCollection<TParent, TKey, TItem> :
		KeyedCollection<TKey, TItem>,
		INotifyCollectionChanged,
		INotifiableCollection<TItem>,
		IBulkCollection<TItem>
		where TParent : class
	{
		public TParent parent { get; }

		protected ParentCollection(TParent parent, bool notifyItems = true)
		{
			Ensure.That(nameof(parent)).IsNotNull(parent);
			this.parent = parent;
			this.notifyItems = notifyItems;
		}

		public event Action itemChanged;

		public event Action collectionChanged;

		protected virtual void BeforeSet(TItem previous, TItem item)
		{
			BeforeRemove(previous);
			BeforeAdd(item);
		}

		protected virtual void AfterSet(TItem previous, TItem item)
		{
			AfterRemove(previous);
			AfterAdd(item);
		}

		protected virtual void BeforeAdd(TItem item)
		{
			if (item is ICollectionChild<TParent> child)
			{
				if (child.parent != null)
				{
					if (child.parent == parent)
					{
						throw new InvalidOperationException($"{typeof(TItem).Name} items cannot be added multiple times into the same parent {typeof(TParent).Name}.");
					}
					else
					{
						throw new InvalidOperationException($"{typeof(TItem).Name} items cannot be shared across multiple parent {typeof(TParent).Name}.");
					}
				}

				child.parent = parent;

				if (item is INotifiedCollectionChild<TParent> notified)
				{
					notified.BeforeAdd(parent);
				}
			}
		}

		protected virtual void AfterAdd(TItem item)
		{
			if (item is INotifiedCollectionChild<TParent> notified)
			{
				notified.AfterAdd(parent);
			}

			if (item is INotifyChanged notifier)
			{
				notifier.changedExternally += OnItemChanged;
			}
		}

		protected virtual void BeforeRemove(TItem item)
		{
			if (item is INotifyChanged notifier)
			{
				notifier.changedExternally += OnItemChanged;
			}

			if (item is INotifiedCollectionChild<TParent> notified)
			{
				notified.BeforeRemove(parent);
			}
		}

		protected virtual void AfterRemove(TItem item)
		{
			if (item is ICollectionChild<TParent> child)
			{
				child.parent = null;

				if (item is INotifiedCollectionChild<TParent> notified)
				{
					notified.AfterRemove(parent);
				}
			}
		}

		protected sealed override void InsertItem(int index, TItem item)
		{
			Ensure.That(nameof(item)).IsNotNull(item);

			if (notifyItems)
			{
				BeforeAdd(item);
			}

			base.InsertItem(index, item);

			if (notifyItems)
			{
				AfterAdd(item);
			}

			OnCollectionChanged();
		}

		protected sealed override void SetItem(int index, TItem item)
		{
			Ensure.That(nameof(item)).IsNotNull(item);

			var previous = this[index];

			if (notifyItems)
			{
				BeforeSet(previous, item);
			}

			base.SetItem(index, item);

			if (notifyItems)
			{
				AfterSet(previous, item);
			}

			OnCollectionChanged();
		}

		protected sealed override void ClearItems()
		{
			var items = this.ToListPooled();

			try
			{
				foreach (var item in items)
				{
					if (notifyItems)
					{
						BeforeRemove(item);
					}

					var index = IndexOf(item);

					base.RemoveItem(index);

					if (notifyItems)
					{
						AfterRemove(item);
					}
				}

				OnCollectionChanged();
			}
			finally
			{
				items.Free();
			}
		}

		protected sealed override void RemoveItem(int index)
		{
			var item = this[index];

			if (notifyItems)
			{
				BeforeRemove(item);
			}

			base.RemoveItem(index);

			if (notifyItems)
			{
				AfterRemove(item);
			}

			OnCollectionChanged();
		}

		public void Add(IEnumerable<TItem> items)
		{
			FillItems(items);
		}

		protected void FillItems(IEnumerable<TItem> items)
		{
			var _items = items.ToListPooled();

			try
			{
				foreach (var item in _items)
				{
					if (notifyItems)
					{
						BeforeAdd(item);
					}

					base.InsertItem(Count, item);

					if (notifyItems)
					{
						AfterAdd(item);
					}
				}
			}
			finally
			{
				_items.Free();
			}

			OnCollectionChanged();
		}

		protected virtual void OnItemChanged()
		{
			itemChanged?.Invoke();
		}

		protected virtual void OnCollectionChanged()
		{
			collectionChanged?.Invoke();
		}



		#region INotifiableCollection

		protected bool notifyItems { get; }

		protected void EnsureNotNotifyingItems()
		{
			if (notifyItems)
			{
				throw new InvalidOperationException("Item events are already being sent automatically for this collection.");
			}
		}

		public void RaiseBeforeAdd(TItem item)
		{
			EnsureNotNotifyingItems();
			BeforeAdd(item);
		}

		public void RaiseAfterAdd(TItem item)
		{
			EnsureNotNotifyingItems();
			AfterAdd(item);
		}

		public void RaiseBeforeRemove(TItem item)
		{
			EnsureNotNotifyingItems();
			BeforeRemove(item);
		}

		public void RaiseAfterRemove(TItem item)
		{
			EnsureNotNotifyingItems();
			AfterRemove(item);
		}

		public void RaiseBeforeSet(TItem previous, TItem item)
		{
			EnsureNotNotifyingItems();
			BeforeSet(previous, item);
		}

		public void RaiseAfterSet(TItem previous, TItem item)
		{
			EnsureNotNotifyingItems();
			AfterSet(previous, item);
		}

		#endregion
	}
}