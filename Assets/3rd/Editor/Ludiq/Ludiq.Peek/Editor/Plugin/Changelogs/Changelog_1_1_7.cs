using System;
using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_1_7), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	internal class Changelog_1_1_7 : PluginChangelog
	{
		public Changelog_1_1_7(Plugin plugin) : base(plugin) { }
		
		public override SemanticVersion version => "1.1.7";
		public override DateTime date => new DateTime(2020, 01, 25);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Fixed] Null reference exception caused by leaked background editor";
				yield return "[Fixed] Component errors in internal MathUtility class";
			}
		}
	}
}