namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public enum MergeScriptsOption
	{
		Never,

		WithoutIcons,

		Always,
	}

	public static class XMergeScriptsOption
	{
		public static bool Merge(this MergeScriptsOption merging, bool hasIcon)
		{
			switch (merging)
			{
				case MergeScriptsOption.Never: return false;
				case MergeScriptsOption.Always: return true;
				case MergeScriptsOption.WithoutIcons: return !hasIcon;
				default: throw merging.Unexpected();
			}
		}
	}
}