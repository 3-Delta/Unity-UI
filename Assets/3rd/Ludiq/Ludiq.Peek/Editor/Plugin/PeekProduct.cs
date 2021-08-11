using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;

[assembly: RegisterProduct(typeof(PeekProduct), PeekProduct.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class PeekProduct : Product
	{
		public PeekProduct() : base(ID) { }

		public override string name => "Peek";

		public override string description => "Workflow Tools for Unity.";

		public override string authorLabel => "Designed & Developed by ";

		public override string author => "Lazlo Bonin";

		public override string copyrightHolder => "Ludiq";
		
		public override SemanticVersion version => "1.3.3";

		public const string ID = "Peek";

		public const int ToolsMenuPriority = -1000000;

		public const int InternalToolsMenuPriority = ToolsMenuPriority + 1000;

		public static PeekProduct instance => (PeekProduct)ProductContainer.GetProduct(ID);

		public override void Initialize()
		{
			base.Initialize();

			logo = PeekPlugin.Resources.LoadTexture("Logos/LogoPeek.png", CreateTextureOptions.Scalable)?.Single();
			authorLogo = PeekPlugin.Resources.LoadTexture("Logos/LogoLudiq.png", CreateTextureOptions.Scalable)?.Single();
		}

		[MenuItem("Tools/Peek/About...", priority = ToolsMenuPriority + 100)]
		private static void HookAboutWindow()
		{
			AboutWindow.Show(instance);
		}

		[MenuItem("Tools/Peek/Setup Wizard...", priority = ToolsMenuPriority + 101)]
		private static void HookSetupWizard()
		{
			SetupWizard.Show(instance);
		}

		[MenuItem("Tools/Peek/Update Wizard...", priority = ToolsMenuPriority + 102)]
		private static void HookUpdateWizard()
		{
			UpdateWizard.Show();
		}

		[MenuItem("Tools/Peek/Manual", priority = ToolsMenuPriority + 201)]
		private static void HookManual()
		{
			Process.Start("https://ludiq.io/peek/manual");
		}

		[MenuItem("Tools/Peek/Shortcuts", priority = ToolsMenuPriority + 202)]
		private static void HookShortcuts()
		{
			Process.Start("https://ludiq.io/peek/manual/shortcuts");
		}

		[MenuItem("Tools/Peek/Discord", priority = ToolsMenuPriority + 203)]
		private static void HookDiscord()
		{
			Process.Start("https://ludiq.io/peek/discord");
		}

		[MenuItem("Tools/Peek/Forum", priority = ToolsMenuPriority + 204)]
		[MenuItem("Tools/Peek/Bug Report", priority = ToolsMenuPriority + 205)]
		private static void HookForum()
		{
			Process.Start("https://ludiq.io/peek/forum");
		}

		public override IEnumerable<Page> SetupWizardConclusionPages(SetupWizard wizard)
		{
			yield return new PeekSetupCompletePage(this, wizard);
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Developer/Prepare for Release...", priority = InternalToolsMenuPriority + 101)]
#endif
		private static bool PrepareForRelease()
		{
			if (!EditorUtility.DisplayDialog("Delete Generated Files", "This action will delete all generated files, including those containing user data.\n\nAre you sure you want to continue?", "Confirm", "Cancel"))
			{
				return false;
			}
			
			foreach (var plugin in PluginContainer.plugins)
			{
				PathUtility.DeleteDirectoryIfExists(plugin.paths.generatedRoot);
			}
			
			PluginAcknowledgement.GenerateLicenseFile(Path.Combine(instance.rootPath, "LICENSES.txt"));

			return true;
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Developer/Export Release Package...", priority = InternalToolsMenuPriority + 101)]
#endif
		private static void ExportReleasePackage()
		{
			if (!PrepareForRelease())
			{
				return;
			}

			var packagePath = EditorUtility.SaveFilePanel("Export Release Package",
														  Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
														  "Peek_" + instance.version.ToString().Replace(".", "_").Replace(" ", "_"),
														  "unitypackage");

			if (packagePath == null)
			{
				return;
			}

			var packageDirectory = Path.GetDirectoryName(packagePath);

			ProgressUtility.DisplayProgressBar("Exporting Release Package", "Creating Unity Package...", 0);

			var paths = new List<string>();

			foreach (var product in ProductContainer.products)
			{
				paths.Add(product.rootPath);
			}

			foreach (var plugin in PluginContainer.plugins)
			{
				paths.Add(plugin.paths.package);
			}

			AssetDatabase.ExportPackage(paths.Select(PathUtility.FromProject).ToArray(), packagePath, ExportPackageOptions.Recurse);
			
			ProgressUtility.ClearProgressBar();

			if (EditorUtility.DisplayDialog("Export Release Package", "Release package export complete.\nOpen containing folder?", "Open Folder", "Close"))
			{
				Process.Start(packageDirectory);
			}
		}
	}
}