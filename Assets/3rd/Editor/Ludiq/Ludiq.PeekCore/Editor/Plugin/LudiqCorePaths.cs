using System.IO;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(LudiqCorePaths), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	public sealed class LudiqCorePaths : PluginPaths
	{
		private LudiqCorePaths(LudiqCore plugin) : base(plugin) { }

		public string propertyProviders => Path.Combine(transientGenerated, "Property Providers");
		public string propertyProvidersEditor => Path.Combine(propertyProviders, "Editor");
		public string assemblyDocumentations => Path.Combine(transientGenerated, "Documentation");
		public string dotNetDocumentation => Path.Combine(package, "DotNetDocumentation");
	}
}