using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class PeekStyles
	{
		static PeekStyles()
		{
			treeViewTool = new GUIStyle();
			treeViewTool.imagePosition = ImagePosition.ImageOnly;

			treeViewTooltip = new GUIStyle(EditorStyles.miniButton);
			treeViewTooltip.fixedHeight = 16;
			treeViewTooltip.padding = new RectOffset(6, 6, 0, 0);
			treeViewTooltip.margin = new RectOffset(0, 0, 0, 0);

			sceneViewTooltip = new GUIStyle(EditorStyles.miniButton);
			sceneViewTooltip.fixedHeight = 16;
			sceneViewTooltip.padding = new RectOffset(6, 6, 0, 0);
			sceneViewTooltip.margin = new RectOffset(0, 0, 0, 2);

			treeViewMoreButton = new GUIStyle();
			treeViewMoreButton.fixedHeight = 16;
			treeViewMoreButton.padding = new RectOffset(0, 0, 4, 0);
			treeViewMoreButton.margin = new RectOffset(0, 0, 0, 0);
			treeViewMoreButton.alignment = TextAnchor.MiddleCenter;

			pinButton = new GUIStyle();
			pinButton.fixedHeight = IconSize.Small;
			pinButton.fixedWidth = IconSize.Small;
			pinButton.normal.background = PeekPlugin.Icons.pin?[IconSize.Small];
			pinButton.normal.scaledBackgrounds = new [] { PeekPlugin.Icons.pin?[IconSize.Medium] };
			pinButton.onNormal.background = PeekPlugin.Icons.pinOn?[IconSize.Small];
			pinButton.onNormal.scaledBackgrounds = new [] { PeekPlugin.Icons.pinOn?[IconSize.Medium] };
			//pinButton.hover.background = pinButton.onNormal.background;
			//pinButton.hover.scaledBackgrounds = pinButton.onNormal.scaledBackgrounds;
			//pinButton.onHover.background = pinButton.normal.background;
			//pinButton.onHover.scaledBackgrounds = pinButton.normal.scaledBackgrounds;
			pinButton.padding = new RectOffset(0, 0, 0, 0);
			pinButton.margin = new RectOffset(0, 0, 0, 0);
		}

		public static readonly GUIStyle pinButton;

		// Tree View
		public static readonly GUIStyle treeViewTool;

		public static readonly GUIStyle treeViewTooltip;

		public static readonly GUIStyle treeViewMoreButton;

		// Scene View
		public static GUIStyle SceneViewTool(bool closeLeft, bool closeRight)
		{
			return LudiqStyles.CommandButtonCompact(closeLeft, closeRight);
		}

		public static readonly GUIStyle sceneViewTooltip;

		public static readonly Vector2 tabsScreenMargin = new Vector2(16, 18);
	}
}