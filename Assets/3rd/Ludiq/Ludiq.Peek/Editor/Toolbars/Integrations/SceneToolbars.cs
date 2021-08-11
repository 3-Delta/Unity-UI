using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Handles = UnityEditor.Handles;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(SceneToolbars))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class SceneToolbars
	{
		private static Event e => Event.current;

		private static readonly ToolbarControlProvider toolbarControlProvider = new ToolbarControlProvider(ToolbarWindow.Scene);

		private static GameObject[] selectionToolbarTargets;

		private static Dictionary<SceneView, ToolbarControl> selectionToolbarControls { get; } = new Dictionary<SceneView, ToolbarControl>();

		public static ToolbarControl dragToolbarControl { get; private set; }

		public static bool dragToolbarLocked { get; set; }
		
		private static DateTime dragToolbarStart;
		
		public static GameObject dragToolbarTarget;

		static SceneToolbars()
		{
			EditorApplicationUtility.onSelectionChange += RefreshSelectionToolbar;
			EditorApplicationUtility.onProjectChange += RefreshSelectionToolbar;
			EditorApplicationUtility.onHierarchyChange += RefreshSelectionToolbar;
			RefreshSelectionToolbar();
		}

		public static void RefreshSelectionToolbar()
		{
			var sceneViews = SceneView.sceneViews.OfType<SceneView>().ToListPooled();
			var sceneViewsToRemove = HashSetPool<SceneView>.New();

			try
			{
				// Don't pick prefabs
				selectionToolbarTargets = Selection.transforms.Select(t => t.gameObject).ToArray();
				
				if (selectionToolbarTargets.Length > 0)
				{
					// Update controls for each open scene view
					foreach (var sceneView in sceneViews)
					{
						var toolbar = ObjectToolbarProvider.GetToolbar(selectionToolbarTargets);

						var toolbarControl = toolbarControlProvider.GetControl(toolbar, sceneView);

						selectionToolbarControls[sceneView] = toolbarControl;

						if (sceneView == SceneView.lastActiveSceneView)
						{
							ShortcutsIntegration.primaryToolbar = toolbarControl;
						}
					}
				}
				else
				{
					selectionToolbarControls.Clear();
					ShortcutsIntegration.primaryToolbar = null;
				}

				// Remove toolbars for closed scene views
				foreach (var selectionToolbarControl in selectionToolbarControls)
				{
					if (!sceneViews.Contains(selectionToolbarControl.Key))
					{
						sceneViewsToRemove.Add(selectionToolbarControl.Key);
					}
				}

				foreach (var sceneViewToRemove in sceneViewsToRemove)
				{
					selectionToolbarControls.Remove(sceneViewToRemove);
				}

				SceneView.RepaintAll();
			}
			finally
			{
				sceneViews.Free();
				sceneViewsToRemove.Free();
			}
		}

		private static void RefreshDragToolbar(SceneView sceneView)
		{
			if (e.type == EventType.DragUpdated && !dragToolbarLocked)
			{
				var previousDragToolbarTarget = dragToolbarTarget;

				dragToolbarTarget = HandleUtility.PickGameObject(e.mousePosition, true);

				if (dragToolbarTarget != null)
				{
					var dragToolbar = ObjectToolbarProvider.GetToolbar(dragToolbarTarget);
					dragToolbarControl = toolbarControlProvider.GetControl(dragToolbar, sceneView);
					sceneView.Repaint();
				}
				else
				{
					dragToolbarControl = null;
					dragToolbarLocked = false;
					sceneView.Repaint();
				}

				if (dragToolbarTarget != previousDragToolbarTarget)
				{
					dragToolbarStart = DateTime.UtcNow;
				}

				if (dragToolbarTarget != null && (DateTime.UtcNow - dragToolbarStart).TotalSeconds > PeekPlugin.Configuration.dropActivationDelay)
				{
					dragToolbarLocked = true;
				}
			}

			if (e.rawType == EventType.DragExited)
			{
				dragToolbarTarget = null;
				dragToolbarControl = null;
				dragToolbarLocked = false;
			}
		}
		
		internal static void OnSceneGUI(SceneView sceneView)
		{
			if (PeekPlugin.Configuration.toggleToolbarShortcut.Check())
			{
				PeekPlugin.Configuration.displaySceneToolbars = !PeekPlugin.Configuration.displaySceneToolbars;
				PeekPlugin.Configuration.Save();
				e.TryUse();
			}

			if (!PeekPlugin.Configuration.enableSceneToolbars.Display(sceneView.maximized) ||
			    !PeekPlugin.Configuration.displaySceneToolbars)
			{
				return;
			}

			Profiler.BeginSample("Peek." + nameof(SceneToolbars));

			try
			{

				Handles.BeginGUI();

				selectionToolbarControls.TryGetValue(sceneView, out var selectionToolbarControl);

				DrawToolbar(sceneView, selectionToolbarControl, selectionToolbarTargets);

				if (PeekPlugin.Configuration.enableStickyDragAndDrop)
				{
					if (dragToolbarControl != null)
					{
						EditorGUI.BeginDisabledGroup(!dragToolbarLocked);
						DrawToolbar(sceneView, dragToolbarControl, new[] {dragToolbarTarget});
						EditorGUI.EndDisabledGroup();
					}

					RefreshDragToolbar(sceneView);
				}

				if (PeekPlugin.Configuration.selectionHierarchyShortcut.Check(e))
				{
					if (OpenHierarchyTool(selectionToolbarControl))
					{
						e.Use();
					}
				}

				Handles.EndGUI();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			finally
			{
				Profiler.EndSample();
			}
		}

		private static void DrawToolbar(SceneView sceneView, ToolbarControl toolbarControl, GameObject[] targets)
		{
			if (toolbarControl == null || !toolbarControl.toolbar.isValid)
			{
				return;
			}
		
			toolbarControl.toolbar.Update();
			
			var stripSize = toolbarControl.GetSceneViewSize();

			float stripX, stripY;

			var position = sceneView.GetInnerGuiPosition();
			
			sceneView.CalculateGuiBounds(targets, out var guiBounds, out var guiCenter);

			if (guiBounds != null)
			{
				stripX = guiBounds.Value.center.x;
				stripY = guiBounds.Value.yMax + Styles.toolbarBoundsMargin;

				// LudiqGUI.DrawEmptyRect(guiBounds.Value, Color.red);
			}
			else if (guiCenter != null)
			{
				stripX = guiCenter.Value.x;
				stripY = guiCenter.Value.y + Styles.toolbarCenterMargin;
			}
			else
			{
				// Object is behind camera
				// We can't return because we might get a layout mismatch error.
				// And we can't do a safe check in Update because we only have a GUI callback.
				// So we just hide the toolbar later.
				stripX = 0;
				stripY = 0;
			}

			stripX -= stripSize.x / 2;

			stripX = Mathf.Clamp
			(
				stripX,
				Styles.toolbarScreenMargin,
				position.width - Styles.toolbarScreenMargin - stripSize.x
			);

			stripY = Mathf.Clamp
			(
				stripY,
				Styles.toolbarScreenMargin,
				position.height - Styles.toolbarScreenMargin - stripSize.y
			);

			toolbarControl.guiPosition = new Rect
			(
				stripX,
				stripY,
				stripSize.x,
				stripSize.y
			);

			var isWithinView = sceneView.IsWithinView(guiBounds, guiCenter);
			var alpha = isWithinView.HasValue ? (isWithinView.Value ? 1 : 0.5f) : 0;

			EditorGUI.BeginDisabledGroup(alpha == 0);
			
			using (LudiqGUI.color.Override(LudiqGUI.color.value.WithAlphaMultiplied(alpha)))
			{
				toolbarControl.DrawInSceneView();
			}

			EditorGUI.EndDisabledGroup();

			if (toolbarControl.guiPosition.Contains(e.mousePosition))
			{
				sceneView.Repaint();
				SceneViewIntegration.Use();
			}
		}
		
		private static bool OpenHierarchyTool(ToolbarControl selectionToolbarControl)
		{
			var gameObjectTool = selectionToolbarControl?.toolbar.OfType<GameObjectEditorTool>().FirstOrDefault();

			if (gameObjectTool == null)
			{
				return false;
			}

			var gameObjectToolControl = selectionToolbarControl.GetToolControl(gameObjectTool);

			gameObjectToolControl.Toggle();

			if (gameObjectTool.isActive)
			{
				gameObjectTool.Close(gameObjectToolControl);
			}

			gameObjectTool.OpenHierarchy(gameObjectToolControl);

			return true;
		}

		private static class Styles
		{
			static Styles() { }

			public static readonly float toolbarCenterMargin = 64;

			public static readonly float toolbarBoundsMargin = 24;

			public static readonly float toolbarScreenMargin = 16;
		}
	}
}