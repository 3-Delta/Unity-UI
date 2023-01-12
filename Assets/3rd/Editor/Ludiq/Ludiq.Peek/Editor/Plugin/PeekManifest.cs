using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(PeekManifest), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class PeekManifest : PluginManifest
	{
		private PeekManifest(PeekPlugin plugin) : base(plugin) { }

		public override string name => "Peek";

		public override string author => "Ludiq";

		public override string description => "Workflow Tools for Unity.";

		public override SemanticVersion version => "1.3.3";
	}
}