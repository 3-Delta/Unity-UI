using System.Collections.Generic;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: AssemblyIsEditorAssembly]
[assembly: RegisterPlugin(typeof(LudiqCore), LudiqCore.ID)]
[assembly: MapToProduct(typeof(LudiqCore), LudiqProduct.ID)]

namespace Ludiq.PeekCore
{
	[PluginRuntimeAssembly(ID + ".Runtime")]
	public class LudiqCore : Plugin
	{
		public LudiqCore() : base(ID)
		{
			instance = this;
		}

		public static LudiqCore instance { get; private set; }

		public const string ID = "Ludiq.PeekCore";

		public static LudiqCoreManifest Manifest => (LudiqCoreManifest)instance.manifest;
		public static LudiqCorePaths Paths => (LudiqCorePaths)instance.paths;
		public static LudiqCoreConfiguration Configuration => (LudiqCoreConfiguration)instance.configuration;
		public static LudiqCoreResources Resources => (LudiqCoreResources)instance.resources;
		public static LudiqCoreResources.Icons Icons => Resources.icons;

		public const string LegacyRuntimeDllGuid = "1eea3bf15bb7ddb4582c462beee0ad13";
		public const string LegacyEditorDllGuid = "8878d90c345be1a43ab0c9a9898ad433";

		public override IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements
		{
			get
			{
				yield break;
				//yield return ScriptReferenceReplacement.From<DictionaryAsset>(ScriptReference.Dll(LegacyRuntimeDllGuid, "Ludiq", "DictionaryAsset"));
			}
		}
		
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