using UnityEditor;

namespace Ludiq.PeekCore
{
	/// <summary>
	/// Provides a way to display background progress in a unique flat progress bar.
	/// Use this as a background equivalent to EditorUtility.DisplayProgressBar.
	/// This utility uses the Progress API in Unity 2020+, and AsyncProgressBar in prior versions.
	/// </summary>
	public static class BackgroundProgress
	{
		static BackgroundProgress()
		{
			EditorApplication.update += Update;
		}

		private static readonly object @lock = new object();

		private static bool shouldClear;
		private static string description;
		private static float proportion;

#if UNITY_2020_1_OR_NEWER
		private static int progressItemId = -1;
#endif

		public static void Report(string title, float progress)
		{
			lock (@lock)
			{
				description = title;
				proportion = progress;
			}
		}

		public static void Clear()
		{
			lock (@lock)
			{
				shouldClear = true;
				description = null;
				proportion = 0;
			}
		}

		private static void Update()
		{
			lock (@lock)
			{
				if (shouldClear)
				{
#if UNITY_2020_1_OR_NEWER
					if (progressItemId != -1)
					{
						progressItemId = Progress.Remove(progressItemId);
					}
#else
					AsyncProgressBarWrapper.Clear();
#endif

					shouldClear = false;
				}

				if (description != null)
				{
#if UNITY_2020_1_OR_NEWER
					if (progressItemId == -1)
					{
						progressItemId = Progress.Start("Ludiq");
					}

					Progress.Report(progressItemId, proportion, description);
#else
					AsyncProgressBarWrapper.Display(description, proportion);
#endif
				}
			}
		}
	}
}