using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Ludiq.PeekCore
{
	public static class Paths
	{
		static Paths()
		{
			assets = Application.dataPath;
			editor = EditorApplication.applicationPath;
			editorContents = EditorApplication.applicationContentsPath;
			project = Directory.GetParent(assets).FullName;
			projectName = Path.GetFileName(project.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			projectSettings = Path.Combine(project, "ProjectSettings");
			editorDefaultResources = Path.Combine(assets, "Editor Default Resources");
		}

		public static string assets { get; }
		
		public static string editor { get; }
		
		public static string editorContents { get; }

		public static string project { get; }

		public static string projectName { get; }

		public static string projectSettings { get; }

		public static string editorDefaultResources { get; }

		#region Assembly Projects

		public static string AssemblyProject(Assembly assemblyDefinition)
		{
			string filename;

			switch (assemblyDefinition.name)
			{
				case "Assembly-CSharp": filename = projectName;
					break;

				case "Assembly-CSharp-firstpass": filename = projectName + ".Plugins";
					break;

				case "Assembly-CSharp-Editor": filename = projectName + ".Editor";
					break;

				case "Assembly-CSharp-Editor-firstpass": filename = projectName + ".Editor.Plugins";
					break;

				default: filename = assemblyDefinition.name;
					break;
			}

			var path = Path.Combine(project, filename + ".csproj");

			return path;
		}
		
		public static IEnumerable<string> assemblyProjects
		{
			get
			{
				foreach (var assemblyDefinition in CompilationPipeline.GetAssemblies())
				{
					var path = AssemblyProject(assemblyDefinition);

					if (File.Exists(path))
					{
						yield return path;
					}
				}
			}
		}

		#endregion

		#region .NET

		public const string MsBuildToolsVersion = "15.0";
		public const string MsBuildDownloadLink = "https://aka.ms/vs/15/release/vs_buildtools.exe";

		public static IEnumerable<string> environmentPaths
		{
			get
			{
				try
				{
					if (Application.platform == RuntimePlatform.WindowsEditor)
					{
						return Environment.GetEnvironmentVariable("PATH").Split(';');
					}
					else
					{
						// http://stackoverflow.com/a/41318134/154502
						var start = new ProcessStartInfo
						{
							FileName = "/bin/bash",
							Arguments = "-l -c \"echo $PATH\"", // -l = 'login shell' to execute /etc/profile
							UseShellExecute = false,
							CreateNoWindow = true,
							RedirectStandardOutput = true,
							RedirectStandardError = true
						};

						var process = Process.Start(start);
						process.WaitForExit();
						var path = process.StandardOutput.ReadToEnd().Trim();
						return path.Split(':');
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Failed to fetch environment paths: \n" + ex);
					return Enumerable.Empty<string>();
				}
			}
		}
		
		public static string msBuild
		{
			get
			{
				if (Application.platform != RuntimePlatform.WindowsEditor)
				{
					return null;
				}
				 
				var visualStudioDirectory = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7", MsBuildToolsVersion, null);

				if (visualStudioDirectory != null)
				{
					return Path.Combine(visualStudioDirectory, $@"MSBuild\{MsBuildToolsVersion}\Bin", "MSBuild.exe");
				}

				return null;
			}
		}

		public static string xBuild
		{
			get
			{
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					return null;
				}

				var path = PathUtility.TryPathsForFile("xbuild", environmentPaths);

				return path;
			}
		}

		public static string roslynCompiler => Path.Combine(Path.GetDirectoryName(editor), "Data/tools/Roslyn/csc.exe");

		public static string projectBuilder => Application.platform == RuntimePlatform.WindowsEditor ? msBuild : xBuild;

		#endregion
	}
}