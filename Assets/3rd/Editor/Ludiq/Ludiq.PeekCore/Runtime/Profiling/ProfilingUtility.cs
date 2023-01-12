using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine.Profiling;

namespace Ludiq.PeekCore
{
	public static class ProfilingUtility
	{
		public const string ConditionalDefine = "ENABLE_PROFILER_LUDIQ";

		private static readonly object @lock = new object();
		
		private static readonly Dictionary<Thread, ProfiledSegment> rootSegments = new Dictionary<Thread, ProfiledSegment>();
		private static readonly Dictionary<Thread, ProfiledSegment> currentSegments = new Dictionary<Thread, ProfiledSegment>();

		public static Dictionary<Thread, ProfiledSegment> allRootSegments
		{
			get
			{
				lock (@lock)
				{
					return rootSegments.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				}
			}
		}

		public static ProfiledSegment rootSegment
		{
			get
			{
				lock (@lock)
				{
					if (!rootSegments.TryGetValue(Thread.CurrentThread, out var rootSegment))
					{
						rootSegment = new ProfiledSegment(null, "Root");
						rootSegments.Add(Thread.CurrentThread, rootSegment);
					}

					return rootSegment;
				}
			}
		}

		public static ProfiledSegment currentSegment
		{
			get
			{
				lock (@lock)
				{
					if (!currentSegments.TryGetValue(Thread.CurrentThread, out var currentSegment))
					{
						currentSegment = rootSegment;
						currentSegments.Add(Thread.CurrentThread, currentSegment);
					}

					return currentSegment;
				}
			}
			set
			{
				lock (@lock)
				{
					if (currentSegments.ContainsKey(Thread.CurrentThread))
					{
						currentSegments[Thread.CurrentThread] = value;
					}
					else
					{
						currentSegments.Add(Thread.CurrentThread, value);
					}
				}
			}
		}

		[Conditional(ConditionalDefine)]
		public static void Clear()
		{
			lock (@lock)
			{
				rootSegments.Clear();
				currentSegments.Clear();
			}
		}

		[Conditional(ConditionalDefine)]
		public static void ClearThisThread()
		{
			lock (@lock)
			{
				var thread = Thread.CurrentThread;
				rootSegments.Remove(thread);
				currentSegments.Remove(thread);
			}
		}
		
		public static ProfilingScope SampleBlock(string name)
		{
			return new ProfilingScope(name);
		}

		[Conditional(ConditionalDefine)]
		public static void BeginSample(string name)
		{
			if (!currentSegment.children.Contains(name))
			{
				currentSegment.children.Add(new ProfiledSegment(currentSegment, name));
			}

			currentSegment = currentSegment.children[name];
			currentSegment.calls++;
			currentSegment.stopwatch.Start();

			if (UnityThread.allowsAPI)
			{
				Profiler.BeginSample(name);
			}
		}

		[Conditional(ConditionalDefine)]
		public static void EndSample()
		{
			currentSegment.stopwatch.Stop();

			if (currentSegment.parent != null)
			{
				currentSegment = currentSegment.parent;
			}

			if (UnityThread.allowsAPI)
			{
				Profiler.EndSample();
			}
		}
	}
}