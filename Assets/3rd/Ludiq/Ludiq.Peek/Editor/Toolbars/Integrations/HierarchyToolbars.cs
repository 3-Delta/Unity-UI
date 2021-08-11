using System;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

[assembly: InitializeAfterPlugins(typeof(HierarchyToolbars))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class HierarchyToolbars
	{
		private static readonly ToolbarControlProvider toolbarControlProvider = new ToolbarControlProvider(ToolbarWindow.Hierarchy);

		static HierarchyToolbars()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
		}

		private static void OnHierarchyItemGUI(int instanceID, Rect position)
		{
			GuiCallback.Process();

			if (!PeekPlugin.Configuration.enableHierarchyToolbars)
			{
				return;
			}

			Profiler.BeginSample("Peek." + nameof(HierarchyToolbars));

			var target = EditorUtility.InstanceIDToObject(instanceID);

			var fullRowPosition = position;
			fullRowPosition.xMax += 16;
			fullRowPosition.xMin -= 32;

			var isFocused = false;

			try
			{
				isFocused = UnityEditorDynamic.SceneHierarchyWindow.lastInteractedHierarchyWindow.sceneHierarchy.m_TreeView.HasFocus();
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to determine if hierarchy window was focused:\n{ex}");
			}

			position.xMax -= Mathf.Abs(PeekPlugin.Configuration.hierarchyToolbarsOffset);

			TreeViewToolbars.OnItemGUI(toolbarControlProvider, target, position, fullRowPosition, isFocused);

			Profiler.EndSample();

			if (fullRowPosition.Contains(Event.current.mousePosition))
			{
				EditorApplication.RepaintHierarchyWindow();
			}
		}
	}
}