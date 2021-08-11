using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class LinqUtility
	{
		public static IEnumerable<T> OfType<T>(this IEnumerable<T> source, Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			return source.Where(o => type.IsInstanceOfTypeNullable(o));
		}

		public static bool None<T>(this IEnumerable<T> source)
		{
			return !source.Any();
		}
		
		public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return !source.Any(predicate);
		}
		
		public static bool NotAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
		{
			return !source.All(predicate);
		}

		public static bool Predicate<T>(this IEnumerable<T> source, Func<T, bool> predicate, SequencePredicate predicateType)
		{
			switch (predicateType)
			{
				case SequencePredicate.Any: return source.Any(predicate);
				case SequencePredicate.All: return source.All(predicate);
				case SequencePredicate.None: return source.None(predicate);
				case SequencePredicate.NotAll: return source.NotAll(predicate);
				default: throw predicateType.Unexpected();
			}
		}

		public static IEnumerable<KeyValuePair<T, int>> ToValueIndexPairs<T>(this IEnumerable<T> items)
		{
			int index = 0;

			foreach (var item in items)
			{
				yield return new KeyValuePair<T, int>(item, index++);
			}
		}

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items, bool throwOnDuplicates = true)
		{
			var result = new Dictionary<TKey, TValue>();

			if (throwOnDuplicates)
			{
				foreach (var item in items)
				{
					result.Add(item.Key, item.Value);
				}
			}
			else
			{
				foreach (var item in items)
				{
					result[item.Key] = item.Value;
				}
			}

			return result;
		}

		public static IEnumerable<T> Concat<T>(params IEnumerable[] enumerables)
		{
			foreach (var enumerable in enumerables.NotNull())
			{
				foreach (var item in enumerable.OfType<T>())
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
		{
			return items.GroupBy(property).Select(x => x.First());
		}

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return Enumerable.Empty<T>();
			}

			return enumerable.Where(i => i != null);
		}

		public static IEnumerable<T> Yield<T>(this T t)
		{
			yield return t;
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
		{
			return new HashSet<T>(enumerable);
		}

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
		}

		public static void AddRange(this IList list, IEnumerable items)
		{
			foreach (var item in items)
			{
				list.Add(item);
			}
		}

		// NETUP: Replace with IReadOnlyCollection, IReadOnlyList

		public static ICollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable is ICollection<T>)
			{
				return (ICollection<T>)enumerable;
			}
			else
			{
				return enumerable.ToList().AsReadOnly();
			}
		}

		public static IList<T> AsReadOnlyList<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable is IList<T>)
			{
				return (IList<T>)enumerable;
			}
			else
			{
				return enumerable.ToList().AsReadOnly();
			}
		}

		public static IEnumerable<T> Flatten<T>
		(
			this IEnumerable<T> source,
			Func<T, IEnumerable<T>> childrenSelector
		)
		{
			var flattenedList = source;

			foreach (var element in source)
			{
				flattenedList = flattenedList.Concat(childrenSelector(element).Flatten(childrenSelector));
			}

			return flattenedList;
		}

		public static IEnumerable<T> IntersectAll<T>(this IEnumerable<IEnumerable<T>> groups)
		{
			HashSet<T> hashSet = null;

			foreach (var group in groups)
			{
				if (hashSet == null)
				{
					hashSet = new HashSet<T>(group);
				}
				else
				{
					hashSet.IntersectWith(group);
				}
			}

			return hashSet == null ? Enumerable.Empty<T>() : hashSet.AsEnumerable();
		}

		public static IEnumerable<T> OrderByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, bool throwOnCycle = true)
		{
			var sorted = new List<T>();
			var visited = HashSetPool<T>.New();

			foreach (var item in source)
			{
				OrderByDependenciesVisit(item, visited, sorted, getDependencies, throwOnCycle);
			}

			HashSetPool<T>.Free(visited);

			return sorted;
		}

		private static void OrderByDependenciesVisit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> getDependencies, bool throwOnCycle)
		{
			if (!visited.Contains(item))
			{
				visited.Add(item);

				foreach (var dependency in getDependencies(item))
				{
					OrderByDependenciesVisit(dependency, visited, sorted, getDependencies, throwOnCycle);
				}

				sorted.Add(item);
			}
			else
			{
				if (throwOnCycle && !sorted.Contains(item))
				{
					throw new InvalidOperationException("Cyclic dependency.");
				}
			}
		}

		public static IEnumerable<T> OrderByDependers<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependers, bool throwOnCycle = true)
		{
			// TODO: Optimize, or use another algorithm (Kahn's?)

			// Convert dependers to dependencies
			var dependencies = new Dictionary<T, HashSet<T>>();

			foreach (var dependency in source)
			{
				foreach (var depender in getDependers(dependency))
				{
					if (!dependencies.ContainsKey(depender))
					{
						dependencies.Add(depender, new HashSet<T>());
					}

					dependencies[depender].Add(dependency);
				}
			}

			return source.OrderByDependencies(depender =>
			{
				if (dependencies.ContainsKey(depender))
				{
					return dependencies[depender];
				}
				else
				{
					return Enumerable.Empty<T>();
				}
			}, throwOnCycle);
		}

		public static IEnumerable<T> Catch<T>(this IEnumerable<T> source, Action<Exception> @catch)
		{
			Ensure.That(nameof(source)).IsNotNull(source);

			using (var enumerator = source.GetEnumerator())
			{
				bool success;

				do
				{
					try
					{
						success = enumerator.MoveNext();
					}
					catch (OperationCanceledException)
					{
						yield break;
					}
					catch (Exception ex)
					{
						@catch?.Invoke(ex);
						success = false;
					}

					if (success)
					{
						yield return enumerator.Current;
					}
				}
				while (success);
			}
		}

		public static IEnumerable<T> Catch<T>(this IEnumerable<T> source, ICollection<Exception> exceptions)
		{
			Ensure.That(nameof(exceptions)).IsNotNull(exceptions);

			return source.Catch(exceptions.Add);
		}

		public static IEnumerable<T> CatchAsLogError<T>(this IEnumerable<T> source, string message)
		{
			return source.Catch((ex) => Debug.LogError(message + "\n" + ex.ToString()));
		}

		public static IEnumerable<T> CatchAsLogWarning<T>(this IEnumerable<T> source, string message)
		{
			return source.Catch((ex) => Debug.LogWarning(message + "\n" + ex.ToString()));
		}

		public static List<List<T>> Chunk<T>(this IList<T> collection, int chunkCount)
		{
			var chunks = new List<List<T>>();
			var chunkSize = collection.Count / chunkCount;
			var chunk = new List<T>();
			chunks.Add(chunk);

			foreach (var item in collection)
			{
				if (chunk.Count > chunkSize)
				{
					chunk = new List<T>();
					chunks.Add(chunk);
				}

				chunk.Add(item);
			}
			
			return chunks;
		}

		public static IEnumerable<TResult> ZipLongest<TFirst, TSecond, TResult>(
			this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector,
			TFirst firstPlaceholder = default(TFirst), TSecond secondPlaceholder = default(TSecond))
		{
			using (var firstEnumerator = first.GetEnumerator())
			using (var secondEnumerator = second.GetEnumerator())
			{
				bool firstSuccess;
				bool secondSuccess;

				while (true)
				{
					firstSuccess = firstEnumerator.MoveNext();
					secondSuccess = secondEnumerator.MoveNext();

					if (firstSuccess && secondSuccess)
					{
						yield return resultSelector(firstEnumerator.Current, secondEnumerator.Current);
					}
					else
					{
						break;
					}
				}

				if (firstSuccess)
				{
					do
					{
						yield return resultSelector(firstEnumerator.Current, secondPlaceholder);
					} while (firstEnumerator.MoveNext());
				}
				else if (secondSuccess)
				{
					do
					{
						yield return resultSelector(firstPlaceholder, secondEnumerator.Current);
					} while (secondEnumerator.MoveNext());
				}
			}
		}

		public static bool AllZipped<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> predicate)
		{
			return first.Zip(second, (a, b) => new KeyValuePair<TFirst, TSecond>(a, b)).All(pair => predicate(pair.Key, pair.Value));
		}

		public static bool AnyZipped<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> predicate)
		{
			return first.Zip(second, (a, b) => new KeyValuePair<TFirst, TSecond>(a, b)).Any(pair => predicate(pair.Key, pair.Value));
		}

		public static bool AllZippedLongest<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> predicate)
		{
			return first.ZipLongest(second, (a, b) => new KeyValuePair<TFirst, TSecond>(a, b)).All(pair => predicate(pair.Key, pair.Value));
		}

		public static bool AnyZippedLongest<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> predicate)
		{
			return first.ZipLongest(second, (a, b) => new KeyValuePair<TFirst, TSecond>(a, b)).Any(pair => predicate(pair.Key, pair.Value));
		}
	}
}
