using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_1), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_0_1 : PluginChangelog
	{
		public Changelog_1_0_1(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.0.1";
		public override DateTime date => new DateTime(2017, 08, 02);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Missing game object constructors";
				yield return "[Fixed] Error when converting unitialized Unity object to string";
				yield return "[Fixed] Check icon for pro skin";
			}
		}
	}
}