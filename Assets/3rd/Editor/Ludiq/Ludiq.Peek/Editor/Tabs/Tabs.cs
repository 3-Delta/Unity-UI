using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(Tabs))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class Tabs
	{
		private static Event e => Event.current;

		private static readonly ToolbarControlProvider toolbarControlProvider = new ToolbarControlProvider(ToolbarWindow.Scene);

		public static TabsToolbar toolbar { get; }

		public static ToolbarControl toolbarControl { get; }

		private static bool reopenedTabs;
		
		private static EditorWindow lastFocusedWindow;

		private static bool lastFocusedWindowWasMaximized;

		static Tabs()
		{
			toolbar = new TabsToolbar();
			toolbar.Initialize();

			toolbarControl = toolbarControlProvider.GetControl(toolbar);
			toolbarControl.isDraggable = true;
			toolbarControl.isActivator = true;

			ShortcutsIntegration.secondaryToolbar = toolbarControl;
			EditorApplication.update += WatchWindowLayout;
			toolbarControlProvider.cleaningUp += SaveOpenTabs;
		}

		private static void SaveOpenTabs()
		{
			PeekPlugin.Configuration.openTabs.Clear();

			foreach (var tool in toolbar.OfType<TabTool>())
			{
				if (tool is TabTool tabTool && tabTool.isActive)
				{
					PeekPlugin.Configuration.openTabs.Add(tabTool.tabKey);
				}
			}

			PeekPlugin.Configuration.Save(nameof(PeekConfiguration.openTabs));
		}

		private static void ReopenTabs()
		{
			foreach (var tabKey in PeekPlugin.Configuration.openTabs)
			{
				foreach (var tabTool in toolbar.OfType<TabTool>())
				{
					if (tabTool.tabKey == tabKey)
					{
						tabTool.Open(toolbarControl.GetToolControl(tabTool));
						break;
					}
				}
			}
		}

		private static void WatchWindowLayout()
		{
			var focusedWindow = EditorWindow.focusedWindow;
			var isMaximized = focusedWindow != null && focusedWindow.maximized;

			if (focusedWindow != lastFocusedWindow ||
			    isMaximized != lastFocusedWindowWasMaximized)
			{
				AnalyzeWindowLayout();
				lastFocusedWindow = focusedWindow;
				lastFocusedWindowWasMaximized = isMaximized;
			}
		}

		private static void AnalyzeWindowLayout()
		{
			var tabs = ListPool<EditorWindow>.New();
			
			try
			{
				foreach (var window in Resources.FindObjectsOfTypeAll<EditorWindow>())
				{
					// Skip invalid windows
					if (window == null)
					{
						continue;
					}

					// Abort the operation if any window is maximized, we don't want to save that layout
					if (window.maximized)
					{
						return;
					}

					// Skip windows that are invalid or not part of the layout
					dynamic dWindow = window.AsDynamic();

					if (dWindow.m_Parent == null || dWindow.m_Parent.window == null || dWindow.m_Parent.window.m_DontSaveToLayout)
					{
						continue;
					}

					var parentWindowShowMode = (int)dWindow.m_Parent.window.showMode;

					// Skip windows not in the main window (4 in the ShowMode enum)
					if (parentWindowShowMode != 4)
					{
						continue;
					}

					tabs.Add(window);
				}

				// Sort tabs by screen position
				tabs.Sort(compareLayoutTab);

				// Store the tabs in the configuration
				// To minimize serialization operations, first check if changed from last time
				var config = PeekPlugin.Configuration;
				var tabsChanged = tabs.Count != config.tabsInLayout.Count;
				var positionsChanged = false;
				var dataChanged = false;
				
				// Store the fact that this tab is in the layout, which we'll need as fallback
				// in case the assembly reloads while the scene view is already maximized
				if (!tabsChanged)
				{
					for (int i = 0; i < tabs.Count; i++)
					{
						var tabKey = Codebase.SerializeType(tabs[i].GetType());
						var configTabKey = config.tabsInLayout[i];

						if (tabKey != configTabKey)
						{
							tabsChanged = true;
							break;
						}
					}
				}

				if (tabsChanged)
				{
					config.tabsInLayout.Clear();

					foreach (var tab in tabs)
					{
						var tabKey = Codebase.SerializeType(tab.GetType());
						config.tabsInLayout.Add(tabKey);
					}

					config.Save(nameof(PeekConfiguration.tabsInLayout));
				}

				foreach (var tab in tabs)
				{
					var tabKey = Codebase.SerializeType(tab.GetType());
					
					// Store the position if the user hasn't configured it already
					if (!config.tabsPositions.ContainsKey(tabKey))
					{
						config.tabsPositions[tabKey] = tab.position;
						positionsChanged = true;
					}

					// Store the data if the user hasn't configured it already
					if (!config.tabsData.ContainsKey(tabKey))
					{
						config.tabsData[tabKey] = EditorJsonUtility.ToJson(tab);
						dataChanged = true;
					}
				}

				if (positionsChanged)
				{
					config.Save(nameof(PeekConfiguration.tabsPositions));
				}

				if (dataChanged)
				{
					config.Save(nameof(PeekConfiguration.tabsData));
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to analyze window layout:\n{ex}");
			}
			finally
			{
				tabs.Free();
			}
		}

		private static readonly Comparison<EditorWindow> compareLayoutTab = CompareLayoutTab;

		private static int CompareLayoutTab(EditorWindow a, EditorWindow b)
		{
			var xComparison = a.position.x.CompareTo(b.position.x);

			if (xComparison != 0)
			{
				return xComparison;
			}

			var yComparison = a.position.y.CompareTo(b.position.y);

			if (yComparison != 0)
			{
				return yComparison;
			}
			
			// TODO: Compare tab order when docked in same container
			return 0;
		}

		internal static void OnSceneGUI(SceneView sceneView)
		{
			if (!PeekPlugin.Configuration.enableTabs.Display(sceneView.maximized))
			{
				// TODO: Generalize, make faster, and also call the Move event when moving the window.
				toolbarControl?.CloseAllTools();
				return;
			}

			Profiler.BeginSample("Peek." + nameof(Tabs));

			if (toolbarControl == null || !toolbarControl.toolbar.isValid)
			{
				return;
			}

			toolbarControl.toolbar.Update();

			var position = sceneView.GetInnerGuiPosition();

			Handles.BeginGUI();

			var toolbarSize = toolbarControl.GetSceneViewSize();

			if (!toolbarControl.isDragging)
			{
				toolbarControl.guiPosition = new Rect
				(
					PeekPlugin.Configuration.tabsOrigin.x,
					PeekPlugin.Configuration.tabsOrigin.y,
					toolbarSize.x,
					toolbarSize.y
				);
			}

			toolbarControl.DrawInSceneView();
			
			if (!reopenedTabs)
			{
				// Wait until we drew once to set the GUI positions
				// ReopenTabs(); // Calling in delayCall to avoid occasional dWindow.parent crash in TabTool.SaveWindowPosition 
				EditorApplication.delayCall += ReopenTabs;
				reopenedTabs = true;
			}

			if (e.type == EventType.Repaint)
			{
				var origin = toolbarControl.guiPosition.position;

				var margin = PeekStyles.tabsScreenMargin;

				origin.x = Mathf.Clamp
				(
					origin.x,
					margin.x,
					position.width - toolbarSize.x - margin.x
				);

				origin.y = Mathf.Clamp
				(
					origin.y,
					margin.y,
					position.height - toolbarSize.y - margin.y
				);

				PeekPlugin.Configuration.tabsOrigin = origin;

				if (toolbarControl.isDragging)
				{
					PeekPlugin.Configuration.Save(nameof(PeekConfiguration.tabsOrigin));
				}
			}

			if (toolbarControl.guiPosition.Contains(e.mousePosition))
			{
				sceneView.Repaint();
				SceneViewIntegration.Use();
			}

			Handles.EndGUI();

			Profiler.EndSample();
		}
	}
}