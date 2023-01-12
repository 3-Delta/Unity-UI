#if UNITY_2020_1_OR_NEWER
// We're using the Progress API when available (2020+) instead of the
// legacy BackgroundProgress wrapper, which lets us report each task independently
// in the progress window, instead of flattening them under a single task.
#define USE_PROGRESS_API
#endif

using System;
using System.Reflection;
using System.Threading;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public sealed class BackgroundTaskRunner : ITaskRunner
	{
		public static BackgroundTaskRunner instance { get; } = new BackgroundTaskRunner();

		private static readonly TaskThreadTracker threadTracker = new TaskThreadTracker();

		public bool runsCurrentThread => threadTracker.runsCurrent;

		public void Report(Task task)
		{
#if !USE_PROGRESS_API
			BackgroundProgress.Report(task.currentStepLabel ?? task.title, task.ratio);
#endif
		}

		public void Run(Task task)
		{
			if (UnityThread.isRunningOnMainThread)
			{
				new Thread(() => RunSynchronous(task)).Start();
			}
			else
			{
				RunSynchronous(task);
			}
		}

#if USE_PROGRESS_API
		private void ReportTaskOnEditorUpdate(Task task)
		{
			// Calling Progress.Report as often as we actually report steps seems to freeze
			// the editor, which is counter-productive. We're moving the report to once per
			// editor update instead. 

			if (!Progress.Exists(task.progressItemId))
			{
				return;
			}

			Progress.Report(task.progressItemId, task.ratio, task.currentStepLabel);
		}
#endif

		private void RunSynchronous(Task task)
		{
			threadTracker.Enter();

#if USE_PROGRESS_API
			// TODO: Improve TaskThreadTracker to track the actual task objects per thread,
			// which would allow us to use the progressItemId of the latest task on the current
			// thread as a parentId in Progress.Start.

			task.progressItemId = Progress.Start(task.title, task.currentStepLabel);

			EditorApplication.CallbackFunction reportDelegate = () => ReportTaskOnEditorUpdate(task);

			EditorApplication.update += reportDelegate;
#else
			BackgroundProgress.Report(task.title, 0);
#endif

			task.Begin();

#if USE_PROGRESS_API
			var status = Progress.Status.Running;
#endif

			try
			{
				task.Run();

#if USE_PROGRESS_API
				status = Progress.Status.Succeeded;
#endif
			}
			catch (ThreadAbortException)
			{
#if USE_PROGRESS_API
				status = Progress.Status.Canceled;
#endif
			}
			catch (OperationCanceledException)
			{
#if USE_PROGRESS_API
				status = Progress.Status.Canceled;
#endif
			}
			catch (TargetInvocationException tiex) when (tiex.InnerException is OperationCanceledException ||
			                                             tiex.InnerException is ThreadAbortException)
			{
#if USE_PROGRESS_API
				status = Progress.Status.Canceled;
#endif
			}
			catch
			{
#if USE_PROGRESS_API
				status = Progress.Status.Failed;
#endif

				throw;
			}
			finally
			{
				task.End();

#if USE_PROGRESS_API
				Progress.Finish(task.progressItemId, status);
				task.progressItemId = -1;
				EditorApplication.update -= reportDelegate;
#else
				BackgroundProgress.Clear();
#endif

				threadTracker.Exit();
			}
		}
	}
}