using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_8), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_8 : PluginChangelog
	{
		public Changelog_1_1_8(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.8";
		public override DateTime date => new DateTime(2020, 04, 20);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Compatibility with Unity 2019.3.10+ and Unity 2020+";
			}
		}
	}
}