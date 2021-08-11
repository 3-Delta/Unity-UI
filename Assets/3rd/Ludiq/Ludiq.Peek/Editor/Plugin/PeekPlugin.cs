using System.Collections.Generic;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: AssemblyIsEditorAssembly]
[assembly: RegisterPlugin(typeof(PeekPlugin), PeekPlugin.ID)]
[assembly: MapToProduct(typeof(PeekPlugin), PeekProduct.ID)]
[assembly: RegisterPluginDependency(PeekPlugin.ID, LudiqCore.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[PluginRuntimeAssembly(ID + ".Runtime")]
	public class PeekPlugin : Plugin
	{
		public PeekPlugin() : base(ID)
		{
			instance = this;
		}

		public static PeekPlugin instance { get; private set; }

		public override IEnumerable<Page> SetupWizardPages(SetupWizard wizard)
		{
			yield break;
		}

		public const string ID = "Ludiq.Peek";

		public static PeekManifest Manifest => (PeekManifest)instance.manifest;

		public static PeekPaths Paths => (PeekPaths)instance.paths;

		public static PeekConfiguration Configuration => (PeekConfiguration)instance.configuration;

		public static PeekResources Resources => (PeekResources)instance.resources;

		public static PeekResources.Icons Icons => Resources.icons;
		
		[SettingsProvider]
		private static SettingsProvider ProjectSettingsProvider()
		{
			return CreateEarlySettingsProvider(ID, SettingsScope.Project);
		}

		[SettingsProvider]
		private static SettingsProvider EditorPrefsProvider()
		{
			return CreateEarlySettingsProvider(ID, SettingsScope.User);
		}
	}
}