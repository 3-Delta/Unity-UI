using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class GuiCallback
	{
		private static readonly Queue<Action> queue = new Queue<Action>();

		public static void Enqueue(Action action)
		{
			lock (queue)
			{
				queue.Enqueue(action);
			}
		}

		public static void Process()
		{
			if (Event.current == null)
			{
				return;
			}

			lock (queue)
			{
				while (queue.Count > 0)
				{
					try
					{
						queue.Dequeue()?.Invoke();
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
			}
		}
	}
}