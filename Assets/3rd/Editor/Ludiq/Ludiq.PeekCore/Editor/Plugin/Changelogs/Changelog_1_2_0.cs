using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_2_0), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_2_0 : PluginChangelog
	{
		public Changelog_1_2_0(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.2.0";
		public override DateTime date => new DateTime(2017, 11, 16);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Added] Support for dragging on number fields (hold Shift)";
				yield return "[Added] Support for [AssemblyIsEditorAssembly]";
				yield return "[Added] Pre-build method for Unity Cloud Build (Ludiq.AotPreBuilder.PreCloudBuild)";
				yield return "[Optimized] Fuzzy finder search cancellation";
				yield return "[Fixed] Expanded tooltip on boolean inspector";
				yield return "[Fixed] Adaptive width for Vector and Quaternion inspectors";
				yield return "[Fixed] Minimum width for Rect, Ray and Ray2D inspectors";
				yield return "[Fixed] First fuzzy window search result not always focusing";
				yield return "[Fixed] NullReferenceException in Namespace class";
				yield return "[Fixed] Coroutine runner being destroyed on scene load";
				yield return "[Fixed] Type options failing to load in .NET 4.6";
			}
		}
	}
}