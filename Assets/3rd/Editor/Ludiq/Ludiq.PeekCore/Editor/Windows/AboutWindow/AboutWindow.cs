using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public sealed class AboutWindow : SinglePageWindow<AboutPage>
	{
		private Product product;
		private IEnumerable<Plugin> plugins;

		public static void Show(IEnumerable<Plugin> plugins)
		{
			Ensure.That(nameof(plugins)).IsNotNull(plugins);
			Show(plugins, null);
		}

		public static void Show(Product product)
		{
			Ensure.That(nameof(product)).IsNotNull(product);
			Show(null, product);
		}

		private static void Show(IEnumerable<Plugin> plugins, Product product)
		{
			var window = CreateInstance<AboutWindow>();
			window.Initialize(plugins, product);
			window.ShowUtility();
			window.Center();
		}

		private void Initialize(IEnumerable<Plugin> plugins, Product product)
		{
			this.plugins = plugins;
			this.product = product;
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			minSize = maxSize = new Vector2(470, 370);
		}

		protected override AboutPage CreatePage()
		{
			if (product != null)
			{
				return new AboutPage(product, this);
			}
			else if (plugins != null)
			{
				return new AboutPage(plugins, this);
			}

			throw new NotSupportedException();
		}
	}
}