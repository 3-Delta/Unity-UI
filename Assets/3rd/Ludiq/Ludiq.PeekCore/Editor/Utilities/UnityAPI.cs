using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Ludiq.PeekCore
{
	[InitializeOnLoad]
	public static class UnityAPI
	{
		static UnityAPI()
		{
			CreateQueues();

			UnityThread.mainThread = Thread.CurrentThread;
			UnityThread.editorAsync = Async;

			EditorApplicationUtility.onModeChange += CreateQueues;
			EditorApplication.update += Process;
		}

		private static void CreateQueues()
		{
			highPriority = new ConcurrentQueue<Action>();
			lowPriority = new ConcurrentQueue<Action>();
		}

		private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(5);

		private const int targetLowPriorityFPS = 45;

		private static readonly TimeSpan lowPriorityMaxDuration = TimeSpan.FromMilliseconds(1000.0 / targetLowPriorityFPS);

		private static ConcurrentQueue<Action> highPriority;

		private static ConcurrentQueue<Action> lowPriority;

		private static readonly Stopwatch stopwatch = new Stopwatch();
		
		public static void Process()
		{
			stopwatch.Reset();
			stopwatch.Start();

			while (highPriority.TryDequeue(out var action))
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}

			while (stopwatch.Elapsed < lowPriorityMaxDuration && lowPriority.TryDequeue(out var action))
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		public static void Async(Action action)
		{
			Async(action, false);
		}

		public static void Async(Action action, bool prioritize)
		{
			if (UnityThread.allowsAPI)
			{
				action();
				return;
			}

			(prioritize ? highPriority : lowPriority).Enqueue(action);
		}

		public static void Await(Action action)
		{
			Await(action, defaultTimeout);
		}

		public static void AwaitForever(Action action)
		{
			Await(action, null);
		}

		private static void Await(Action action, TimeSpan? timeout)
		{
			if (UnityThread.allowsAPI)
			{
				action();
				return;
			}

			var are = new AutoResetEvent(false);
			ExceptionDispatchInfo exceptionInfo = null;

			highPriority.Enqueue(() =>
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					exceptionInfo = ExceptionDispatchInfo.Capture(ex);
				}
				finally
				{
					are.Set();
				}
			});
			
			if (timeout.HasValue)
			{
				if (!are.WaitOne(timeout.Value))
				{
					throw new TimeoutException("Time-out exceeded on Unity API thread action delegate. Potential deadlock.");
				}
			}
			else
			{
				are.WaitOne();
			}

			exceptionInfo?.Throw();
		}

		public static T Await<T>(Func<T> func)
		{
			return Await(func, defaultTimeout);
		}

		public static T AwaitForever<T>(Func<T> func)
		{
			return Await(func, null);
		}

		public static T Await<T>(Func<T> func, TimeSpan? timeout)
		{
			if (UnityThread.allowsAPI)
			{
				return func();
			}

			var are = new AutoResetEvent(false);
			ExceptionDispatchInfo exceptionInfo = null;

			// Define as object for boxing
			object result = default(T);

			highPriority.Enqueue(() =>
			{
				try
				{
					result = func();
				}
				catch (Exception ex)
				{
					exceptionInfo = ExceptionDispatchInfo.Capture(ex);
				}
				finally
				{
					are.Set();
				}
			});
			
			if (timeout.HasValue)
			{
				if (!are.WaitOne(timeout.Value))
				{
					throw new TimeoutException("Time-out exceeded on Unity API thread function delegate. Potential deadlock.");
				}
			}
			else
			{
				are.WaitOne();
			}

			exceptionInfo?.Throw();
			
			return (T)result;
		}
	}
}