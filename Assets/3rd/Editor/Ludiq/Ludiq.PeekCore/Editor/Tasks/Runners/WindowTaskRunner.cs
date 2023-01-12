using System;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class WindowTaskRunner : ITaskRunner
	{
		public static WindowTaskRunner instance { get; } = new WindowTaskRunner();
		
		private static readonly TaskThreadTracker threadTracker = new TaskThreadTracker();

		public bool runsCurrentThread => threadTracker.runsCurrent;

		private static readonly object @lock = new object();

		private TaskWindow window
		{
			get => TaskWindow.instance;
			set => TaskWindow.instance = value;
		}

		public void Run(Task task)
		{
			if (UnityThread.isRunningOnMainThread)
			{
				if (window == null)
				{
					window = ScriptableObject.CreateInstance<TaskWindow>();
					window.titleContent = new GUIContent(task.title);
				}

				new Thread(() => RunSynchronous(task)).Start();
				
				window.ShowModal();
			}
			else
			{
				if (window == null)
				{
					throw new NotSupportedException("Must be on the main thread to run a root window task.");
				}
				
				RunSynchronous(task);
			}
		}

		private void RunSynchronous(Task task)
		{
			threadTracker.Enter();

			lock (@lock)
			{
				window.tasks.Add(task);
			}

			task.Begin();

			try
			{
				task.Run();
			}
			catch (ThreadAbortException) { }
			catch (OperationCanceledException) { }
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				task.End();

				lock (@lock)
				{
					if (window != null)
					{
						window.tasks.Remove(task);

						if (window.tasks.Count == 0)
						{
							window.Close();
						}
					}
				}

				threadTracker.Exit();
			}
		}

		public void Report(Task task)
		{

		}
	}
}
