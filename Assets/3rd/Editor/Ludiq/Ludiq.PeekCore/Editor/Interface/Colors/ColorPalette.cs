using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	[InitializeOnLoad]
	public static class ColorPalette
	{
		// Unity

		public static SkinnedColor unityBackgroundVeryDark = new SkinnedColor(ColorUtility.Gray(0.33f), ColorUtility.Gray(0.10f));
		public static SkinnedColor unityBackgroundDark = new SkinnedColor(ColorUtility.Gray(0.64f), ColorUtility.Gray(0.16f));
		public static SkinnedColor unityBackgroundMid = new SkinnedColor(ColorUtility.Gray(0.76f), ColorUtility.Gray(0.22f));
		public static SkinnedColor unityBackgroundLight = new SkinnedColor(ColorUtility.Gray(0.87f), ColorUtility.Gray(0.24f));
		public static SkinnedColor unityBackgroundLighter = new SkinnedColor(ColorUtility.Gray(0.87f * 1.1f), ColorUtility.Gray(0.24f * 1.1f));
		public static SkinnedColor unityBackgroundPure = new SkinnedColor(Color.white, Color.black);
		public static SkinnedColor unityForeground = new SkinnedColor(ColorUtility.Gray(0.00f), ColorUtility.Gray(0.81f));
		public static SkinnedColor unityForegroundDim = unityForeground.WithAlphaMultiplied(0.40f, 0.40f);
		public static SkinnedColor unityForegroundSelected = new SkinnedColor(ColorUtility.Gray(1.00f), ColorUtility.Gray(1.00f));
		public static SkinnedColor unitySelectionHighlight = new SkinnedColor(new Color(0.24f, 0.49f, 0.91f), new Color(0.20f, 0.38f, 0.57f));
		public static SkinnedColor unityGraphBackground = new SkinnedColor(ColorUtility.Gray(0.36f), ColorUtility.Gray(0.16f));

		// Rotorz

		public static SkinnedColor reorderableListBackground = new SkinnedColor(ColorUtility.Gray(0.83f), ColorUtility.Gray(0.22f));

		// Utility

		public static SkinnedColor transparent = new SkinnedColor(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0));
		public static SkinnedColor hyperlink = new SkinnedColor(Color.blue, new Color(0.34f, 0.61f, 0.84f));
		public static SkinnedColor hyperlinkActive = new SkinnedColor(Color.red, Color.white);

		// Colors
		public static SkinnedColor orange = new SkinnedColor(new Color(1.0f, 0.62f, 0.35f));
		public static SkinnedColor purple = new SkinnedColor(new Color(0.86f, 0.55f, 0.92f));
		public static SkinnedColor yellow = new SkinnedColor(new Color(1.0f, 0.90f, 0.40f));
		public static SkinnedColor pink = new SkinnedColor(new Color(1.0f, 0.63f, 0.66f));
		public static SkinnedColor blue = new SkinnedColor(new Color(0.45f, 0.78f, 1f));
		public static SkinnedColor teal = new SkinnedColor(new Color(0.45f, 1.00f, 0.82f));
		public static SkinnedColor green = new SkinnedColor(new Color(0.60f, 0.88f, 0.00f));
		public static SkinnedColor red = new SkinnedColor(new Color(1, 0.3f, 0.3f));
	}
}