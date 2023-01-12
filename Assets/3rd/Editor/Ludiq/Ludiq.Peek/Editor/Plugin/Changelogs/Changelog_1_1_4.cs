using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_4), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_4 : PluginChangelog
	{
		public Changelog_1_1_4(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.4";
		public override DateTime date => new DateTime(2019, 11, 07);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Reference inspector button position when label is missing";
				yield return "[Fixed] Editor crashes related to dynamic reflected calls on destroyed objects (tentative)";
				yield return "[Fixed] Failed assertion during build (EditorApplicaiton.MayUpdate)";
				yield return "[Fixed] Failure to deserialize UnityEngine.Touch during project settings load";
				yield return "[Fixed] Spacing issues in the updated 2019.3 beta flat skin";
				yield return "[Fixed] Errors when saving project settings when the asset was missing";
				yield return "[Fixed] Assets failing to load after a library reimport"; 
			}
		}
	}
}