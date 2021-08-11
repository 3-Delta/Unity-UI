using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(SceneToolbars))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class SceneHierarchyIntegration
	{
		private static Event e => Event.current;

		static SceneHierarchyIntegration() { }

		internal static void OnSceneGUI(SceneView sceneView)
		{
			if (PeekPlugin.Configuration.sceneHierarchyShortcut.Check(e))
			{
				var width = Mathf.Min(sceneView.position.width * 0.9f, 400);

				var activator = new Rect
				(
					(sceneView.position.width - width) / 2,
					-1,
					width,
					0
				);

				activator = LudiqGUIUtility.GUIToScreenRect(activator);

				HierarchyPopup.Show(activator);
				e.Use();
			}
		}
	}
}