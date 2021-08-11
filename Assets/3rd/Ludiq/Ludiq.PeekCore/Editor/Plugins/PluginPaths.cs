#pragma warning disable 162

using System.IO;
using Ludiq.PeekCore;
using UnityEngine;

[assembly: RegisterPluginModuleType(typeof(PluginPaths), true)]

namespace Ludiq.PeekCore
{
	public class PluginPaths : IPluginModule
	{
		protected PluginPaths(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public virtual void Initialize() { }

		public virtual void LateInitialize() { }

		public Plugin plugin { get; }
		
		private string _package;

		public string package
		{
			get
			{
				if (_package == null)
				{
					_package = PathUtility.GetRootPath(rootFileName, Path.Combine(Paths.assets, $"Ludiq/{plugin.id}"), true);
				}

				return _package;
			}
		}

		private static string _generatedRoot;

		public string generatedRoot
		{
			get
			{
				if (_generatedRoot == null)
				{
					_generatedRoot = PathUtility.GetRootPath("Ludiq.Generated.root", Path.Combine(Paths.assets, "Ludiq.Generated"), true);
					
					if (PathUtility.IsInFirstPassFolder(_generatedRoot))
					{
						Debug.LogWarning($"Plugin '{plugin.id}' has generated assets in a special Unity folder that makes it compile first.\nThis might cause issues with generated assets. Path:\n{_generatedRoot}");
					}
				}

				return _generatedRoot;
			}
		}
		
		protected string rootFileName => plugin.id + ".root";
		
		public string resourcesFolder => Path.Combine(package, "Editor/Resources");

		public string resourcesBundle => Path.Combine(package, "Editor/Resources.assetbundle");

		public string projectSettings => Path.Combine(persistentGenerated, $"ProjectSettings/{plugin.id}.ProjectSettings.asset");
		
		public string persistentGenerated => Path.Combine(generatedRoot, "Persistent");
		
		public string transientGenerated => Path.Combine(generatedRoot, "Transient");

		public string iconMap => Path.Combine(package, "Editor/IconMap");
	}
}
