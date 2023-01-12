using UnityEngine;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(Migration_1_2_2_to_1_2_3), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	internal class Migration_1_2_2_to_1_2_3 : LudiqCoreMigration
	{
		public Migration_1_2_2_to_1_2_3(Plugin plugin) : base(plugin) { }

		public override SemanticVersion from => "1.2.2";

		public override SemanticVersion to => "1.2.3";

		public override void Run()
		{
			// AddLegacyDefaultTypeOption(typeof(Screen));
		}
	}
}
