using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_5), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_5 : PluginChangelog
	{
		public Changelog_1_1_5(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.5";
		public override DateTime date => new DateTime(2019, 11, 11);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Attempted crash fix by using manual reflection instead of dynamic keyword in two areas reported to cause crashes";
				yield return "[Fixed] Reference inspector ExitGUIException wrapped inside TargetInvocationException getting caught and handled by Odin";
				yield return "[Fixed] Errors in when fuzzy window was open during assembly reload";
			}
		}
	}
}