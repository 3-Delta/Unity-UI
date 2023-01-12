using System;
using System.Collections.Generic;
using Ludiq.PeekCore;
using Random = UnityEngine.Random;

[assembly: InitializeAfterPlugins(typeof(Tips))]

namespace Ludiq.PeekCore
{
	public static class Tips
	{
		static Tips()
		{
			foreach (var plugin in PluginContainer.plugins)
			{
				Add(plugin.tips);
			}
		}
		
		private static readonly List<string> tips = new List<string>();

		public static IEnumerable<string> all => tips;

		public static bool any => count > 0;

		public static int count => tips.Count;

		public static string random => tips[Random.Range(0, tips.Count)];

		public static int wordsPerMinute { get; set; } = 120;

		public static void Add(string tip)
		{
			tips.Add(tip);
		}

		public static void Add(IEnumerable<string> tips)
		{
			Tips.tips.AddRange(tips);
		}

		public static TimeSpan TimeToRead(string tip)
		{
			var words = tip.Split(' ').Length;
			var minutes = (double)words / wordsPerMinute;
			return TimeSpan.FromMinutes(minutes);
		}
	}
}
