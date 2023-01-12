using System;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;
using UnityEngine.Profiling;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(ProjectToolbars))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class ProjectToolbars
	{
		private static readonly ToolbarControlProvider toolbarControlProvider = new ToolbarControlProvider(ToolbarWindow.Project);

		static ProjectToolbars()
		{
			EditorApplication.projectWindowItemOnGUI += OnProjectItemGUI;
		}

		private static void OnProjectItemGUI(string guid, Rect position)
		{
			try
			{
				Profiler.BeginSample("Peek." + nameof(ProjectToolbars));

				GuiCallback.Process();

				var isList = position.height <= EditorGUIUtility.singleLineHeight;

				if (!isList)
				{
					return;
				}

				if (!PeekPlugin.Configuration.enableProjectToolbars)
				{
					return;
				}

				// Note: We can't properly handle sub-assets, because all we get is the GUID.
				var path = AssetDatabase.GUIDToAssetPath(guid);

				// Note: Force loading assets causes massive memory pressure on big projects
				if (!AssetDatabase.IsMainAssetAtPathLoaded(path))
				{
					return;
				}

				position.xMin++;

				var fullRowPosition = position;
				fullRowPosition.xMax += 0;
				fullRowPosition.xMin -= 16;

				var target = AssetDatabase.LoadMainAssetAtPath(path);

				var isFocused = false;

				try
				{
					isFocused = ((EditorWindow)UnityEditorDynamic.ProjectBrowser.s_LastInteractedProjectBrowser).IsFocused();
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"Failed to determine if hierarchy window was focused:\n{ex}");
				}

				TreeViewToolbars.OnItemGUI(toolbarControlProvider, target, position, fullRowPosition, isFocused);

				if (position.Contains(Event.current.mousePosition))
				{
					EditorApplication.RepaintProjectWindow();
				}
			}
			finally
			{
				Profiler.EndSample();
			}
		}
	}
}