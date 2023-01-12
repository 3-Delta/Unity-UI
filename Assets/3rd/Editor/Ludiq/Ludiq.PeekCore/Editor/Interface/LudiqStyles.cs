using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore 
{
	public static class LudiqStyles
	{
		static LudiqStyles()
		{
			// General
			
			centeredLabel = new GUIStyle(EditorStyles.label);
			centeredLabel.alignment = TextAnchor.MiddleCenter;
			centeredLabel.margin = new RectOffset(0, 0, 5, 5);
			centeredLabel.wordWrap = true;

			richLabel = new GUIStyle(EditorStyles.label);
			richLabel.richText = true;

			horizontalSeparator = ColorPalette.unityBackgroundVeryDark.CreateBackground();
			horizontalSeparator.fixedHeight = 1;
			horizontalSeparator.stretchWidth = true;

			verticalSeparator = ColorPalette.unityBackgroundVeryDark.CreateBackground();
			verticalSeparator.fixedWidth = 1;
			verticalSeparator.stretchHeight = true;

			expandedTooltip = new GUIStyle(EditorStyles.label);
			expandedTooltip.normal.textColor = ColorPalette.unityForegroundDim;
			expandedTooltip.wordWrap = true;
			expandedTooltip.fontSize = 10;
			expandedTooltip.padding = new RectOffset(2, 5, 0, 10);

			paddedButton = new GUIStyle("Button");
			paddedButton.padding = new RectOffset(10, 10, 5, 5);

			textAreaWordWrapped = new GUIStyle(EditorStyles.textArea);
			textAreaWordWrapped.wordWrap = true;

			textFieldWordWrapped = new GUIStyle(EditorStyles.textField);
			textFieldWordWrapped.wordWrap = true;

			spinnerButton = new GUIStyle("MiniToolbarButton");
			spinnerButton.padding = new RectOffset(0, 0, 0, 0);
			spinnerButton.imagePosition = ImagePosition.ImageOnly;
			spinnerButton.alignment = TextAnchor.MiddleCenter;
			spinnerButton.fixedWidth = 0;
			spinnerButton.fixedHeight = 0;

			spinnerDownArrow = new GUIStyle("IN Dropdown").normal.background;

			largePopup = new GUIStyle(EditorStyles.popup);
			largePopup.border = new RectOffset(3, 16, 3, 3);
			largePopup.fontSize = 11;
			largePopup.fixedHeight = 20;

			mixedToggle = new GUIStyle("ToggleMixed");
			
			componentTitle = new GUIStyle(EditorStyles.boldLabel);
			
			componentTitleField = new GUIStyle(EditorStyles.textField);

			componentTitleFieldHidable = new GUIStyle(componentTitleField);
			componentTitleFieldHidable.hover.background = componentTitleFieldHidable.normal.background;
			componentTitleFieldHidable.normal.background = ColorPalette.transparent.GetPixel();

			componentTitlePlaceholder = new GUIStyle(EditorStyles.label);
			componentTitlePlaceholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
			componentTitlePlaceholder.padding = componentTitleField.padding;
			componentTitlePlaceholder.fontSize = componentTitleField.fontSize;

			// Search
			
			searchField = new GUIStyle("SearchTextField");
			searchFieldCancelButton = new GUIStyle("SearchCancelButton");
			searchFieldCancelButtonEmpty = new GUIStyle("SearchCancelButtonEmpty");
			searchFieldBackground = ColorPalette.unityBackgroundMid.CreateBackground();
			searchFieldBackground.padding = new RectOffset(6, 6, 7, 7);

			// Headers

			headerBackground = new GUIStyle("IN BigTitle");
			headerBackground.margin = new RectOffset(0, 0, 0, 5);

			// Show smaller icons on high DPI displays,
			// and crisp 32x icons on standard DPI displays
			
			if (EditorGUIUtility.pixelsPerPoint >= 2)
			{
				headerBackground.padding = new RectOffset(8, 8, 8, 9);
			}
			else
			{
				headerBackground.padding = new RectOffset(8, 6, 6, 7);
			}

			headerTitle = new GUIStyle(EditorStyles.label);
			headerTitle.fontSize = 13;
			headerTitle.wordWrap = true;
			headerTitle.padding.bottom = 0;

			headerSummary = new GUIStyle(EditorStyles.label);
			headerSummary.wordWrap = true;

			headerIcon = new GUIStyle();

			if (EditorGUIUtility.pixelsPerPoint >= 2)
			{
				headerIcon.fixedWidth = 20;
				headerIcon.fixedHeight = 20;
				headerIcon.margin = new RectOffset(0, 8, 3, 0);
			}
			else
			{
				headerIcon.fixedWidth = 32;
				headerIcon.fixedHeight = 32;
				headerIcon.margin = new RectOffset(0, 6, 3, 0);
			}

			headerTitleField = new GUIStyle(EditorStyles.textField);
			headerTitleField.fontSize = 13;
			headerTitleField.fixedHeight = 19;

			headerTitleFieldHidable = new GUIStyle(headerTitleField);
			headerTitleFieldHidable.hover.background = headerTitleFieldHidable.normal.background;
			headerTitleFieldHidable.normal.background = ColorPalette.transparent.GetPixel();

			headerTitlePlaceholder = new GUIStyle(EditorStyles.label);
			headerTitlePlaceholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
			headerTitlePlaceholder.padding = headerTitleField.padding;
			headerTitlePlaceholder.fontSize = headerTitleField.fontSize;

			headerSummaryField = new GUIStyle(EditorStyles.textArea);

			headerSummaryFieldHidable = new GUIStyle(headerSummaryField);
			headerSummaryFieldHidable.hover.background = headerSummaryFieldHidable.normal.background;
			headerSummaryFieldHidable.normal.background = ColorPalette.transparent.GetPixel();

			headerSummaryPlaceholder = new GUIStyle(EditorStyles.label);
			headerSummaryPlaceholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
			headerSummaryPlaceholder.padding = EditorStyles.textField.padding;
			
			// Lists

			listBackground = ColorPalette.unityBackgroundLight.CreateBackground();

			listRow = new GUIStyle();
			listRow.fontSize = 13;
			listRow.richText = true;
			listRow.alignment = TextAnchor.MiddleRight;
			listRow.padding = new RectOffset(18, 8, 10, 10);

			var normalBackground = ColorPalette.transparent.GetPixel();
			var selectedBackground = ColorPalette.unitySelectionHighlight.GetPixel();
			var normalForeground = ColorPalette.unityForeground;
			var selectedForeground = ColorPalette.unityForegroundSelected;

			listRow.normal.background = normalBackground;
			listRow.normal.textColor = normalForeground;
			listRow.onNormal.background = selectedBackground;
			listRow.onNormal.textColor = selectedForeground;

			listRow.active.background = normalBackground;
			listRow.active.textColor = normalForeground;
			listRow.onActive.background = selectedBackground;
			listRow.onActive.textColor = selectedForeground;

			listRow.border = new RectOffset(1, 1, 1, 1);

			// Toolbars

			toolbarBackground = new GUIStyle(EditorStyles.toolbar);
			toolbarButton = new GUIStyle(EditorStyles.toolbarButton);
			toolbarButton.alignment = TextAnchor.MiddleCenter;
			toolbarButton.padding.right-=2;
			toolbarPopup = new GUIStyle(EditorStyles.toolbarPopup);

			toolbarBreadcrumbRoot = new GUIStyle("GUIEditor.BreadcrumbLeft");
			toolbarBreadcrumbRoot.alignment = TextAnchor.MiddleCenter;
			toolbarBreadcrumbRoot.padding.bottom++;
			toolbarBreadcrumbRoot.fontSize = 9;
			toolbarBreadcrumbRoot.margin.left = 0;

			toolbarBreadcrumb = new GUIStyle("GUIEditor.BreadcrumbMid");
			toolbarBreadcrumb.alignment = TextAnchor.MiddleCenter;
			toolbarBreadcrumb.padding.bottom++;
			toolbarBreadcrumb.fontSize = 9;

			toolbarLabel = new GUIStyle(EditorStyles.label);
			toolbarLabel.alignment = TextAnchor.MiddleCenter;
			toolbarLabel.padding = new RectOffset(2, 2, 2, 2);
			toolbarLabel.fontSize = 9;

			// Windows

			windowHeaderBackground = new GUIStyle("IN BigTitle");
			windowHeaderBackground.margin = new RectOffset(0, 0, 0, 0);
			windowHeaderBackground.padding = new RectOffset(10, 10, 20, 20);

			windowHeaderTitle = new GUIStyle(EditorStyles.label);
			windowHeaderTitle.padding = new RectOffset(0, 0, 0, 0);
			windowHeaderTitle.margin = new RectOffset(0, 0, 0, 0);
			windowHeaderTitle.alignment = TextAnchor.MiddleCenter;
			windowHeaderTitle.fontSize = 14;

			windowHeaderIcon = new GUIStyle();
			windowHeaderIcon.imagePosition = ImagePosition.ImageOnly;
			windowHeaderIcon.alignment = TextAnchor.MiddleCenter;
			windowHeaderIcon.fixedWidth = IconSize.Medium;
			windowHeaderIcon.fixedHeight = IconSize.Medium;

			windowHeaderTitle.fixedHeight = headerIcon.fixedHeight;

			windowBackground = ColorPalette.unityBackgroundMid.CreateBackground();

			// Buttons
			buttonLeft = new GUIStyle("ButtonLeft");
			buttonMid = new GUIStyle("ButtonMid");
			buttonRight = new GUIStyle("ButtonRight");

			// Command Buttons
			var commandButtonExtraBottomMargin = (EditorGUIUtility.isProSkin && !LudiqGUIUtility.isFlatSkin);
			commandButton = new GUIStyle("Command");
			commandButtonLeft = new GUIStyle("CommandLeft");
			commandButtonMid = new GUIStyle("CommandMid");
			commandButtonRight = new GUIStyle("CommandRight");
			// commandButton.richText = true;
			// commandButtonLeft.richText = true;
			// commandButtonMid.richText = true;
			// commandButtonRight.richText = true;
			commandButton.imagePosition = ImagePosition.ImageLeft;
			commandButtonLeft.imagePosition = ImagePosition.ImageLeft;
			commandButtonMid.imagePosition = ImagePosition.ImageLeft;
			commandButtonRight.imagePosition = ImagePosition.ImageLeft;
			commandButton.alignment = TextAnchor.MiddleLeft;
			commandButtonLeft.alignment = TextAnchor.MiddleLeft;
			commandButtonMid.alignment = TextAnchor.MiddleLeft;
			commandButtonRight.alignment = TextAnchor.MiddleLeft;
			commandButton.fixedWidth = 0;
			commandButtonLeft.fixedWidth = 0;
			commandButtonMid.fixedWidth = 0;
			commandButtonRight.fixedWidth = 0;
			commandButton.margin = new RectOffset(0, 0, 0, 0);
			commandButtonLeft.margin = new RectOffset(0, 0, 0, 0);
			commandButtonMid.margin = new RectOffset(0, 0, 0, 0);
			commandButtonRight.margin = new RectOffset(0, 0, 0, 0);
			commandButton.padding = new RectOffset(6, 6, 0, commandButtonExtraBottomMargin ? 2 : 1);
			commandButtonLeft.padding = new RectOffset(6, 6, 0, commandButtonExtraBottomMargin ? 2 : 1);
			commandButtonMid.padding = new RectOffset(6, 6, 0, commandButtonExtraBottomMargin ? 2 : 1);
			commandButtonRight.padding = new RectOffset(6, 6, 0, commandButtonExtraBottomMargin ? 2 : 1);
			commandButton.fontSize = 10;
			commandButtonLeft.fontSize = 10;
			commandButtonMid.fontSize = 10;
			commandButtonRight.fontSize = 10;

			commandButtonSoft = new GUIStyle(commandButton);
			commandButtonLeftSoft = new GUIStyle(commandButtonLeft);
			commandButtonMidSoft = new GUIStyle(commandButtonMid);
			commandButtonRightSoft = new GUIStyle(commandButtonRight);
			commandButtonSoft.onNormal.background = commandButtonSoft.active.background;
			commandButtonLeftSoft.onNormal.background = commandButtonLeftSoft.active.background;
			commandButtonMidSoft.onNormal.background = commandButtonMidSoft.active.background;
			commandButtonRightSoft.onNormal.background = commandButtonRightSoft.active.background;
			commandButtonSoft.active.background = commandButtonSoft.onActive.background;
			commandButtonLeftSoft.active.background = commandButtonLeftSoft.onActive.background;
			commandButtonMidSoft.active.background = commandButtonMidSoft.onActive.background;
			commandButtonRightSoft.active.background = commandButtonRightSoft.onActive.background;
			
			commandButtonCompact = new GUIStyle(commandButton);
			commandButtonLeftCompact = new GUIStyle(commandButtonLeft);
			commandButtonMidCompact = new GUIStyle(commandButtonMid);
			commandButtonRightCompact = new GUIStyle(commandButtonRight);
			commandButtonCompact.padding.left = commandButtonCompact.padding.right = 4;
			commandButtonLeftCompact.padding.right = 4;
			commandButtonRightCompact.padding.left = 4;
			commandButtonMidCompact.padding.left = commandButtonMidCompact.padding.right = 4;


			// Big Buttons
			bigButton = new GUIStyle("Button");
			bigButton.padding = new RectOffset(12, 12, 8, 8);
			bigButton.fixedWidth = 220;
			bigButton.fixedHeight = 62;
			bigButton.stretchWidth = true;
			bigButton.stretchHeight = true;

			bigButtonTitle = new GUIStyle(EditorStyles.label);
			bigButtonTitle.fontSize = 12;
			bigButtonTitle.fixedHeight = 18;

			bigButtonSubtitle = new GUIStyle(EditorStyles.label);
			bigButtonSubtitle.fontSize = 10;
			bigButtonSubtitle.wordWrap = true;

			// Object Fields

			objectFieldTarget = new GUIStyle("IN ObjectField");
			objectFieldTarget.fixedHeight = EditorGUIUtility.singleLineHeight;
			objectFieldTarget.fixedWidth = 14;
			
			objectFieldThumbnailBackground = new GUIStyle(EditorStyles.textField);
			objectFieldThumbnailBackground.fixedHeight = 0;

			objectFieldThumbnailForeground = new GUIStyle("IN ObjectField");
			objectFieldThumbnailForeground.fixedHeight = 0;
			objectFieldThumbnailForeground.border = new RectOffset(4, 14, 4, 14);
		}

		// General
		public static readonly GUIStyle centeredLabel;
		public static readonly GUIStyle richLabel;
		public static readonly GUIStyle horizontalSeparator;
		public static readonly GUIStyle verticalSeparator;
		public static readonly GUIStyle expandedTooltip;
		public static readonly GUIStyle paddedButton;
		public static readonly float compactHorizontalSpacing = 2;
		public static readonly GUIStyle textAreaWordWrapped;
		public static readonly GUIStyle textFieldWordWrapped;
		public static readonly GUIStyle spinnerButton;
		public static readonly Texture2D spinnerDownArrow;
		public static readonly GUIStyle largePopup;
		public static readonly GUIStyle mixedToggle;

		public static readonly GUIStyle componentTitle;
		public static readonly GUIStyle componentTitleField;
		public static readonly GUIStyle componentTitleFieldHidable;
		public static readonly GUIStyle componentTitlePlaceholder;

		// Search
		public static readonly GUIStyle searchField;
		public static readonly GUIStyle searchFieldCancelButton;
		public static readonly GUIStyle searchFieldCancelButtonEmpty;
		public static readonly GUIStyle searchFieldBackground;
		public static readonly float searchFieldInnerHeight = 20;
		public static readonly float searchFieldOuterHeight = 32;

		// Headers
		public static readonly GUIStyle headerBackground;
		public static readonly GUIStyle headerIcon;
		public static readonly GUIStyle headerTitle;
		public static readonly GUIStyle headerTitleField;
		public static readonly GUIStyle headerTitleFieldHidable;
		public static readonly GUIStyle headerTitlePlaceholder;
		public static readonly GUIStyle headerSummary;
		public static readonly GUIStyle headerSummaryField;
		public static readonly GUIStyle headerSummaryFieldHidable;
		public static readonly GUIStyle headerSummaryPlaceholder;

		// Lists
		public static readonly GUIStyle listRow;
		public static readonly GUIStyle listBackground;

		// Toolbars
		public static readonly GUIStyle toolbarBackground;
		public static readonly GUIStyle toolbarButton;
		public static readonly GUIStyle toolbarPopup;
		public static readonly GUIStyle toolbarBreadcrumbRoot;
		public static readonly GUIStyle toolbarBreadcrumb;
		public static readonly GUIStyle toolbarLabel;

		// Windows
		public static readonly GUIStyle windowHeaderBackground;
		public static readonly GUIStyle windowHeaderTitle;
		public static readonly GUIStyle windowHeaderIcon;
		public static readonly GUIStyle windowBackground;
		public static readonly float spaceBetweenWindowHeaderIconAndTitle = 14;

		// Type trees
		public static readonly float typeTreeIndentation = 8;

		// Buttons
		public static readonly GUIStyle buttonLeft;
		public static readonly GUIStyle buttonMid;
		public static readonly GUIStyle buttonRight;

		// Command Buttons
		public static readonly float commandButtonWidth = 32;

		public static readonly GUIStyle commandButton;
		public static readonly GUIStyle commandButtonLeft;
		public static readonly GUIStyle commandButtonMid;
		public static readonly GUIStyle commandButtonRight;

		public static readonly GUIStyle commandButtonCompact;
		public static readonly GUIStyle commandButtonLeftCompact;
		public static readonly GUIStyle commandButtonMidCompact;
		public static readonly GUIStyle commandButtonRightCompact;

		public static readonly GUIStyle commandButtonSoft;
		public static readonly GUIStyle commandButtonLeftSoft;
		public static readonly GUIStyle commandButtonMidSoft;
		public static readonly GUIStyle commandButtonRightSoft;

		public static readonly float spaceBetweenCommandToolbars = 10;

		public static GUIStyle CommandButton(bool closeLeft, bool closeRight)
		{
			if (closeLeft && closeRight)
			{
				return commandButton;
			}
			else if (closeLeft)
			{
				return commandButtonLeft;
			}
			else if (closeRight)
			{
				return commandButtonRight;
			}
			else
			{
				return commandButtonMid;
			}
		}

		public static GUIStyle CommandButtonCompact(bool closeLeft, bool closeRight)
		{
			if (closeLeft && closeRight)
			{
				return commandButtonCompact;
			}
			else if (closeLeft)
			{
				return commandButtonLeftCompact;
			}
			else if (closeRight)
			{
				return commandButtonRightCompact;
			}
			else
			{
				return commandButtonMidCompact;
			}
		}

		public static GUIStyle CommandButtonSoft(bool closeLeft, bool closeRight)
		{
			if (closeLeft && closeRight)
			{
				return commandButtonSoft;
			}
			else if (closeLeft)
			{
				return commandButtonLeftSoft;
			}
			else if (closeRight)
			{
				return commandButtonRightSoft;
			}
			else
			{
				return commandButtonMidSoft;
			}
		}

		// Big Buttons
		public static readonly GUIStyle bigButton;
		public static readonly GUIStyle bigButtonTitle;
		public static readonly GUIStyle bigButtonSubtitle;
		public static readonly float bigButtonIconSize = IconSize.Medium;
		public static readonly float spaceAfterBigButtonIcon = 8;

		// Object Field
		
		public static readonly GUIStyle objectFieldTarget;
		public static readonly GUIStyle objectFieldThumbnailBackground;
		public static readonly GUIStyle objectFieldThumbnailForeground;
	}
}