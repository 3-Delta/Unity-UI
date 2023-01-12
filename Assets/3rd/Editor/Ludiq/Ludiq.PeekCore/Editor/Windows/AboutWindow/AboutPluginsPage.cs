using System.Collections.Generic;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public class AboutPluginsPage : ListPage
	{
		public AboutPluginsPage(IEnumerable<Plugin> plugins, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);

			title = "About Plugins";
			shortTitle = "Plugins";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/AboutWindow/AboutPluginsPage.png");

			foreach (var plugin in plugins)
			{
				pages.Add(new AboutablePage(plugin.manifest, window));
			}
		}
	}
}