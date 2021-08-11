using System.Collections.Generic;
using System.Threading;

namespace Ludiq.PeekCore
{
	public sealed class TaskThreadTracker
	{
		private readonly Dictionary<Thread, int> threads = new Dictionary<Thread, int>();

		public void Enter()
		{
			var thread = Thread.CurrentThread;

			lock (threads)
			{
				if (!threads.ContainsKey(thread))
				{
					threads.Add(thread, 0);
				}

				threads[thread]++;
			}
		}

		public void Exit()
		{
			var thread = Thread.CurrentThread;

			lock (threads)
			{
				if (!threads.ContainsKey(thread))
				{
					throw new InvalidImplementationException("Exiting a thread that was never entered.");
				}

				if (--threads[thread] == 0)
				{
					threads.Remove(thread);
				}
			}
		}

		public bool Runs(Thread thread)
		{
			lock (threads)
			{
				return threads.ContainsKey(thread);
			}
		}

		public bool runsCurrent => Runs(Thread.CurrentThread);
	}
}
