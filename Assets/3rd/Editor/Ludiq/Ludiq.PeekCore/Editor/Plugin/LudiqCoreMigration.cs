using System;

namespace Ludiq.PeekCore
{
	internal abstract class LudiqCoreMigration : PluginMigration
	{
		protected LudiqCoreMigration(Plugin plugin) : base(plugin) { }

		[Obsolete]
		protected void AddLegacyDefaultTypeOption(Type type)
		{

		}

		[Obsolete]
		protected void AddLegacyDefaultAssemblyOption(LooseAssemblyName assembly)
		{

		}
	}
}
