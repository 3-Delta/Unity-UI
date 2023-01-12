using System.Diagnostics;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: MapToPlugin(typeof(PeekResources), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	class PeekSetupCompletePage : SetupCompletePage
	{
		public PeekSetupCompletePage(Product product, EditorWindow window) : base(product, window) { }

		protected override void SetupButtons()
		{
			AddButton
			(
				"Manual",
				"Learn more about the tools and shortcuts in Peek.", 
				LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/LearnButton.png"),
				() => Process.Start("https://ludiq.io/peek/manual")
			); 
			
			AddButton
			( 
				"Forum",
				"The place to request features or report bugs.", 
				LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/CommunityButton.png"),
				() => Process.Start("https://ludiq.io/peek/forum")
			);

			AddButton
			(
				"Preferences",
				"Customize the behaviour of your Peek tools.",
				LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/ConfigurationButton.png"),
				() => SettingsService.OpenUserPreferences("Preferences/Ludiq/Peek")
			);

			AddButton
			(
				"Project Settings",
				"Configure Peek options for the current project.",
				LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/ConfigurationButton.png"),
				() => SettingsService.OpenProjectSettings("Project/Ludiq/Peek")
			);

			base.SetupButtons();
		}
	}
}
