using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_3), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_3 : PluginChangelog
	{
		public Changelog_1_1_3(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.3";
		public override DateTime date => new DateTime(2019, 09, 26);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Custom window icons not working in tabs";
				yield return "[Fixed] Removed forgotten debug log statement when opening tabs";
			}
		}
	}
}