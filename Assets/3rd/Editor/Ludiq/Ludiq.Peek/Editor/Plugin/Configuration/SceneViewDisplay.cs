namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public enum SceneViewDisplay
	{
		Never = 0,

		Always = 1,

		FullscreenOnly = 2
	}

	public static class XSceneViewDisplay
	{
		public static bool Display(this SceneViewDisplay display, bool isMaximized)
		{
			switch (display)
			{
				case SceneViewDisplay.Never: return false;
				case SceneViewDisplay.Always: return true;
				case SceneViewDisplay.FullscreenOnly: return isMaximized;
				default: throw display.Unexpected();
			}
		}
	}
}