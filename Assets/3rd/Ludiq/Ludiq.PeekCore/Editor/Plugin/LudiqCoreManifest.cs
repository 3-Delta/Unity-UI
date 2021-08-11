using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(LudiqCoreManifest), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	public sealed class LudiqCoreManifest : PluginManifest
	{
		private LudiqCoreManifest(LudiqCore plugin) : base(plugin) { }

		public override string name => "Ludiq Core (Peek)";
		public override string author => "Ludiq";
		public override string description => "IoC framework and toolset for Unity plugin development.";
		public override SemanticVersion version => "2.0.0a5";
	}
}