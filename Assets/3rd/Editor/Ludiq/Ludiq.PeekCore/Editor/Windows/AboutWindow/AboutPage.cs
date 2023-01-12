using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class AboutPage : TabbedPage
	{
		private AboutPage(EditorWindow window) : base(window)
		{
			title = shortTitle = "About";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/AboutWindow/AboutPage.png");
		}

		public AboutPage(IEnumerable<Plugin> plugins, EditorWindow window) : this(window)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);

			if (plugins.Count() == 1)
			{
				title = shortTitle = $"About {plugins.Single().manifest.name}";
				mainAboutable = plugins.Single().manifest;
			}

			CreatePages(plugins.OrderByDependencies());
		}

		public AboutPage(Product product, EditorWindow window) : this(window)
		{
			Ensure.That(nameof(product)).IsNotNull(product);

			title = shortTitle = $"About {product.name}";
			mainAboutable = product;

			var productPage = new AboutablePage(product, window);
			productPage.shortTitle = "Product";

			pages.Add(productPage);

			CreatePages(product.plugins);
		}

		private readonly IAboutable mainAboutable;

		protected override void OnHeaderGUI()
		{
			if (mainAboutable == null || mainAboutable.logo == null)
			{
				base.OnHeaderGUI();
				return;
			}

			GUILayout.BeginVertical(LudiqStyles.windowHeaderBackground, GUILayout.ExpandWidth(true));
			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			var logoHeight = Styles.logoHeight;
			var logoWidth = (float)mainAboutable.logo.width / mainAboutable.logo.height * logoHeight;
			var logoPosition = GUILayoutUtility.GetRect(logoWidth, logoHeight);
			GUI.DrawTexture(logoPosition, mainAboutable.logo);

			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndHorizontal();

			LudiqGUI.Space(13);
			OnTabsGUI();
			LudiqGUI.Space(-5);

			LudiqGUI.EndVertical();
		}

		private void CreatePages(IEnumerable<Plugin> plugins)
		{
			pages.Add(new AboutPluginsPage(plugins, window));
			pages.Add(new ChangelogsPage(plugins, window));
			pages.Add(new AcknowledgementsPage(plugins, window));
		}

		public new static class Styles
		{
			static Styles() { }

			public static readonly float logoHeight = 80;
		}
	}
}