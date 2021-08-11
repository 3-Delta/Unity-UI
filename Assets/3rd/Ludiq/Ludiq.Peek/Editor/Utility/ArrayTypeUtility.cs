using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class ArrayTypeUtility
	{
		public static bool TryGetCommonType<T>(IEnumerable<T> enumerable, out Type commonType)
		{
			commonType = null;

			foreach (var item in enumerable)
			{
				if (item == null)
				{
					continue;
				}

				var type = item.GetType();

				if (commonType == null)
				{
					commonType = type;
				}
				else if (commonType != type)
				{
					return false;
				}
			}

			if (commonType == null)
			{
				commonType = typeof(T);
			}

			return true;
		}

		public static Type GetCommonType<T>(IEnumerable<T> enumerable)
		{
			if (!TryGetCommonType(enumerable, out var commonType))
			{
				commonType = typeof(T);
			}

			return commonType;
		}

		public static Array RetypeArray<T>(T[] source)
		{
			source = source.NotNull().ToArray();
			var result = Array.CreateInstance(GetCommonType(source), source.Length);
			source.CopyTo(result, 0);
			return result;
		}
	}
}