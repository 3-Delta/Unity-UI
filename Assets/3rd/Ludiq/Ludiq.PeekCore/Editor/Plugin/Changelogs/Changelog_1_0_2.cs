using System;
using System.Collections.Generic;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Changelog_1_0_2), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Changelog_1_0_2 : PluginChangelog
	{
		public Changelog_1_0_2(Plugin plugin) : base(plugin) { }

		public override SemanticVersion version => "1.0.2";
		public override DateTime date => new DateTime(2017, 09, 08);

		public override IEnumerable<string> changes
		{
			get
			{
				yield return "[Optimized] Editor plugin container initialization";
				yield return "[Added] LayerMask type, icon and inspector";
				yield return "[Changed] Application and NavMesh to default types";
				yield return "[Fixed] Programmer naming for constructors";
				yield return "[Fixed] Hierarchy issues in type option tree";
			}
		}
	}
}