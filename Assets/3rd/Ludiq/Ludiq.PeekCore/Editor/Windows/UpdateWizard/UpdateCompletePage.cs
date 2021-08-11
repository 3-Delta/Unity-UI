using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class UpdateCompletePage : Page
	{
		public UpdateCompletePage(IEnumerable<Plugin> plugins, EditorWindow window) : base(window)
		{
			title = "Update Complete";
			shortTitle = "Finish";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/UpdateWizard/UpdateCompletePage.png");

			hasChangelogs = plugins.ResolveDependencies().SelectMany(plugin => plugin.resources.changelogs).Any();
		}

		private readonly bool hasChangelogs;

		protected override void OnShow()
		{
			base.OnShow();

			PluginContainer.UpdateVersionMismatch();
		}

		protected override void OnContentGUI()
		{
			GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

			LudiqGUI.FlexibleSpace();
			GUILayout.Label($"Plugins have been updated.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();

			if (hasChangelogs)
			{
				LudiqGUI.BeginHorizontal();
				LudiqGUI.FlexibleSpace();

				if (GUILayout.Button("Show Changelogs", Styles.button))
				{
					Complete();
				}

				LudiqGUI.FlexibleSpace();
				LudiqGUI.EndHorizontal();

				LudiqGUI.Space(10);
			}

			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			if (GUILayout.Button("Close", Styles.button))
			{
				window.Close();
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