using Ludiq.PeekCore;
using UnityEditor;

[assembly: MapToPlugin(typeof(LudiqCoreResources), LudiqCore.ID)]

namespace Ludiq.PeekCore
{
	public sealed class LudiqCoreResources : PluginResources
	{
		private LudiqCoreResources(LudiqCore plugin) : base(plugin)
		{
			icons = new Icons(this);
		}

		public override void Initialize()
		{
			base.Initialize();

			icons.Load();

			loader = LoadTexture("Loader/Loader.png", CreateTextureOptions.PixelPerfect);
		}

		public Icons icons { get; private set; }

		public EditorTexture loader { get; private set; }

		public class Icons
		{
			public Icons(LudiqCoreResources resources)
			{
				this.resources = resources;
			}

			private readonly LudiqCoreResources resources;
			
			private readonly LazyDictionary<string, EditorTexture> storage = new LazyDictionary<string, EditorTexture>();

			public EditorTexture empty => storage[nameof(empty)];

			public EditorTexture progress => storage[nameof(progress)];
			public EditorTexture errorState => storage[nameof(errorState)];
			public EditorTexture successState => storage[nameof(successState)];
			public EditorTexture warningState => storage[nameof(warningState)];

			public EditorTexture informationMessage => storage[nameof(informationMessage)];
			public EditorTexture questionMessage => storage[nameof(questionMessage)];
			public EditorTexture warningMessage => storage[nameof(warningMessage)];
			public EditorTexture successMessage => storage[nameof(successMessage)];
			public EditorTexture errorMessage => storage[nameof(errorMessage)];

			public EditorTexture upgrade => storage[nameof(upgrade)];
			public EditorTexture upToDate => storage[nameof(upToDate)];
			public EditorTexture downgrade => storage[nameof(downgrade)];

			public EditorTexture supportWindow => storage[nameof(supportWindow)];
			public EditorTexture sidebarAnchorLeft => storage[nameof(sidebarAnchorLeft)];
			public EditorTexture sidebarAnchorRight => storage[nameof(sidebarAnchorRight)];

			public EditorTexture editorPref => storage[nameof(editorPref)];
			public EditorTexture projectSetting => storage[nameof(projectSetting)];

			public EditorTexture @null => storage[nameof(@null)];
			public EditorTexture generic => storage[nameof(generic)];
			public EditorTexture @new => storage[nameof(@new)];
			public EditorTexture folder => storage[nameof(folder)];
			
			public void Load()
			{
				storage.Bind(nameof(empty), () => EditorTexture.Single(ColorPalette.transparent.GetPixel()));

				// Messages
				storage.Bind(nameof(informationMessage), () => LudiqGUIUtility.LoadBuiltinTexture("console.infoicon"));
				storage.Bind(nameof(questionMessage), () => resources.LoadIcon("Icons/Messages/Question.png"));
				storage.Bind(nameof(warningMessage), () => LudiqGUIUtility.LoadBuiltinTexture("console.warnicon"));
				storage.Bind(nameof(successMessage), () => resources.LoadIcon("Icons/Messages/Success.png"));
				storage.Bind(nameof(errorMessage), () => LudiqGUIUtility.LoadBuiltinTexture("console.erroricon"));

				// States
				storage.Bind(nameof(warningState), () => LudiqGUIUtility.LoadBuiltinTexture("console.warnicon"));
				storage.Bind(nameof(successState), () => resources.LoadIcon("Icons/State/Success.png"));
				storage.Bind(nameof(errorState), () => LudiqGUIUtility.LoadBuiltinTexture("console.erroricon"));
				storage.Bind(nameof(progress), () => resources.LoadIcon("Icons/State/Progress.png"));

				// Versioning
				storage.Bind(nameof(upgrade), () => resources.LoadIcon("Icons/Versioning/Upgrade.png"));
				storage.Bind(nameof(upToDate), () => resources.LoadIcon("Icons/Versioning/UpToDate.png"));
				storage.Bind(nameof(downgrade), () => resources.LoadIcon("Icons/Versioning/Downgrade.png"));

				// Windows
				storage.Bind(nameof(supportWindow), () => resources.LoadIcon("Icons/Windows/SupportWindow.png"));
				storage.Bind(nameof(sidebarAnchorLeft), () => resources.LoadTexture("Icons/Windows/SidebarAnchorLeft.png", CreateTextureOptions.PixelPerfect));
				storage.Bind(nameof(sidebarAnchorRight), () => resources.LoadTexture("Icons/Windows/SidebarAnchorRight.png", CreateTextureOptions.PixelPerfect));

				// Configuration
				storage.Bind(nameof(editorPref), () => resources.LoadTexture("Icons/Configuration/EditorPref.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect));
				storage.Bind(nameof(projectSetting), () => resources.LoadTexture("Icons/Configuration/ProjectSetting.png", new TextureResolution[] { 12, 24 }, CreateTextureOptions.PixelPerfect));

				// Other
				storage.Bind(nameof(@null), () => resources.LoadIcon("Icons/Null.png"));
				storage.Bind(nameof(generic), () => resources.LoadIcon("Icons/Generic.png"));
				storage.Bind(nameof(@new), () => resources.LoadIcon("Icons/New.png"));
				storage.Bind(nameof(folder), () => EditorTexture.Single(AssetDatabase.GetCachedIcon("Assets")));
			}
		}
	}
}