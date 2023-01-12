using System.Linq;
using Ludiq.Peek;
using UnityEditor;
using UnityEngine;
using GenericMenu = UnityEditor.GenericMenu;
using UnityObject = UnityEngine.Object;

[assembly: RegisterEditorTool(typeof(GameObject), typeof(GameObjectEditorTool))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class GameObjectEditorTool : EditorTool<GameObject>
	{
		public GameObjectEditorTool(GameObject[] targets) : base(targets)
		{

		}

		public override Texture2D overlay
		{
			get
			{
				if (hasSingleTarget && PrefabUtility.IsAddedGameObjectOverride(target))
				{
					return PeekPlugin.Icons.prefabOverlayAdded?[IconSize.Small];
				}

				return base.overlay;
			}
		}

		private bool creatorShortcut
		{
			get
			{
				var e = Event.current;
				return e != null && e.CtrlOrCmd() && e.shift && !e.alt;
			}
		}

		public override string tooltip
		{
			get
			{
				if (creatorShortcut)
				{
					return "Replace...";
				}

				return base.tooltip;
			}
		}

		public override void Open(ToolControl control)
		{
			if (creatorShortcut)
			{
				activatorControl = control;
				PopupWatcher.Release(window);
				window = Replacer.Open(targets, activatorControl.activatorScreenPosition);
				PopupWatcher.Watch(window);
			}
			else
			{
				base.Open(control);
			}
		}

		public override void OpenContextual(ToolControl control)
		{
			activatorControl = control;
			var menu = new GenericMenu();
			menu.allowDuplicateNames = true;
			GameObjectContextMenu.Fill(targets, menu, activatorControl.activatorScreenPosition, null);
			menu.DropDown(activatorControl.activatorGuiPosition);
		}

		public void OpenHierarchy(ToolControl control)
		{
			activatorControl = control;
			var activatorPosition = activatorControl.activatorScreenPosition;
			activatorPosition.width = 300;
			PopupWatcher.Release(window);
			window = HierarchyPopup.Show(targets, activatorPosition);
			PopupWatcher.Watch(window);
		}

		public override GUIStyle SceneViewStyle(bool closeLeft, bool closeRight)
		{
			if (hasSingleTarget)
			{
				if (PrefabUtility.IsPrefabAssetMissing(target))
				{
					return Styles.Scene(closeLeft, closeRight, true, true);
				}
				else if (PrefabUtility.IsPartOfPrefabInstance(target))
				{
					return Styles.Scene(closeLeft, closeRight, true, false);
				}
			}

			return base.SceneViewStyle(closeLeft, closeRight);
		}

		public override bool isDimmed => hasSingleTarget && !target.activeSelf;

		private static class Styles
		{
			static Styles()
			{
				scene_normal = ColoredLabel(PeekStyles.SceneViewTool(true, true), false, false);
				scene_prefab = ColoredLabel(PeekStyles.SceneViewTool(true, true), true, false);
				scene_broken = ColoredLabel(PeekStyles.SceneViewTool(true, true), true, true);

				scene_left_normal = ColoredLabel(PeekStyles.SceneViewTool(true, false), false, false);
				scene_left_prefab = ColoredLabel(PeekStyles.SceneViewTool(true, false), true, false);
				scene_left_broken = ColoredLabel(PeekStyles.SceneViewTool(true, false), true, true);

				scene_mid_normal = ColoredLabel(PeekStyles.SceneViewTool(false, false), false, false);
				scene_mid_prefab = ColoredLabel(PeekStyles.SceneViewTool(false, false), true, false);
				scene_mid_broken = ColoredLabel(PeekStyles.SceneViewTool(false, false), true, true);

				scene_right_normal = ColoredLabel(PeekStyles.SceneViewTool(false, true), false, false);
				scene_right_prefab = ColoredLabel(PeekStyles.SceneViewTool(false, true), true, false);
				scene_right_broken = ColoredLabel(PeekStyles.SceneViewTool(false, true), true, true);
			}

			private static readonly GUIStyle scene_normal;
			private static readonly GUIStyle scene_prefab;
			private static readonly GUIStyle scene_broken;

			private static readonly GUIStyle scene_left_normal;
			private static readonly GUIStyle scene_left_prefab;
			private static readonly GUIStyle scene_left_broken;

			private static readonly GUIStyle scene_mid_normal;
			private static readonly GUIStyle scene_mid_prefab;
			private static readonly GUIStyle scene_mid_broken;

			private static readonly GUIStyle scene_right_normal;
			private static readonly GUIStyle scene_right_prefab;
			private static readonly GUIStyle scene_right_broken;

			public static GUIStyle Scene(bool isFirst, bool isLast, bool isPrefab, bool isBroken)
			{
				if (isFirst && isLast)
				{
					if (isBroken)
					{
						return scene_broken;
					}
					else if (isPrefab)
					{
						return scene_prefab;
					}
					else
					{
						return scene_normal;
					}
				}
				else if (isFirst)
				{
					if (isBroken)
					{
						return scene_left_broken;
					}
					else if (isPrefab)
					{
						return scene_left_prefab;
					}
					else
					{
						return scene_left_normal;
					}
				}
				else if (isLast)
				{
					if (isBroken)
					{
						return scene_right_broken;
					}
					else if (isPrefab)
					{
						return scene_right_prefab;
					}
					else
					{
						return scene_right_normal;
					}
				}
				else
				{
					if (isBroken)
					{
						return scene_mid_broken;
					}
					else if (isPrefab)
					{
						return scene_mid_prefab;
					}
					else
					{
						return scene_mid_normal;
					}
				}
			}

			private static GUIStyle ColoredLabel(GUIStyle original, bool isPrefab, bool isBroken)
			{
				var label = new GUIStyle(original);

				if (isBroken)
				{
					label.normal.textColor = HierarchyStyles.brokenPrefabLabel.normal.textColor;
					label.onNormal.textColor = HierarchyStyles.brokenPrefabLabel.onNormal.textColor;
				}
				else if (isPrefab)
				{
					label.normal.textColor = HierarchyStyles.prefabLabel.normal.textColor;
					label.onNormal.textColor = HierarchyStyles.prefabLabel.onNormal.textColor;
				}

				return label;
			}
		}
	}
}