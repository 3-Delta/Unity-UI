using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_5), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_0_5 : PluginChangelog
	{
		public Changelog_1_0_5(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.0.5";
		public override DateTime date => new DateTime(2017, 10, 16);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Missing modular assemblies in 2017.2+";
				yield return "[Added] In-memory deep cloning system";
				yield return "[Optimized] Custom exception handling memory allocation";
				yield return "[Optimized] Custom collections memory allocation";
			}
		}
	}
}