using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_3_0), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_3_0 : PluginChangelog
	{
		public Changelog_1_3_0(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.3.0";
		public override DateTime date => new DateTime(2020, 09, 25);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Persistent Pins preference (experimental)";
				yield return "[Optimized] Editor lag due to AnalyzeWindowLayout";
				yield return "[Optimized] Memory pressure due to automatic loading of project assets";
				yield return "[Fixed] Hierarchy ordering of groups and their contents";
				yield return "[Fixed] Missing configuration panels when window was open on load";
				yield return "[Fixed] Preview icon offset in Project folder";
				yield return "[Fixed] Unnecessary instantiation of meshes from mesh filters and renderers";
			}
		}
	}
}