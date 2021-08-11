using System;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public static class WindowLayoutUtility
	{
		private static readonly MethodInfo WindowLayout_LoadWindowLayout;

		private static readonly MethodInfo WindowLayout_SaveWindowLayout;

		private static readonly PropertyInfo Toolbar_lastLoadedLayoutName; // internal static string lastLoadedLayoutName { get; set; }

		static WindowLayoutUtility()
		{
			try
			{
				var ToolbarType = Type.GetType("UnityEditor.Toolbar,UnityEditor", true);
				var WindowLayoutType = Type.GetType("UnityEditor.WindowLayout,UnityEditor", true);
				
				WindowLayout_LoadWindowLayout = WindowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), typeof(bool) }, null);
				WindowLayout_SaveWindowLayout = WindowLayoutType.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
				Toolbar_lastLoadedLayoutName = ToolbarType.GetProperty("lastLoadedLayoutName", BindingFlags.Static | BindingFlags.NonPublic);

				if (WindowLayout_LoadWindowLayout == null)
				{
					throw new MissingMemberException(WindowLayoutType.FullName, "LoadWindowLayout");
				}

				if (WindowLayout_SaveWindowLayout == null)
				{
					throw new MissingMemberException(WindowLayoutType.FullName, "SaveWindowLayout");
				}

				if (Toolbar_lastLoadedLayoutName == null)
				{
					throw new MissingMemberException(ToolbarType.FullName, "lastLoadedLayoutName");
				}
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}
		
		public static void LoadLayout(string path)
		{
			try
			{
				WindowLayout_LoadWindowLayout.Invoke(null, new object[] { path, true });
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}
		
		public static void SaveLayout(string path)
		{
			try
			{
				PathUtility.CreateParentDirectoryIfNeeded(path);
				WindowLayout_SaveWindowLayout.Invoke(null, new object[] { path });
			}
			catch (Exception ex)
			{
				throw new UnityEditorInternalException(ex);
			}
		}

		public static string lastLoadedLayoutName
		{
			get
			{
				try
				{
					return (string)Toolbar_lastLoadedLayoutName.GetValue(null);
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
			}
			set
			{
				try
				{
					Toolbar_lastLoadedLayoutName.SetValue(null, value);
				}
				catch (Exception ex)
				{
					throw new UnityEditorInternalException(ex);
				}
			}
		}
	}
}
