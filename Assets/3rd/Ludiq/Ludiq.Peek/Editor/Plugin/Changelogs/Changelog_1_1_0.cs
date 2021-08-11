using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_0), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_0 : PluginChangelog
	{
		public Changelog_1_1_0(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.0";
		public override DateTime date => new DateTime(2019, 08, 26);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Probe tool to select any object behind mouse cursor (right-click)";
				yield return "[Added] ProBuilder integration for Probe";
				yield return "[Added] Scene view shortcut to toggle the display of toolbars (B)";
				yield return "[Optimized] Creator initialization with lazy loading";
				yield return "[Fixed] NullReferenceException in PluginContainer when entering play mode";
				yield return "[Fixed] Align With View shortcut conflict";
				yield return "[Fixed] TimeOutException with creator in big projects";
				yield return "[Fixed] NullReferenceException with creator when create sprites was disabled";
			}
		}
	}
}