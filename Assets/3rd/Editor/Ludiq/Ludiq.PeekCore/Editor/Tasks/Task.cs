using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Ludiq.PeekCore
{
	public sealed class Task
	{
		public ITaskRunner runner { get; }

		public string title { get; }

		public Func<EditorTexture> getIcon { get; }

		public EditorTexture icon { get; private set; }

		public int totalSteps { get; }
		
		public int completedSteps { get; private set; }

		public float ratio => (float)completedSteps / totalSteps;

		public float animatedRatio;

		public string currentStepLabel { get; set; }
		
		public bool stepsHaveStarted { get; private set; }

		public bool stepsHaveCompleted => completedSteps == totalSteps;

		private readonly object @lock = new object();
		
		private ManualResetEvent stepsWaitHandle;

		private Stopwatch stopwatch;

		public TimeSpan elapsed => stopwatch.Elapsed;

		private readonly Action<Task> work;

		private CancellationTokenSource cancellation { get; }

		private CancellationToken cancellationToken { get; }

#if UNITY_2020_1_OR_NEWER
		public int progressItemId { get; set; }
#endif

		public Task(ITaskRunner runner, string title, int totalSteps, Action<Task> work)
		{
			Ensure.That(nameof(runner)).IsNotNull(runner);
			Ensure.That(nameof(title)).IsNotNull(title);
			Ensure.That(nameof(totalSteps)).IsGt(totalSteps, 0);
			Ensure.That(nameof(work)).IsNotNull(work);

			this.title = title;
			this.totalSteps = totalSteps;
			this.runner = runner;
			this.work = work;

			cancellation = new CancellationTokenSource();
			cancellationToken = cancellation.Token;
		}

		public void Run()
		{
			work(this);
		}

		public void Begin()
		{
			stopwatch = Stopwatch.StartNew();
			ProfilingUtility.BeginSample(title);
		}

		public void End()
		{
			stopwatch.Stop();
			cancellation.Dispose();
			ProfilingUtility.EndSample();
		}

		public void Cancel()
		{
			cancellation.Cancel();

			stepsWaitHandle?.Set();
		}

		public ParallelOptions parallelOptions => new ParallelOptions() {CancellationToken = cancellationToken};

		public void AllowCancellation()
		{
			cancellationToken.ThrowIfCancellationRequested();
		}

		public void StartStep(string label = null)
		{
			lock (@lock)
			{
				AllowCancellation();

				if (stepsHaveCompleted)
				{
					throw new ArgumentOutOfRangeException($"Over-starting progress task: '{title}'.");
				}
				
				currentStepLabel = label;
				stepsHaveStarted = true;
				runner.Report(this);
			}
		}

		public void CompleteStep()
		{
			lock (@lock)
			{
				if (stepsHaveCompleted)
				{
					throw new ArgumentOutOfRangeException($"Over-completing progress task: '{title}'.");
				}
			
				completedSteps++;
				currentStepLabel = null;
				runner.Report(this);
				
				AllowCancellation();

				if (stepsHaveCompleted)
				{
					stepsWaitHandle?.Set();
				}
			}
		}

		public void WaitUntilStepsHaveCompleted()
		{
			if (stepsWaitHandle == null)
			{
				stepsWaitHandle = new ManualResetEvent(false);
			}

			if (!stepsHaveCompleted)
			{
				stepsWaitHandle.WaitOne();
			}

			AllowCancellation();
		}

		private static IEnumerable<ITaskRunner> runners
		{
			get
			{
				yield return ForegroundTaskRunner.instance;
				yield return BackgroundTaskRunner.instance;
				yield return WindowTaskRunner.instance;
			}
		}

		public static bool allowWindowRunner { get; set; } = true;

		private static ITaskRunner ChooseRunner()
		{
			foreach (var runner in runners)
			{
				if (runner.runsCurrentThread)
				{
					return runner;
				}
			}

			if (UnityThread.isRunningOnMainThread || TaskWindow.instance != null)
			{
				if (allowWindowRunner)
				{
					return WindowTaskRunner.instance;
				}
				else
				{
					return ForegroundTaskRunner.instance;
				}
			}
			else
			{
				return BackgroundTaskRunner.instance;
			}
		}

		public static void Run(string title, int steps, Action<Task> work)
		{
			var runner = ChooseRunner();

			var task = new Task(runner, title, steps, work);
			
			runner.Run(task);
		}
	}
}
