using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(SceneMaximizerIntegration))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class SceneMaximizerIntegration
	{
		private static Event e => Event.current;

		internal static void OnSceneGUI(SceneView sceneView)
		{
			if (SceneViewIntegration.used)
			{
				return;
			}

			var position = sceneView.GetInnerGuiPosition(); 

			Handles.BeginGUI();

			if (PeekPlugin.Configuration.maximizeShortcut.Check(e) &&
			    position.Contains(e.mousePosition))
			{
				GUIUtility.hotControl = 0;
				sceneView.maximized = !sceneView.maximized;
				GUIUtility.hotControl = 0;
				e.Use();
				SceneViewIntegration.Use();
			}

			Handles.EndGUI();
		}
	}
}