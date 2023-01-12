using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ludiq.PeekCore
{
	public static class TaskLinq
	{
		public static bool parallelizeByDefault = false;

		public static void ForEachTask<T>(this IEnumerable<T> items, string title, bool parallelize, Func<T, string> getLabel, Action<T> action)
		{
			var array = items.ToArray();

			if (array.Length == 0)
			{
				return;
			}

			Task.Run(title, array.Length, task =>
			{
				if (parallelize)
				{
					Parallel.ForEach(Partitioner.Create(array), task.parallelOptions, item =>
					{
						task.StartStep(getLabel?.Invoke(item));
						action(item);
						task.CompleteStep();
					});
				}
				else
				{
					foreach (var item in array)
					{
						task.StartStep(getLabel?.Invoke(item));
						action(item);
						task.CompleteStep();
					}
				}
			});
		}

		public static void ForEachTask<T>(this IEnumerable<T> items, string title, Action<T> action)
		{
			items.ForEachTask(title, parallelizeByDefault, null, action);
		}
		
		public static HashSet<TResult> SelectManyTask<TSource, TResult>(this IEnumerable<TSource> items, string title, bool parallelize, Func<TSource, string> getLabel, Func<TSource, IEnumerable<TResult>> selector)
		{
			var results = new HashSet<TResult>();

			var array = items.ToArray();

			if (array.Length == 0)
			{
				return results;
			}

			Task.Run(title, array.Length, task =>
			{
				if (parallelize)
				{
					var parallelResults = new ConcurrentBag<TResult>();
					
					Parallel.ForEach(Partitioner.Create(array), task.parallelOptions, item =>
					{
						task.StartStep(getLabel?.Invoke(item));

						foreach (var selected in selector(item))
						{
							parallelResults.Add(selected);
						}

						task.CompleteStep();
					});
					
					results.UnionWith(parallelResults);
				}
				else
				{
					foreach (var item in array)
					{
						task.AllowCancellation();

						task.StartStep(getLabel?.Invoke(item));

						results.UnionWith(selector(item));

						task.CompleteStep();
					}
				}
			});

			return results;
		}

		public static HashSet<TResult> SelectTask<TSource, TResult>(this IEnumerable<TSource> items, string title, bool parallelize, Func<TSource, string> getLabel, Func<TSource, TResult> selector)
		{
			return items.SelectManyTask(title, parallelize, getLabel, item => selector(item).Yield());
		}

		public static HashSet<T> WhereTask<T>(this IEnumerable<T> items, string title, bool parallelize, Func<T, string> getLabel, Func<T, bool> predicate)
		{
			return items.SelectManyTask(title, parallelize, getLabel, item => predicate(item) ? item.Yield() : Enumerable.Empty<T>());
		}

		public static HashSet<TResult> SelectWhereTask<TSource, TResult>(this IEnumerable<TSource> items, string title, bool parallelize, Func<TSource, string> getLabel, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			return items.SelectManyTask(title, parallelize, getLabel, item => predicate(item) ? selector(item).Yield() : Enumerable.Empty<TResult>());
		}
		
		public static HashSet<TResult> SelectManyTask<TSource, TResult>(this IEnumerable<TSource> items, string title, Func<TSource, IEnumerable<TResult>> selector)
		{
			return items.SelectManyTask(title, parallelizeByDefault, null, selector);
		}

		public static HashSet<TResult> SelectTask<TSource, TResult>(this IEnumerable<TSource> items, string title, Func<TSource, TResult> selector)
		{
			return items.SelectManyTask(title, parallelizeByDefault, null, item => selector(item).Yield());
		}

		public static HashSet<T> WhereTask<T>(this IEnumerable<T> items, string title, Func<T, bool> predicate)
		{
			return items.SelectManyTask(title, parallelizeByDefault, null, item => predicate(item) ? item.Yield() : Enumerable.Empty<T>());
		}

		public static HashSet<TResult> SelectWhereTask<TSource, TResult>(this IEnumerable<TSource> items, string title, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
		{
			return items.SelectManyTask(title, parallelizeByDefault, null, item => predicate(item) ? selector(item).Yield() : Enumerable.Empty<TResult>());
		}
	}
}
