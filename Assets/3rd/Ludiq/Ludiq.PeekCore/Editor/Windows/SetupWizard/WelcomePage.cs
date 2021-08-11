using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class WelcomePage : Page
	{
		public WelcomePage(Product product, EditorWindow window) : base(window)
		{
			Ensure.That(nameof(product)).IsNotNull(product);

			title = $"{product.name} Setup Wizard";
			shortTitle = "Welcome";
			icon = LudiqCore.Resources.LoadIcon("Icons/Windows/SetupWizard/WelcomePage.png");

			this.product = product;
		}

		private readonly Product product;

		protected override void OnHeaderGUI()
		{
			if (product.logo == null)
			{
				base.OnHeaderGUI();
				return;
			}

			GUILayout.BeginVertical(LudiqStyles.windowHeaderBackground);

			LudiqGUI.Space(-5);

			// Welcome label
			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();
			GUILayout.Label("Welcome to", Styles.welcome);
			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndHorizontal();

			// Logo
			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			if (product.logo != null)
			{
				var logoHeight = Styles.productLogoHeight;
				var logoWidth = (float)product.logo.width / product.logo.height * logoHeight;
				var logoPosition = GUILayoutUtility.GetRect(logoWidth, logoHeight);
				GUI.DrawTexture(logoPosition, product.logo);
			}

			LudiqGUI.FlexibleSpace();
			LudiqGUI.EndHorizontal();

			LudiqGUI.EndVertical();
		}

		protected override void OnContentGUI()
		{
			GUILayout.BeginVertical(Styles.background, GUILayout.ExpandHeight(true));

			LudiqGUI.FlexibleSpace();
			GUILayout.Label($"Welcome to {product.name}.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();
			GUILayout.Label("This setup wizard will help you get started.", LudiqStyles.centeredLabel);
			LudiqGUI.FlexibleSpace();

			LudiqGUI.BeginHorizontal();
			LudiqGUI.FlexibleSpace();

			if (GUILayout.Button(completeLabel, Styles.nextButton))
			{
				Complete();
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
				background.padding = new RectOffset(10, 10, 10, 16);

				nextButton = new GUIStyle("Button");
				nextButton.padding = new RectOffset(20, 20, 10, 10);

				welcome = new GUIStyle(LudiqStyles.centeredLabel);
				welcome.fontSize = 14;
				welcome.margin.bottom = 16;
			}

			public static readonly GUIStyle background;
			public static readonly GUIStyle nextButton;
			public static readonly GUIStyle welcome;
			public static readonly float productLogoHeight = 96;
		}
	}
}