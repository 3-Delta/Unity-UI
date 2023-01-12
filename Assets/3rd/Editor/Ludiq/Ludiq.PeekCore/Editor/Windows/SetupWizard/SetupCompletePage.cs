using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class SetupCompletePage : Page
	{
		public SetupCompletePage(Product product, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(product)).IsNotNull(product);

			title = "Setup Complete";
			shortTitle = "Finish";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/SetupCompletePage.png");
			
			this.product = product;
		}

		private readonly List<(GUIContent, Action)> buttons = new List<(GUIContent, Action)>();

		private readonly Product product;

		protected void AddButton(GUIContent label, Action action)
		{
			buttons.Add((label, action));
		}

		protected void AddButton(string title, string subtitle, EditorTexture icon, Action action)
		{
			AddButton(new GUIContent(title, icon?[IconSize.Medium], subtitle), action);
		}

		protected virtual void SetupButtons()
		{
			AddButton
			(
				"Done",
				$"Close the wizard and start using {product.name}!", 
				LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/CompleteButton.png"),
				Complete
			);
		}

		protected override void OnShow()
		{
			base.OnShow();

			buttons.Clear();
			SetupButtons();

			foreach (var plugin in product.plugins.ResolveDependencies())
			{
				plugin.configuration.projectSetupCompleted = true;
				plugin.configuration.editorSetupCompleted = true;
				plugin.configuration.Save();
			}

			AssetDatabase.SaveAssets();

			// Run the gizmo disabler. It's an expensive operation,
			// so we don't do it on every assembly reload, but this way at least
			// we make sure that the gizmos will be properly disabled on install.
			AnnotationDisabler.DisableGizmos();
		}
		
		protected override void OnContentGUI()
		{
			GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

			LudiqGUI.FlexibleSpace();
			GUILayout.Label($"{product.name} has successfully been setup.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();

			int index = 0;

			foreach (var button in buttons)
			{
				if (index % 2 == 0)
				{
					LudiqGUI.BeginHorizontal();

					LudiqGUI.FlexibleSpace();
				}

				EditorGUI.BeginDisabledGroup(button.Item2 == null);

				if (LudiqGUI.BigButtonLayout(button.Item1))
				{
					button.Item2?.Invoke();
				}

				EditorGUI.EndDisabledGroup();
				
				if (index % 2 == 0)
				{
					LudiqGUI.FlexibleSpace();
				}
				else
				{
					LudiqGUI.FlexibleSpace();

					LudiqGUI.EndHorizontal();
					
					LudiqGUI.Space(Styles.spaceBetweenButtons);
				}

				index++;
			}

			if (index % 2 == 1)
			{
				LudiqGUI.EndHorizontal();
			}
			
			LudiqGUI.FlexibleSpace();

			LudiqGUI.EndVertical();
		}

		public static class Styles
		{
			static Styles()
			{
				background = new GUIStyle(LudiqStyles.windowBackground);
				background.padding = new RectOffset(10, 10, 10, 10);
			}

			public static readonly GUIStyle background;
			public static readonly float spaceBetweenButtons = 12;
		}
	}
}