using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_1), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_0_1 : PluginChangelog
	{
		public Changelog_1_0_1(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.0.1";
		public override DateTime date => new DateTime(2019, 08, 19);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Shortcuts mapping adding unwanted Shift modifier";
				yield return"[Fixed] Creator menu exception when multiple assets point to the same object";
				yield return"[Fixed] Errors when hierarchy popup was open during assembly reload";
				yield return"[Optimized] Duplicate tabs if layout had multiple of the same window open";
				yield return"[Fixed] Errors when initializing settings providers (attempted)";
			}
		}
	}
}