using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_2_0), PeekPlugin.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_2_0f1), PeekPlugin.ID)]
[assembly: MapToPlugin(typeof(Changelog_1_2_0f2), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_2_0 : PluginChangelog
	{
		public Changelog_1_2_0(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.2.0";
		public override DateTime date => new DateTime(2020, 09, 19);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Configurable Shortcuts";
			}
		}
	}

	internal class Changelog_1_2_0f1 : PluginChangelog
	{
		public Changelog_1_2_0f1(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.2.0f1";
		public override DateTime date => new DateTime(2020, 09, 21);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Error with toolbar shortcuts";
			}
		}
	}

	internal class Changelog_1_2_0f2 : PluginChangelog
	{
		public Changelog_1_2_0f2(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.2.0f2";
		public override DateTime date => new DateTime(2020, 09, 22);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Performance regression with Creator tool in scene view";
			}
		}
	}
}