using System;
using System.Collections.Generic;
using System.Linq;
using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: MapToPlugin(typeof(PeekConfiguration), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class PeekConfiguration : PluginConfiguration
	{
		private PeekConfiguration(PeekPlugin plugin) : base(plugin) { }

		#region Editor Prefs

		/// <summary>
		/// Whether strips should be shown in the hierarchy window.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Hierarchy Toolbars")]
		public bool enableHierarchyToolbars { get; set; } = true;

		/// <summary>
		/// Whether strips should be shown in the project window.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Project Toolbars")]
		public bool enableProjectToolbars { get; set; } = true;
		
		/// <summary>
		/// Where to align the tool strips in the tree view.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Toolbar Alignment")]
		public TreeViewToolbarAlignment treeViewToolbarAlignment { get; set; } = TreeViewToolbarAlignment.Right;

		/// <summary>
		/// The distance from the hierarchy edge where Peek toolbars should be rendered (in pixels).
		/// Use this to resolve compatibility with third-party plugins that also use the hierarchy.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Hierarchy Toolbars Offset")]
		public int hierarchyToolbarsOffset { get; set; } = 0;

		/// <summary>
		/// Whether toolbars should be shown around the selection in the scene view.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Scene Toolbars")]
		public SceneViewDisplay enableSceneToolbars { get; set; } = SceneViewDisplay.Always;
		
		/// <summary>
		/// Whether toolbars should be shown around the selection in the scene view.
		/// This can be toggled quickly using the B key.
		/// </summary>
		[EditorPref]
		public bool displaySceneToolbars { get; set; } = true;

		/// <summary>
		/// When scripts should be combined as a single tool.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Merge Scripts")]
		public MergeScriptsOption scriptsMerging { get; set; } = MergeScriptsOption.WithoutIcons;

		/// <summary>
		/// Whether a window strip should be shown in the scene view.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Tabs")]
		public SceneViewDisplay enableTabs { get; set; } = SceneViewDisplay.FullscreenOnly;

		/// <summary>
		/// The window type scene tabs should open with by default.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Tabs Mode")]
		public TabsMode tabsMode { get; set; } = TabsMode.Pinned;
		
		[EditorPref]
		[InspectorLabel("Tabs Titles")]
		public bool showTabsTitles { get; set; } = false;
		
		[EditorPref]
		[TypeFilter(typeof(EditorWindow), NonPublic = true, Abstract = false)]
		public List<Type> tabsOrder { get; set; } = new List<Type>()
		{
			(Type)UnityEditorDynamic.SceneHierarchyWindow,
			(Type)UnityEditorDynamic.ProjectBrowser,
			(Type)UnityEditorDynamic.ConsoleWindow,
			(Type)UnityEditorDynamic.GameView
		};
		
		[EditorPref(visible = false)]
		public List<string> tabsInLayout { get; set; } = new List<string>();

		[EditorPref(visible = false)]
		public Dictionary<string, string> tabsData { get; set; } = new Dictionary<string, string>();

		[EditorPref(visible = false)]
		public Dictionary<string, Rect> tabsPositions { get; set; } = new Dictionary<string, Rect>();
		
		[EditorPref(visible = false)]
		public Vector2 tabsOrigin { get; set; } = new Vector2(16, 4);
		
		[EditorPref(visible = false)]
		public List<string> openTabs { get; set; } = new List<string>();

		[EditorPref]
		[InspectorLabel("Persistent Pins (Experimental)")]
		public bool persistentPinnedEditors { get; set; } = false;
		
		[EditorPref]
		[InspectorLabel("Inspector Popup Width")]
		public int editorPopupWidth { get; set; } = 330;

		/// <summary>
		/// Whether an inspector peek tool should be shown next to object reference drawers.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Reference Inspector")]
		public bool enableReferenceInspector { get; set; } = true;

		/// <summary>
		/// Whether asset previews should be used as icons whenever available.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Preview Icons")]
		public bool enablePreviewIcons { get; set; } = true;

		/// <summary>
		/// Whether the object-passthrough probe should be enabled.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Probe")]
		public bool enableProbe { get; set; } = true;

		/// <summary>
		/// Whether the quick game object creator should be enabled in the scene view.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Creator")]
		public SceneViewDisplay enableCreator { get; set; } = SceneViewDisplay.Always;

		/// <summary>
		/// Where the creator should place new objects in the hierarchy.
		/// Root: At the root.
		/// Sibling: After the object under the cursor, if any.
		/// Sibling Outside Prefabs: Like siblings, but never within a prefab instance.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Creator Parenting")]
		public CreatorParenting creatorParenting { get; set; } = CreatorParenting.SiblingOutsidePrefabs;

		/// <summary>
		/// Whether the replacer should preserve the scale.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Preserve Scale on Replace")]
		public bool preserveScaleOnReplace { get; set; } = false;

		/// <summary>
		/// Whether pressing Escape should clear the selection. 
		/// </summary>
		[EditorPref]
		[InspectorLabel("Quick Deselect")]
		public bool enableQuickDeselect { get; set; } = true;
		
		/// <summary>
		/// Whether strips should be shown in the hierarchy window.
		/// </summary>
		[EditorPref]
		[InspectorLabel("Hierarchy Framing")]
		public HierarchyFramingOption hierarchyFraming { get; set; } = HierarchyFramingOption.WhenOutOfView;
		
		[EditorPref]
		[InspectorLabel("Sticky Drag & Drop")]
		public bool enableStickyDragAndDrop { get; set; } = true;
		
		[EditorPref]
		[InspectorLabel("Drop Activation Delay")]
		public float dropActivationDelay { get; set; } = 1;
		
		[EditorPref]
		[InspectorLabel("Probe ProBuilder Radius")]
		[InspectorRange(1, 64)]
		public int probeProBuilderRadius { get; set; } = 16;
		
		[EditorPref]
		[InspectorLabel("Probe ProBuilder Depth Test")]
		public bool probeProBuilderDepthTest { get; set; } = true;

		#endregion



		#region Project Settings

		/// <summary>
		/// The layers on which the scene creator should pick objects via raycast.
		/// </summary>
		[ProjectSetting]
		public LayerMask probeLayerMask { get; set; } = Physics.DefaultRaycastLayers; 
		
		/// <summary>
		/// The maximum number of results in the probe.
		/// </summary>
		[EditorPref]
		public int probeLimit { get; set; } = 50;
		
		/// <summary>
		/// Whether the scene creator should include primitives.
		/// </summary>
		[ProjectSetting]
		public bool createPrimitives { get; set; } = true;
		
		/// <summary>
		/// Whether the scene creator should include prefabs.
		/// </summary>
		[ProjectSetting]
		public bool createPrefabs { get; set; } = true;
		
		/// <summary>
		/// Whether the scene creator should include models.
		/// </summary>
		[ProjectSetting]
		public bool createModels { get; set; } = true;
		
		/// <summary>
		/// Whether the scene creator should include sprites.
		/// </summary>
		[ProjectSetting]
		public bool createSprites { get; set; } = true;

		/// <summary>
		/// Whether newly created objects should use their bounds to align with the target instead of their pivot.
		/// </summary>
		[ProjectSetting]
		public bool createOnBounds { get; set; } = true;

		/// <summary>
		/// How big the 3D creator gizmo should be in units.
		/// </summary>
		[ProjectSetting]
		public float creatorUnitSize { get; set; } = 1;

		/// <summary>
		/// A list of menu item paths to ignore from the GameObject creation menu.
		/// Should be formatted as "GameObject/{Folder}/Item".
		/// </summary>
		[ProjectSetting]
		public List<string> createMenuBlacklist { get; set; } = new List<string>();

		/// <summary>
		/// A list of project folder paths to ignore from the GameObject creation menu.
		/// Should be formatted as "Assets/{Folder}".
		/// </summary>
		[ProjectSetting]
		public List<string> createFolderBlacklist { get; set; } = new List<string>();

		/// <summary>
		/// A list of window types that should always be included in tabs, even if absent
		/// from the current layout.
		/// </summary>
		[ProjectSetting]
		[TypeFilter(typeof(EditorWindow), NonPublic = true, Abstract = false)]
		public List<Type> tabsWhitelist { get; set; } = new List<Type>();

		/// <summary>
		/// A list of window types that shouldn't be included in tabs.
		/// </summary>
		[ProjectSetting]
		[TypeFilter(typeof(EditorWindow), NonPublic = true, Abstract = false)]
		public List<Type> tabsBlacklist { get; set; } = new List<Type>()
		{
			UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.InspectorWindow"),
			UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.SceneView"),
		};

		#endregion


		#region Shortcuts

		[EditorPref]
		[InspectorLabel("Toggle Toolbar Shortcut")]
		public KeyboardShortcut toggleToolbarShortcut { get; set; } = new KeyboardShortcut(KeyCode.B, ShortcutModifiers.None);

		[EditorPref]
		[InspectorLabel("Creator Shortcut")]
		public PolyShortcut creatorShortcut { get; set; } = new PolyShortcut(MouseShortcutButton.Left, ShortcutModifiers.Action | ShortcutModifiers.Shift);

		[EditorPref]
		[InspectorLabel("Probe Shortcut")]
		public PolyShortcut probeShortcut { get; set; } = new PolyShortcut(MouseShortcutButton.Right, ShortcutModifiers.None);

		[EditorPref]
		[InspectorLabel("Maximize Shortcut")]
		public PolyShortcut maximizeShortcut { get; set; } = new PolyShortcut(MouseShortcutButton.Left, ShortcutModifiers.None, MouseShortcutAction.DoubleClick);

		[EditorPref]
		[InspectorLabel("Scene Hierarchy Shortcut")]
		public KeyboardShortcut sceneHierarchyShortcut { get; set; } = new KeyboardShortcut(KeyCode.F, ShortcutModifiers.Action);

		[EditorPref]
		[InspectorLabel("Selection Hierarchy Shortcut")]
		public KeyboardShortcut selectionHierarchyShortcut { get; set; } = new KeyboardShortcut(KeyCode.Space, ShortcutModifiers.None);

		#endregion
	}
}