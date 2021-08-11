using System;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class UnityThread
	{
		public static Thread mainThread = Thread.CurrentThread;

		public static bool isRunningOnMainThread => Thread.CurrentThread == mainThread;

		public static void EnsureRunningOnMainThread()
		{
			if (!isRunningOnMainThread)
			{
				throw new NotSupportedException("This operation must be run on the main Unity thread.");
			}
		}

		public static Action<Action> editorAsync;

		public static bool allowsAPI => !Serialization.isUnityDeserializing && isRunningOnMainThread;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInitialize()
		{
			mainThread = Thread.CurrentThread;
		}

		public static void EditorAsync(Action action)
		{
			editorAsync?.Invoke(action);
		}
	}
}