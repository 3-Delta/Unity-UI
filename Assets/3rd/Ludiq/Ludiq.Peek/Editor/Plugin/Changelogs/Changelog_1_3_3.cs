using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_3_3), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_3_3 : PluginChangelog
	{
		public Changelog_1_3_3(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.3.3";

		public override DateTime date => new DateTime(2021, 03, 20);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Regression on tabs positioning";
				yield return "[Fixed] Error when grouping nested objects";
				yield return "[Fixed] Error when Probe hit MeshRenderer without MeshFilter";
			}
		}
	}
}