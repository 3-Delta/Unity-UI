using System;
using System.IO;

namespace Ludiq.PeekCore
{
	public static class DebugUtility
	{
		public static string logPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Ludiq.log");

		public static void LogToFile(string message)
		{
			File.AppendAllText(logPath, message + Environment.NewLine);
		}
	}
}