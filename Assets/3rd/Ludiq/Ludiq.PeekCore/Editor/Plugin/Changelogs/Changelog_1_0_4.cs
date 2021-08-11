using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_4), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_0_4 : PluginChangelog
	{
		public Changelog_1_0_4(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.0.4";
		public override DateTime date => new DateTime(2017, 10, 10);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Boolean inspector height";
				yield return "[Fixed] Unity Object inspector adaptive width";
				yield return "[Fixed] Equality and inequality handling for numeric types and nulls";
			}
		}
	}
}