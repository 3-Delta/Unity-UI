using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_0), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_0_0 : PluginChangelog
	{
		public Changelog_1_0_0(Plugin plugin) : base(plugin) { }

		public override string description => "Initial Release. Welcome to Peek!";
		public override SemanticVersion version => "1.0.0";
		public override DateTime date => new DateTime(2019, 08, 08);
		public override IEnumerable<string> changes => Enumerable.Empty<string>();
	}
}