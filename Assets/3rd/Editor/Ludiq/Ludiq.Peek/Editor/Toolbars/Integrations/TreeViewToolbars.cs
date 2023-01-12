using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class TreeViewToolbars
	{
		private static Event e => Event.current;

		public static void Draw(bool drawIcon, ToolbarControlProvider toolbarControlProvider, UnityObject target, UnityObject[] targets, bool isSelected, string label, Rect contentPosition, Rect rowPosition, bool hasFocus)
		{
			var isHovered = rowPosition.Contains(Event.current.mousePosition);

			try
			{
				// TODO: We can hook into AssetsTreeViewGUI.postAssetIconDrawCallback
				// to draw under the VCS integration icons

				var leftPadding = 0;

				if (toolbarControlProvider.window == ToolbarWindow.Project)
				{
					if (UnityEditor.VersionControl.Provider.enabled)
					{
						leftPadding = 9;
					}
					else
					{
						leftPadding = 2;
					}
				}

				var iconPosition = new Rect
				(
					contentPosition.x + leftPadding,
					contentPosition.y,
					IconSize.Small,
					IconSize.Small
				);
			
				if (drawIcon)
				{
					if (PeekPlugin.Configuration.enablePreviewIcons && PreviewUtility.TryGetPreview(target, out var preview) && preview != null)
					{
						GUI.DrawTexture(iconPosition, preview);

						if (target is GameObject gameObject && PrefabUtility.IsAddedGameObjectOverride(gameObject))
						{
							GUI.DrawTexture(iconPosition, PeekPlugin.Icons.prefabOverlayAdded?[(int)iconPosition.width]);
						}
					}
				}

				if (isHovered || isSelected)
				{
					var toolbar = ObjectToolbarProvider.GetToolbar(targets);

					if (!toolbar.isValid)
					{
						return;
					}

					toolbar.Update();
					var toolbarControl = toolbarControlProvider.GetControl(toolbar, target);

					toolbarControl.DrawMainToolInTreeView(iconPosition, contentPosition);

					var nameWidth = EditorStyles.label.CalcSize(LudiqGUIUtility.TempContent(label)).x;

					var maxStripWidth = contentPosition.width - nameWidth - IconSize.Small;
					var desiredStripWidth = toolbarControl.GetTreeViewWidth();
					var stripWidth = Mathf.Min(desiredStripWidth, maxStripWidth);

					float stripX;

					switch (PeekPlugin.Configuration.treeViewToolbarAlignment)
					{
						case TreeViewToolbarAlignment.Left:
							stripX = iconPosition.xMax + nameWidth;
							break;

						case TreeViewToolbarAlignment.Right:
							stripX = contentPosition.xMax - stripWidth;
							break;

						default: throw PeekPlugin.Configuration.treeViewToolbarAlignment.Unexpected();
					}

					var stripPosition = new Rect
					(
						stripX,
						contentPosition.y,
						stripWidth,
						contentPosition.height
					);

					toolbarControl.guiPosition = stripPosition;
					toolbarControl.DrawInTreeView(contentPosition, isSelected && hasFocus);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemGUI(ToolbarControlProvider toolbarControlProvider, UnityObject target, Rect contentPosition, Rect rowPosition, bool hasFocus)
		{
			if (target == null)
			{
				return;
			}

			var isSelected = Selection.objects.Contains(target);

			UnityObject[] targets;

			if (isSelected)
			{
				targets = Selection.objects;

				if (PeekPlugin.Configuration.enableQuickDeselect &&
				    e.type == EventType.KeyDown && 
					e.keyCode == KeyCode.Escape && 
				    e.modifiers == EventModifiers.None)
				{
					Selection.activeObject = null;
					e.Use();
				}
			}
			else
			{
				targets = new[] {target};
			}

			Draw(true, toolbarControlProvider, target, targets, isSelected, target.name, contentPosition, rowPosition, hasFocus);
		}
	}
}