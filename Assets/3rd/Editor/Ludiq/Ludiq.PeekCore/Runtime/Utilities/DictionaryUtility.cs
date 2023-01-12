using System.Collections;
using System.Collections.Generic;

namespace Ludiq.PeekCore
{
	public static class DictionaryUtility
	{
		public static IDictionary Merge(this IDictionary destination, IDictionary source)
		{
			var sourceEnumerator = source.GetEnumerator();

			while (sourceEnumerator.MoveNext())
			{
				if (!destination.Contains(sourceEnumerator.Key))
				{
					destination.Add(sourceEnumerator.Key, sourceEnumerator.Value);
				}
			}

			return destination;
		}

		public static IDictionary Merge(this IDictionary destination, params IDictionary[] sources)
		{
			foreach (var source in sources)
			{
				destination.Merge(source);
			}

			return destination;
		}

		public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			dictionary.TryGetValue(key, out var value);
			return value;
		}

		// TODO: Report, there seems to be a Unity bug that fails to find IROD in .NET 2.0 standard but only on the first compile 
		/*
		public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
		{
			dictionary.TryGetValue(key, out var value);
			return value;
		}*/

		public static TValue GetValueOrDefault<TKey, TValue>(this IKeyedCollection<TKey, TValue> dictionary, TKey key)
		{
			dictionary.TryGetValue(key, out var value);
			return value;
		}
	}
}