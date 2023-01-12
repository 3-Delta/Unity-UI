using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class UpdateConsolidatePage : Page
	{
		public UpdateConsolidatePage(IEnumerable<Plugin> plugins, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);

			this.plugins = plugins.Where(plugin => plugin.manifest.savedVersion > plugin.manifest.currentVersion).ToList();

			title = "Force Version Consolidation";
			shortTitle = "Consolidate";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/UpdateWizard/UpdateConsolidatePage.png");
		}

		private readonly List<Plugin> plugins;

		protected override void OnContentGUI()
		{
			if (!plugins.Any())
			{
				Complete();
			}

			GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));
			LudiqGUI.FlexibleSpace();
			GUILayout.Label($"The following plugins have been downgraded to a previous version: ", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();
			UpdateWizard.DrawPluginVersionTable(plugins);
			LudiqGUI.FlexibleSpace();
			GUILayout.Label("This will likely cause backwards incompatibility issues.", LudiqStyles.centeredLabel);
			GUILayout.Label("We recommend that you re-install their saved version to avoid data corruption.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();

			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			if (GUILayout.Button("Close", Styles.button))
			{
				window.Close();
			}

			LudiqGUI.Space(10);

			if (GUILayout.Button("Force Consolidate", Styles.button))
			{
				var consolidationExplanation = string.Empty;
				consolidationExplanation += $"By forcing consolidation, the saved versions will be set to match the installed versions. ";
				consolidationExplanation += "This does not roll back updates and may lead to data corruption. ";
				consolidationExplanation += "You should only do this if you know what you are doing. ";
				consolidationExplanation += "A backup will be created automatically if you decide to proceed.";

				if (EditorUtility.DisplayDialog("Force Consolidate Version", consolidationExplanation, "Force Consolidate", "Cancel"))
				{
					foreach (var plugin in plugins)
					{
						plugin.manifest.savedVersion = plugin.manifest.currentVersion;
					}

					Complete();
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

				button = new GUIStyle("Button");
				button.padding = new RectOffset(15, 15, 5, 5);
			}

			public static readonly GUIStyle background;
			public static readonly GUIStyle button;
		}
	}
}