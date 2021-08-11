using System;

namespace Ludiq.PeekCore
{
	public class LanguageIconSet
	{
		public LanguageIconSet(string name)
		{
			storage.Bind(nameof(@public), () => LoadAccessibility(name, false));
			storage.Bind(nameof(@private), () => LoadAccessibility(name + "_Private", false) ?? @public);
			storage.Bind(nameof(@protected), () => LoadAccessibility(name + "_Protected", false) ?? @private ?? @public);
			storage.Bind(nameof(@internal), () => LoadAccessibility(name + "_Internal", false) ?? @private ?? @public);
		}

		private readonly LazyDictionary<string, EditorTexture> storage = new LazyDictionary<string, EditorTexture>();

		public EditorTexture @public => storage[nameof(@public)];
		public EditorTexture @private => storage[nameof(@public)];
		public EditorTexture @protected => storage[nameof(@public)];
		public EditorTexture @internal => storage[nameof(@public)];

		public static LanguageIconSet Load(string name)
		{
			return new LanguageIconSet(name);
		}

		private static EditorTexture LoadAccessibility(string name, bool required)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			var path = $"Icons/Language/{Icons.Language.skin}/{name}.png";

			return LudiqCore.Resources.LoadIcon(path, required);
		}
	}
}