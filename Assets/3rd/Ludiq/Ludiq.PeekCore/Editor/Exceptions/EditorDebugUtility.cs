using System.IO;
using UnityEditor;

namespace Ludiq.PeekCore
{
	[InitializeOnLoad]
	public static class EditorDebugUtility
	{
		static EditorDebugUtility()
		{
			if (File.Exists(DebugUtility.logPath))
			{
				File.Delete(DebugUtility.logPath);
			}
		}
	}
}