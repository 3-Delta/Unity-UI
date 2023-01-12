using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(PeekPaths), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class PeekPaths : PluginPaths
	{
		private PeekPaths(PeekPlugin plugin) : base(plugin) { }
	}
}