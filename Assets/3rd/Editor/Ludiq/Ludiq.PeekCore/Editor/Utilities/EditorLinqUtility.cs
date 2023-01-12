using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Ludiq.PeekCore
{
	public static class EditorLinqUtility
	{
		public static IEnumerable<T> Cancellable<T>(this IEnumerable<T> source, CancellationToken cancellation)
		{
			foreach (var item in source)
			{
				yield return item;

				cancellation.ThrowIfCancellationRequested();
			}
		}

		public static IEnumerable<T> Cancellable<T>(this IEnumerable<T> source, CancellationToken cancellation, Action cancel)
		{
			Ensure.That(nameof(cancel)).IsNotNull(cancel);

			foreach (var item in source)
			{
				yield return item;

				if (cancellation.IsCancellationRequested)
				{
					cancel();
				}
			}
		}

		public static void UnionWith<T>(this ConcurrentBag<T> bag, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				bag.Add(item);
			}
		}
	}
}
