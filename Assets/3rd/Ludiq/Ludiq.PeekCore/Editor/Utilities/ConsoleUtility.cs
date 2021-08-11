using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class ConsoleUtility
	{
		static ConsoleUtility()
		{
			try
			{
				ConsoleWindowType = UnityEditorDynamic.UnityEditorAssembly.GetType("UnityEditor.ConsoleWindow", true);
				ConsoleWindow_m_ActiveText = ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
				ConsoleWindow_m_ActiveInstanceID = ConsoleWindowType.GetField("m_ActiveInstanceID", BindingFlags.Instance | BindingFlags.NonPublic);

				if (ConsoleWindow_m_ActiveText == null)
				{
					throw new MissingMemberException(ConsoleWindowType.FullName, "m_ActiveText");
				}

				if (ConsoleWindow_m_ActiveInstanceID == null)
				{
					throw new MissingMemberException(ConsoleWindowType.FullName, "m_ActiveInstanceID");
				}
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}

			EditorApplication.update += WatchConsoleActivity;
		}

		public delegate void ConsoleEntryChanged(string text, int instanceID);

		public static event ConsoleEntryChanged entryChanged;

		private static readonly Type ConsoleWindowType;

		private static readonly FieldInfo ConsoleWindow_m_ActiveText;

		private static readonly FieldInfo ConsoleWindow_m_ActiveInstanceID;

		private static string lastText;

		private static int lastInstanceID;

		private static bool wasFocused;

		private static void WatchConsoleActivity()
		{
			if (entryChanged == null)
			{
				return;
			}

			try
			{
				var consoleWindow = (EditorWindow)Resources.FindObjectsOfTypeAll(ConsoleWindowType).FirstOrDefault();

				string activeText;
				int activeInstanceID;
				bool focused;

				if (consoleWindow != null)
				{
					activeText = (string)ConsoleWindow_m_ActiveText.GetValueOptimized(consoleWindow);
					activeInstanceID = (int)ConsoleWindow_m_ActiveInstanceID.GetValueOptimized(consoleWindow);
					focused = consoleWindow.IsFocused();
				}
				else
				{
					activeText = null;
					activeInstanceID = 0;
					focused = false;
				}

				try
				{
					if (activeText != lastText || activeInstanceID != lastInstanceID || (focused && !wasFocused))
					{
						entryChanged?.Invoke(activeText, activeInstanceID);
					}
				}
				finally
				{
					lastText = activeText;
					lastInstanceID = activeInstanceID;
					wasFocused = focused;
				}
			}
			catch
			{
				if (LudiqCore.Configuration.developerMode)
				{
					throw;
				}
			}
		}
	}
}