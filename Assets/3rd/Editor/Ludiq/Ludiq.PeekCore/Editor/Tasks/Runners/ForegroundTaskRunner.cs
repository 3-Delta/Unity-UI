using System;
using System.Threading;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public sealed class ForegroundTaskRunner : ITaskRunner
	{
		public static ForegroundTaskRunner instance { get; } = new ForegroundTaskRunner();

		public bool runsCurrentThread { get; private set; }

		public void Run(Task task)
		{
			UnityThread.EnsureRunningOnMainThread();
		
			EditorUtility.DisplayProgressBar(task.title, null, 0);
			
			task.Begin();
			runsCurrentThread = true;

			try 
			{
				task.Run();
			}
			catch (ThreadAbortException) { }
			catch (OperationCanceledException) { }
			finally
			{
				task.End();
				EditorUtility.ClearProgressBar();
				runsCurrentThread = false;
			}
		}

		public void Report(Task task)
		{
			if (EditorUtility.DisplayCancelableProgressBar(task.title, task.currentStepLabel, task.ratio))
			{
				task.Cancel();
			}
		}
	}
}