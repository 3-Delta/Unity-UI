using System;
using System.Collections.Generic;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// Unity popups close when out of focus, which makes it impossible
	// to have toggle button that check their open state unless we have a class like this.
	// Internally, Unity does the same thing for every single one of their popups... without reusing code, lol.

	public static class PopupWatcher
	{
		static PopupWatcher()
		{
			EditorApplication.update += Update;
		}

		public static void Watch(EditorWindow window)
		{
			watched.Add(window);
		}

		public static void Release(EditorWindow window)
		{
			if (ReferenceEquals(window, null))
			{
				return;
			}

			watched.Remove(window);
			lastCloseTimes.Remove(window);
		}

		public static bool IsOpenOrJustClosed(EditorWindow window)
		{
			// Actually nothing
			if (ReferenceEquals(window, null))
			{
				return false;
			}

			if (!lastCloseTimes.TryGetValue(window, out var lastCloseTime))
			{
				// Still open
				return true;
			}

			// Just closed
			return DateTime.UtcNow - lastCloseTime < maxDelay;
		}

		public static readonly TimeSpan maxDelay = TimeSpan.FromMilliseconds(50); // Unity uses 50ms as well

		private static readonly HashSet<EditorWindow> watched = new HashSet<EditorWindow>();

		private static readonly Dictionary<EditorWindow, DateTime> lastCloseTimes = new Dictionary<EditorWindow, DateTime>();

		private static void Update()
		{
			var now = DateTime.UtcNow;

			var stillOpen = HashSetPool<EditorWindow>.New();

			foreach (var window in watched)
			{
				if (window.IsDestroyed())
				{
					lastCloseTimes.Add(window, now);
				}
				else
				{
					stillOpen.Add(window);
				}
			}

			watched.IntersectWith(stillOpen);

			stillOpen.Free();
		}
	}
}