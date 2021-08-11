using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class SetupWizard : Wizard
	{
		private Product product;

		public static void Show(Product product)
		{
			Ensure.That(nameof(product)).IsNotNull(product);
			var wizard = CreateInstance<SetupWizard>();
			wizard.Initialize(product);
			wizard.ShowUtility();
			wizard.Center();
		}

		private void Initialize(Product product)
		{
			this.product = product;
			
			foreach (var introductionPage in product.SetupWizardIntroductionPages(this).NotNull())
			{
				pages.Add(introductionPage);
			}

			foreach (var plugin in product.plugins.ResolveDependencies())
			{
				foreach (var pluginPage in plugin.SetupWizardPages(this).NotNull())
				{
					pages.Add(pluginPage);
				}
			}
			
			foreach (var conclusionPage in product.SetupWizardConclusionPages(this).NotNull())
			{
				pages.Add(conclusionPage);
			}

			titleContent = new GUIContent($"{product.name} Setup Wizard");

			Initialize();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			minSize = maxSize = new Vector2(500, 400);
		}
	}
}