using System;
using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public sealed class LazyDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> items = new Dictionary<TKey, TValue>();

		private readonly Dictionary<TKey, Func<TValue>> factories = new Dictionary<TKey, Func<TValue>>();

		private readonly Func<TKey, TValue> defaultFactory;

		public LazyDictionary()
		{

		}

		public LazyDictionary(Func<TKey, TValue> defaultFactory) : this()
		{
			this.defaultFactory = defaultFactory;
		}

		public TValue this[TKey key]
		{
			get
			{
				if (!items.TryGetValue(key, out var item))
				{
					if (factories.TryGetValue(key, out var factory))
					{
						item = factory();
						items[key] = item;
					}
					else if (defaultFactory != null)
					{
						item = defaultFactory(key);
						items[key] = item;
					}
					else
					{
						throw new InvalidOperationException($"Trying to fetch an item from a lazy storage without a factory: '{key}'.");
					}
				}

				return item;
			}
		}

		public void Bind(TKey key, Func<TValue> factory)
		{
			Ensure.That(nameof(key)).IsNotNull(key);
			Ensure.That(nameof(factory)).IsNotNull(factory);

			factories[key] = factory;
		}

		public void Bind(TKey key, TValue item)
		{
			Ensure.That(nameof(key)).IsNotNull(key);

			items[key] = item;
		}
	}
}
