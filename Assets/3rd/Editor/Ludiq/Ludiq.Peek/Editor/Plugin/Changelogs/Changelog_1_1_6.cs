using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_6), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_6 : PluginChangelog
	{
		public Changelog_1_1_6(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.6";
		public override DateTime date => new DateTime(2019, 12, 12);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Option to offset hierarchy toolbars for compatibility with other third-party plugins that extend the hierarchy";
				yield return "[Added] Option to blacklist asset folders from creator menu";
				yield return "[Added] Option to whitelist tabs for windows that are not within the current layout";
				yield return "[Added] Tools menu shortcuts to online resources";
				yield return "[Fixed] Asset folders not showing up in creator if not direct parent of assets";
				yield return "[Fixed] Warning when plugin path instead of generated path is in a first-pass compilation folder";
				yield return "[Fixed] GameObject root folder appearing in creator menu";
				yield return "[Fixed] Editor popup not automatically repainting on scene change";
				yield return "[Fixed] Recover when assembly fails to load during codebase initialization due to corruption";
				yield return "[Fixed] AmbiguousMatchException on EditorUtility.IsDirty in Unity 2018";
				yield return "[Fixed] Script tools disappearing when exiting play mode because caches were not cleared due to lack of assembly reload";
				yield return "[Changed] Hid developer/internal menu items when developer mode is disabled in the editor preferences";
				yield return "[Removed] Warning when plugin container initialization has to be delayed";
			}
		}
	}
}