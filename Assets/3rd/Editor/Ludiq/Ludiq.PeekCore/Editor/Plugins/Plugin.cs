using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public abstract class Plugin
	{
		protected Plugin(string id)
		{
			Ensure.That(nameof(id)).IsNotNull(id);

			this.id = id;

			dependencies = PluginContainer.pluginDependencies[id].Select(PluginContainer.GetPlugin).ToList().AsReadOnly();
		}

		public string id { get; }
		public ReadOnlyCollection<Plugin> dependencies { get; }

		public PluginManifest manifest { get; internal set; }
		public PluginConfiguration configuration { get; internal set; }
		public PluginPaths paths { get; internal set; }
		public PluginResources resources { get; internal set; }

		public virtual IEnumerable<ScriptReferenceReplacement> scriptReferenceReplacements => Enumerable.Empty<ScriptReferenceReplacement>();

		public virtual IEnumerable<object> aotStubs => Enumerable.Empty<object>();

		public virtual IEnumerable<string> tips => Enumerable.Empty<string>();

		public virtual IEnumerable<Page> SetupWizardPages(SetupWizard wizard)
		{
			return Enumerable.Empty<Page>();
		}

		protected static SettingsProvider CreateEarlySettingsProvider(string id, SettingsScope scope)
		{
			// If the preference / setting panel is open before the plugin container gets initialized,
			// we can't afford to return null in [SettingsProvider], because the panel will then never 
			// get created later. This is a workaround that creates a wrapper around the configuration now
			// which doesn't render anything until the plugin container is initialized.

			if (PluginContainer.initialized)
			{
				return PluginContainer.GetPlugin(id).configuration.CreateSettingsProvider(scope);
			}
			else
			{
				return new SettingsProvider(PluginConfiguration.ScopeRoot(scope) + "/Ludiq/" + id, scope)
				{
					label = id,
					guiHandler = query =>
					{
						if (PluginContainer.initialized)
						{
							PluginContainer.GetPlugin(id).configuration.OnGUI(scope, query);
						}
					},
					footerBarGuiHandler = () =>
					{
						if (PluginContainer.initialized)
						{
							PluginContainer.GetPlugin(id).configuration.OnFooterGUI(scope);
						}
					},
				};
			}
		}
	}
}