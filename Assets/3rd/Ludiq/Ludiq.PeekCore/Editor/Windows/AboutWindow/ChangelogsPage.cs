using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public class ChangelogsPage : ListPage
	{
		public ChangelogsPage(IEnumerable<Plugin> plugins, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);

			title = "Changelogs";
			shortTitle = "Changelogs";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/AboutWindow/ChangelogPage.png");

			var pluginsWithDependencies = plugins.ResolveDependencies().ToArray();

			foreach (var changelog in pluginsWithDependencies.SelectMany(plugin => plugin.resources.changelogs).OrderByDescending(changelog => changelog.date))
			{
				pages.Add(new ChangelogPage(changelog, pluginsWithDependencies.Length > 1, window));
			}
		}
	}
}