#if !UNITY_2020_1_OR_NEWER
using System;
using System.Reflection;
using UnityEditor;

namespace Ludiq.PeekCore
{
	public static class AsyncProgressBarWrapper
	{
		static AsyncProgressBarWrapper()
		{
			try
			{
				AsyncProgressBarType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AsyncProgressBar", true);
				AsyncProgressBar_Display = AsyncProgressBarType.GetMethod("Display", BindingFlags.Static | BindingFlags.Public);
				AsyncProgressBar_Clear = AsyncProgressBarType.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

				if (AsyncProgressBar_Display == null)
				{
					throw new MissingMemberException(AsyncProgressBarType.FullName, "Display");
				}

				if (AsyncProgressBar_Clear == null)
				{
					throw new MissingMemberException(AsyncProgressBarType.FullName, "Clear");
				}
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		private static readonly Type AsyncProgressBarType; // internal sealed class AsyncProgressBar

		private static readonly MethodInfo AsyncProgressBar_Display; // public static extern void AsyncStatusBar.Display(string progressInfo, float progress);

		private static readonly MethodInfo AsyncProgressBar_Clear; // public static extern void AsyncStatusBar.Clear();

		public static void Display(string progressInfo, float progress)
		{
			try
			{
				AsyncProgressBar_Display.InvokeOptimized(null, progressInfo, progress);
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		public static void Clear()
		{
			try
			{
				AsyncProgressBar_Clear.InvokeOptimized(null);
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}
	}
}
#endif