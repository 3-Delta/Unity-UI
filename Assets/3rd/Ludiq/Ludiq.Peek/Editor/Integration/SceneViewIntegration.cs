using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[assembly: InitializeAfterPlugins(typeof(SceneViewIntegration))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class SceneViewIntegration
	{
		static SceneViewIntegration()
		{
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui += OnSceneGUI;
#else
			SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
		}

		public static bool used { get; private set; }

		public static void Use()
		{
			used = true;
		}

		private static void OnSceneGUI(SceneView sceneView)
		{
			GuiCallback.Process();

			used = false;

			// Optim: we don't care about MouseMove because we get constant Repaint anyway
			if (Event.current.type == EventType.MouseMove)
			{
				return;
			}

			// Optim: we don't use Layout, skip for another massive boost
			if (Event.current.type == EventType.Layout)
			{
				// TODO: We can't do this optims because the fact that we call GetControlID in 
				// ToolControl.DropdownToggle offsets the IDs across events and thus
				// breaks the handles. We'd need to do like in Bolt, AKA fetch all the CIDs
				// on all events, but don't render anything after that.

				// return;
			}

			Tabs.OnSceneGUI(sceneView);

			SceneToolbars.OnSceneGUI(sceneView);
			
			Probe.OnSceneGUI(sceneView);

			Creator.OnSceneGUI(sceneView);
			
			SceneMaximizerIntegration.OnSceneGUI(sceneView);
			
			SceneDeselectIntegration.OnSceneGUI(sceneView);

			SceneHierarchyIntegration.OnSceneGUI(sceneView);
		}
	}
}