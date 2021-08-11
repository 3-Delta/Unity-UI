using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Ludiq.PeekCore
{
	[InitializeOnLoad]
	public static class EditorApplicationUtility
	{
		static EditorApplicationUtility()
		{
			Recursion.safeMode = true;
			OptimizedReflection.safeMode = true;
			Ensure.IsActive = true;

			Selection.selectionChanged += OnSelectionChange;
			EditorApplication.projectChanged += OnProjectChange;
			EditorApplication.hierarchyChanged += OnHierarchyChange;
			Undo.undoRedoPerformed += OnUndoRedo;
			PrefabUtility.prefabInstanceUpdated += OnPrefabChange;
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Restart %#&r", priority = LudiqProduct.InternalToolsMenuPriority)]
#endif
		public static void Restart()
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorApplication.OpenProject(Environment.CurrentDirectory);
		}
		
#region Version

		private static SemanticVersion? _unityVersion;

		private static readonly SemanticVersion fallbackUnityVersion = "2017.4.0";

		public static SemanticVersion unityVersion
		{
			get
			{
				if (_unityVersion == null)
				{
					var unityVersionString = Application.unityVersion;
					
					if (SemanticVersion.TryParse(unityVersionString, out var parsedUnityVersion))
					{
						_unityVersion = parsedUnityVersion;
					}
					else
					{
						Debug.LogWarning($"Failed to parse Unity version string '{unityVersionString}', falling back to {fallbackUnityVersion}");
						_unityVersion = fallbackUnityVersion;
					}
				}

				return _unityVersion.Value;
			}
		}

#endregion



#region Assembly Lock
		
		public static OverrideStack<bool> isAssemblyReloadLocked { get; } = new OverrideStack<bool>(false);

		public static void LockReloadAssemblies()
		{
			isAssemblyReloadLocked.BeginOverride(true);
			EditorApplication.LockReloadAssemblies();
		}

		public static void UnlockReloadAssemblies()
		{
			EditorApplication.UnlockReloadAssemblies();
			isAssemblyReloadLocked.EndOverride();
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Force Unlock Assembly Reload", priority = LudiqProduct.InternalToolsMenuPriority + 602)]
#endif
		private static void ForceUnlockReloadAssemblies()
		{
			EditorApplication.UnlockReloadAssemblies();
		}

#endregion


		
#region Events

		public static event Action onAssemblyReload;

		public static event Action onEnterPlayMode;

		public static event Action onExitPlayMode;

		public static event Action onEnterEditMode;

		public static event Action onExitEditMode;

		public static event Action onModeChange;

		public static event Action onPause;

		public static event Action onResume;

		public static event Action onPauseChange;

		public static event Action onSelectionChange;

		public static event Action onProjectChange;

		public static event Action onHierarchyChange;

		public static event Action onUndoRedo;

		public static event Action<GameObject> onPrefabChange;

		private static void OnSelectionChange()
		{
			if (PluginContainer.initialized)
			{
				LudiqGUIUtility.BeginNotActuallyOnGUI();
				onSelectionChange?.Invoke();
				LudiqGUIUtility.EndNotActuallyOnGUI();
			}
		}

		private static void OnProjectChange()
		{
			if (PluginContainer.initialized)
			{
				LudiqGUIUtility.BeginNotActuallyOnGUI();
				onProjectChange?.Invoke();
				LudiqGUIUtility.EndNotActuallyOnGUI();
			}
		}

		private static void OnHierarchyChange()
		{
			if (PluginContainer.initialized)
			{
				LudiqGUIUtility.BeginNotActuallyOnGUI();
				onHierarchyChange?.Invoke();
				LudiqGUIUtility.EndNotActuallyOnGUI();
			}
		}

		private static void OnUndoRedo()
		{
			if (PluginContainer.initialized)
			{
				LudiqGUIUtility.BeginNotActuallyOnGUI();
				onUndoRedo?.Invoke();
				LudiqGUIUtility.EndNotActuallyOnGUI();
			}
		}

		private static void OnPrefabChange(GameObject instance)
		{
			if (PluginContainer.initialized)
			{
				// LudiqGUIUtility.BeginNotActuallyOnGUI();
				onPrefabChange?.Invoke(instance);
				// LudiqGUIUtility.EndNotActuallyOnGUI();
			}
		}

		internal static void BeforeInitializeAfterPlugins()
		{
			EditorApplication.pauseStateChanged += delegate(PauseState pauseState)
			{
				switch (pauseState)
				{
					case PauseState.Paused:
						onPause?.Invoke();
						break;
					case PauseState.Unpaused:
						onResume?.Invoke();
						break;
				}

				onPauseChange?.Invoke();
			};

			EditorApplication.playModeStateChanged += delegate(PlayModeStateChange stateChange)
			{
				switch (stateChange)
				{
					case PlayModeStateChange.EnteredEditMode:
						onEnterEditMode?.Invoke();
						break;
					case PlayModeStateChange.ExitingEditMode:
						onExitEditMode?.Invoke();
						break;
					case PlayModeStateChange.EnteredPlayMode:
						onEnterPlayMode?.Invoke();
						break;
					case PlayModeStateChange.ExitingPlayMode:
						onExitPlayMode?.Invoke();
						break;
				}

				onModeChange?.Invoke();
			};
		}

		internal static void AfterInitializeAfterPlugins()
		{
			onAssemblyReload?.Invoke();

			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// Playmode state changed does not get called when
				// the editor assemblies load, therefore we have to 
				// manually invoke enter edit mode. 

				// This won't cause a double invoke because oddly, 
				// assemblies do not get reloaded when you exit
				// play mode and get back into the edit mode. This may
				// cause issues somewhere in the Ludiq / Bolt source,
				// because most of it was coded assuming that they did.
				onEnterEditMode?.Invoke();
			}
		}
		
#endregion
	}
}