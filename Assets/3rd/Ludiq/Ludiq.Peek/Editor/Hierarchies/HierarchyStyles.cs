using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class HierarchyStyles
	{
		static HierarchyStyles()
		{
			normalLabel = new GUIStyle("TV Line");
			disabledLabel = new GUIStyle("PR DisabledLabel");
			prefabLabel = new GUIStyle("PR PrefabLabel");
			disabledPrefabLabel = new GUIStyle("PR DisabledPrefabLabel");
			brokenPrefabLabel = new GUIStyle("PR BrokenPrefabLabel");
			disabledBrokenPrefabLabel = new GUIStyle("PR DisabledBrokenPrefabLabel");
			sceneOptionsButton = new GUIStyle("PaneOptions");
			sceneHeader = new GUIStyle("ProjectBrowserTopBarBg");
			openPrefabButton = new GUIStyle("ArrowNavigationRight");
			hoveredItemBackgroundStyle = new GUIStyle("WhiteBackground");

			var padding = normalLabel.padding.Clone();
			disabledLabel.padding = padding;
			prefabLabel.padding = padding;
			disabledPrefabLabel.padding = padding;
			brokenPrefabLabel.padding = padding;
			disabledBrokenPrefabLabel.padding = padding;

			normalLabel.richText = true;
			disabledLabel.richText = true;
			prefabLabel.richText = true;
			disabledPrefabLabel.richText = true;
			brokenPrefabLabel.richText = true;
			disabledBrokenPrefabLabel.richText = true;

			sceneHeader.fixedHeight = 0;
			sceneHeader.alignment = TextAnchor.MiddleCenter;
		}

		public static GUIStyle Label(bool isPrefab, bool isBroken, bool isDisabled)
		{
			if (isBroken)
			{
				if (isDisabled)
				{
					return disabledBrokenPrefabLabel;
				}
				else
				{
					return brokenPrefabLabel;
				}
			}
			else if (isPrefab)
			{
				if (isDisabled)
				{
					return disabledPrefabLabel;
				}
				else
				{
					return prefabLabel;
				}
			}
			else
			{
				if (isDisabled)
				{
					return disabledLabel;
				}
				else
				{
					return normalLabel;
				}
			}
		}

		public static GUIStyle normalLabel = "PR Label";

		public static GUIStyle disabledLabel = "PR DisabledLabel";

		public static GUIStyle prefabLabel = "PR PrefabLabel";

		public static GUIStyle disabledPrefabLabel = "PR DisabledPrefabLabel";

		public static GUIStyle brokenPrefabLabel = "PR BrokenPrefabLabel";

		public static GUIStyle disabledBrokenPrefabLabel = "PR DisabledBrokenPrefabLabel";
		
		public static GUIStyle sceneOptionsButton = "PaneOptions";

		public static GUIStyle sceneHeader = "ProjectBrowserTopBarBg";

		public static GUIStyle openPrefabButton = "ArrowNavigationRight";

		public static GUIStyle hoveredItemBackgroundStyle = "WhiteBackground";
	}
}