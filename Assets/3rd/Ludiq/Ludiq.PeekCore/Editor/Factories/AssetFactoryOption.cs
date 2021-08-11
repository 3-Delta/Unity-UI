namespace Ludiq.PeekCore
{
	public sealed class AssetFactoryOption : FactoryOption
	{
		public AssetFactoryOption(IFactory factory, IFactoryConfiguration configuration = null, bool showNewIcon = true) : base(factory, configuration, showNewIcon)
		{
			label += "...";
		}

		protected override bool Func(out object o)
		{
			var result = AssetFactoryUtility.TryCreateAndSave(factory, configuration, out var asset);
			o = asset;
			return result;
		}
	}
}
