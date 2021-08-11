using Ludiq.Peek;
using Ludiq.PeekCore;

[assembly: MapToPlugin(typeof(PeekResources), PeekPlugin.ID)]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class PeekResources : PluginResources
	{
		private PeekResources(PeekPlugin plugin) : base(plugin)
		{
			icons = new Icons(this);
		}

		public override void Initialize()
		{
			base.Initialize();

			icons.Load();
		}

		public Icons icons { get; private set; }

		public class Icons
		{
			public Icons(PeekResources resources)
			{
				this.resources = resources;
			}

			private readonly PeekResources resources;

			private readonly LazyDictionary<string, EditorTexture> storage = new LazyDictionary<string, EditorTexture>();

			public EditorTexture addComponent => storage[nameof(addComponent)];

			public EditorTexture more => storage[nameof(more)];

			public EditorTexture createGameObjectOptions => storage[nameof(createGameObjectOptions)];

			public EditorTexture createGameObject => storage[nameof(createGameObject)];

			public EditorTexture replace => storage[nameof(replace)];

			public EditorTexture hierarchy => storage[nameof(hierarchy)];

			public EditorTexture toolbarDragHandle => storage[nameof(toolbarDragHandle)];

			public EditorTexture script => storage[nameof(script)];

			public EditorTexture pin => storage[nameof(pin)];

			public EditorTexture pinOn => storage[nameof(pinOn)];

			public EditorTexture prefab => storage[nameof(prefab)];

			public EditorTexture prefabOverlayAdded => storage[nameof(prefabOverlayAdded)];

			public EditorTexture prefabOverlayRemoved => storage[nameof(prefabOverlayRemoved)];

			public EditorTexture prefabOverlayModified => storage[nameof(prefabOverlayModified)];

			public EditorTexture propertyDrawer => storage[nameof(propertyDrawer)];

			public EditorTexture moreOverlay => storage[nameof(moreOverlay)];

			public EditorTexture inconsistentComponents => storage[nameof(inconsistentComponents)];

			public void Load()
			{
				storage.Bind(nameof(addComponent), () => LudiqGUIUtility.LoadBuiltinTexture("Toolbar Plus More"));
				storage.Bind(nameof(replace), () => resources.LoadIcon("Icons/Replace.png"));
				storage.Bind(nameof(more), () => LudiqGUIUtility.LoadBuiltinTexture("LookDevPaneOption", false) ?? LudiqGUIUtility.LoadBuiltinTexture("pane options"));
				storage.Bind(nameof(createGameObject), () => LudiqGUIUtility.LoadBuiltinTexture("Toolbar Plus"));
				storage.Bind(nameof(createGameObjectOptions), () => LudiqGUIUtility.LoadBuiltinTexture("Toolbar Plus More"));
				storage.Bind(nameof(hierarchy), () => LudiqGUIUtility.LoadBuiltinTexture("UnityEditor.SceneHierarchyWindow"));
				storage.Bind(nameof(toolbarDragHandle), () => null);
				storage.Bind(nameof(script), () => LudiqGUIUtility.LoadBuiltinTexture("cs Script Icon"));
				storage.Bind(nameof(pin), () => resources.LoadIcon("Icons/Pin.png"));
				storage.Bind(nameof(pinOn), () => resources.LoadIcon("Icons/PinOn.png"));
				storage.Bind(nameof(propertyDrawer), () => resources.LoadIcon("Icons/PropertyDrawer.png"));
				storage.Bind(nameof(moreOverlay), () => resources.LoadIcon("Icons/MoreOverlay.png"));
				storage.Bind(nameof(inconsistentComponents), () => LudiqGUIUtility.LoadBuiltinTexture("FilterByType"));
				storage.Bind(nameof(prefab), () => LudiqGUIUtility.LoadBuiltinTexture("Prefab Icon"));
				storage.Bind(nameof(prefabOverlayAdded), () => LudiqGUIUtility.LoadBuiltinTexture("PrefabOverlayAdded Icon"));
				storage.Bind(nameof(prefabOverlayRemoved), () => LudiqGUIUtility.LoadBuiltinTexture("PrefabOverlayRemoved Icon"));
				storage.Bind(nameof(prefabOverlayModified), () => LudiqGUIUtility.LoadBuiltinTexture("PrefabOverlayModified Icon"));
			}
		}
	}
}