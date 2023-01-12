using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: InitializeAfterPlugins(typeof(BackgroundWorker))]

namespace Ludiq.PeekCore
{
	public static class BackgroundWorker
	{
		static BackgroundWorker()
		{
			queue = new Queue<Action>();

			foreach (var registration in Codebase.GetTypeRegistrations<RegisterBackgroundWorkerAttribute>())
			{
				var backgroundWorkMethod = registration.type.GetMethod(registration.methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

				if (backgroundWorkMethod != null)
				{
					tasks += () => backgroundWorkMethod.Invoke(null, new object[0]);
				}
				else
				{
					Debug.LogWarningFormat($"Missing '{registration.methodName}' method for '{registration.type}' background worker.");
				}
			}

			EditorApplication.delayCall += delegate { new Thread(Work) { Name = "Background Worker" }.Start(); };
		}

		private static readonly Queue<Action> queue;

		public static event Action tasks
		{
			add { Schedule(value); }
			remove { }
		}

		public static void Schedule(Action action)
		{
			lock (queue)
			{
				queue.Enqueue(action);
			}
		}

		private static void Work()
		{
			while (true)
			{
				Action task = null;
				var remaining = 0;

				lock (queue)
				{
					if (queue.Count > 0)
					{
						remaining = queue.Count;
						task = queue.Dequeue();
					}
				}

				if (task != null)
				{
					BackgroundProgress.Report($"{remaining} task{(queue.Count > 1 ? "s" : "")} remaining...", 0);

					try
					{
						task();
					}
					catch (Exception ex)
					{
						EditorApplication.delayCall += () => Debug.LogException(ex);
					}
					finally
					{
						BackgroundProgress.Clear();
					}
				}
				else
				{
					Thread.Sleep(100);
				}
			}
		}
	}
}