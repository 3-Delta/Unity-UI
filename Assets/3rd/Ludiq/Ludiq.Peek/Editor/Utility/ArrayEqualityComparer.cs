using System.Collections.Generic;

namespace Ludiq
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	// Adapted from: https://stackoverflow.com/a/4598406
	public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
	{
		public static IEqualityComparer<T[]> Default { get; } = new ArrayEqualityComparer<T>();

		private readonly IEqualityComparer<T> elementComparer;

		public ArrayEqualityComparer() : this(EqualityComparer<T>.Default) { }

		public ArrayEqualityComparer(IEqualityComparer<T> elementComparer)
		{
			this.elementComparer = elementComparer;
		}

		public bool Equals(T[] x, T[] y)
		{
			if (x == y)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			if (x.Length != y.Length)
			{
				return false;
			}

			for (var i = 0; i < x.Length; i++)
			{
				if (!elementComparer.Equals(x[i], y[i]))
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(T[] array)
		{
			if (array == null)
			{
				return 0;
			}

			var hash = 17;

			foreach (var item in array)
			{
				hash = (hash * 23) + elementComparer.GetHashCode(item);
			}

			return hash;
		}
	}
}