using UnityEditor;

namespace Ludiq.PeekCore
{
	public static class ProgressUtility
	{
		public static void DisplayProgressBar(string title, string info, float progress)
		{
			if (UnityThread.allowsAPI)
			{
				EditorUtility.DisplayProgressBar(title, info, progress);
			}
			else
			{
				BackgroundProgress.Report(title, progress);
			}
		}

#if LUDIQ_DEVELOPER
		[MenuItem("Tools/Peek/Ludiq/Developer/Force Clear Progress Bar", priority = LudiqProduct.InternalToolsMenuPriority + 601)]
#endif
		public static void ClearProgressBar()
		{
			if (UnityThread.allowsAPI)
			{
				EditorUtility.ClearProgressBar();
			}

			BackgroundProgress.Clear();
		}
	}
}