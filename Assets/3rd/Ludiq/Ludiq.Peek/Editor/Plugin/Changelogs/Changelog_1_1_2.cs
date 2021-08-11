using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_2), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_2 : PluginChangelog
	{
		public Changelog_1_1_2(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.2";
		public override DateTime date => new DateTime(2019, 09, 25);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Tabs persistence when entering play mode";
				yield return "[Added] Editor preference for default editor popup width";
			}
		}
	}
}