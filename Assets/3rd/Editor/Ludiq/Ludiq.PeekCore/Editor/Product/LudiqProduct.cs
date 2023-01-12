using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterProduct(typeof(LudiqProduct), LudiqProduct.ID)]

namespace Ludiq.PeekCore
{
	public sealed class LudiqProduct : Product
	{
		public LudiqProduct() : base(ID) { }

		public override string name => "Ludiq Framework (Peek Version)";
		public override string description => "Complete toolset for quality Unity plugin development.";
		public override string authorLabel => "Designed & Developed by ";
		public override string author => "Lazlo Bonin";
		public override string copyrightHolder => "Ludiq";
		public override SemanticVersion version => "2.0.0a5";
		public const string ID = "Ludiq.PeekCore";

		public const int ToolsMenuPriority = -1000000;
		public const int InternalToolsMenuPriority = ToolsMenuPriority + 1000;

		public static LudiqProduct instance => (LudiqProduct)ProductContainer.GetProduct(ID);

		public override void Initialize()
		{
			base.Initialize();

			logo = LudiqCore.Resources.LoadTexture("Logos/LogoFramework.png", CreateTextureOptions.Scalable)?.Single();
			authorLogo = LudiqCore.Resources.LoadTexture("Logos/LogoLudiq.png", CreateTextureOptions.Scalable)?.Single();
		}

		[MenuItem("Tools/Peek/Ludiq/About...", priority = ToolsMenuPriority + 100)]
		private static void HookAboutWindow()
		{
			AboutWindow.Show(instance);
		}

		[MenuItem("Tools/Peek/Ludiq/Setup Wizard...", priority = ToolsMenuPriority + 101)]
		private static void HookSetupWizard()
		{
			SetupWizard.Show(instance);
		}

		[MenuItem("Tools/Peek/Ludiq/Update Wizard...", priority = ToolsMenuPriority + 102)]
		private static void HookUpdateWizard()
		{
			UpdateWizard.Show();
		}
	}
}