using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_1), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_1 : PluginChangelog
	{
		public Changelog_1_1_1(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.1";
		public override DateTime date => new DateTime(2019, 08, 29);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] SceneVisibilityManager errors in Unity 2019.1";
				yield return "[Fixed] ProBuilder errors when integration setup hadn't been done";
			}
		}
	}
}