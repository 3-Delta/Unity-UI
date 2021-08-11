using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class UpdateStartPage : Page
	{
		public UpdateStartPage(Product product, IEnumerable<Plugin> plugins, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);

			title = $"{product?.name ?? "Plugin"} Update Wizard";
			shortTitle = "Status";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/UpdateWizard/UpdateStartPage.png");

			this.plugins = new List<Plugin>(plugins.OrderByDependencies());
		}

		private readonly List<Plugin> plugins;

		protected override void OnContentGUI()
		{
			GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));
			LudiqGUI.FlexibleSpace();

			if (plugins.All(plugin => plugin.manifest.savedVersion == plugin.manifest.currentVersion))
			{
				GUILayout.Label("All your plugins are up to date.", LudiqStyles.centeredLabel);
			}
			else
			{
				GUILayout.Label("Welcome to the plugin update wizard.", LudiqStyles.centeredLabel);
			}

			LudiqGUI.FlexibleSpace();
			UpdateWizard.DrawPluginVersionTable(plugins);
			LudiqGUI.FlexibleSpace();
			LudiqGUI.FlexibleSpace();
			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			if (plugins.Any(plugin => plugin.manifest.savedVersion != plugin.manifest.currentVersion))
			{
				if (GUILayout.Button("Start", Styles.completeButton))
				{
					Complete();
				}
			}
			else
			{
				if (GUILayout.Button("Close", Styles.completeButton))
				{
					window.Close();
				}
			}

			LudiqGUI.FlexibleSpace();

			LudiqGUI.EndHorizontal();

			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndVertical();
		}

		public static class Styles
		{
			static Styles()
			{
				background = new GUIStyle(LudiqStyles.windowBackground);
				background.padding = new RectOffset(10, 10, 10, 10);

				completeButton = new GUIStyle("Button");
				completeButton.padding = new RectOffset(13, 13, 7, 7);
			}

			public static readonly GUIStyle background;
			public static readonly GUIStyle completeButton;
		}
	}
}