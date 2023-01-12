using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class GUIContentPool
	{
		private static readonly object @lock = new object();

		private static readonly Stack<GUIContent> free = new Stack<GUIContent>();

		private static readonly HashSet<GUIContent> busy = new HashSet<GUIContent>();

		public static GUIContent New()
		{
			lock (@lock)
			{
				if (free.Count == 0)
				{
					free.Push(new GUIContent());
				}

				var content = free.Pop();

				busy.Add(content);

				return content;
			}
		}

		public static void Free(GUIContent content)
		{
			lock (@lock)
			{
				if (!busy.Contains(content))
				{
					throw new ArgumentException("The GUI Content to free is not in use by the pool.", nameof(content));
				}

				content.text = null;
				content.image = null;
				content.tooltip = null;

				busy.Remove(content);

				free.Push(content);
			}
		}
	}
}