using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class PathUtility
	{
		public static string TryPathsForFile(string fileName, IEnumerable<string> directories)
		{
			return directories.Select(directory => Path.Combine(directory, fileName)).FirstOrDefault(File.Exists);
		}

		public static string TryPathsForFile(string fileName, params string[] directories)
		{
			return TryPathsForFile(fileName, (IEnumerable<string>)directories);
		}

		public static string GetRelativePath(string path, string directory)
		{
			Ensure.That(nameof(path)).IsNotNull(path);
			Ensure.That(nameof(directory)).IsNotNull(directory);

			if (!directory.EndsWith(Path.DirectorySeparatorChar))
			{
				directory += Path.DirectorySeparatorChar;
			}

			try
			{
				// Optimization: Try a simple substring if possible

				path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				directory = directory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

				if (path.StartsWith(directory, StringComparison.Ordinal))
				{
					return path.Substring(directory.Length);
				}

				// Otherwise, use the URI library

				var pathUri = new Uri(path);
				var folderUri = new Uri(directory);

				return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString()
					.Replace('/', Path.DirectorySeparatorChar));
			}
			catch (UriFormatException ufex)
			{
				throw new UriFormatException($"Failed to get relative path.\nPath: {path}\nDirectory:{directory}\n{ufex}");
			}
		}

		public static string FromEditorResources(string path)
		{
			return GetRelativePath(path, Paths.editorDefaultResources);
		}

		public static string FromAssets(string path)
		{
			return GetRelativePath(path, Paths.assets);
		}

		public static string FromProject(string path)
		{
			return GetRelativePath(path, Paths.project);
		}

		public static void CreateParentDirectoryIfNeeded(string path)
		{
			CreateDirectoryIfNeeded(Directory.GetParent(path).FullName);
		}

		public static void CreateDirectoryIfNeeded(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static void DeleteDirectoryIfExists(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			
			var metaFilePath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + ".meta");

			if (File.Exists(metaFilePath))
			{
				File.Delete(metaFilePath);
			}
		}

		public static string MakeSafeFilename(string filename, char replace)
		{
			foreach (var c in Path.GetInvalidFileNameChars())
			{
				filename = filename.Replace(c, replace);
			}

			return filename;
		}

		public static string GetRootPath(string rootFileName, string defaultRootFolderPath, bool autoCreate)
		{
			// Quick & dirty optimization: looking in all directories is expensive,
			// so if the user left the plugin in the default directory that we ship
			// (directly under Plugins), we'll use this path directly.

			string rootFilePath;

			var defaultRootFilePath = Path.Combine(defaultRootFolderPath, rootFileName);

			if (File.Exists(defaultRootFilePath))
			{
				rootFilePath = defaultRootFilePath;
			}
			else
			{
				var rootFiles = Directory.GetFiles(Paths.assets, rootFileName, SearchOption.AllDirectories);

				if (rootFiles.Length > 1)
				{
					throw new IOException($"More than one root files found ('{rootFileName}'). Cannot determine root path.\n{rootFiles.ToLineSeparatedString()}");
				}
				else if (rootFiles.Length == 0)
				{
					if (autoCreate)
					{
						try
						{
							CreateParentDirectoryIfNeeded(defaultRootFilePath);
							File.WriteAllBytes(defaultRootFilePath, Empty<byte>.array);
							rootFilePath = defaultRootFilePath;
						}
						catch (Exception ex)
						{
							throw new FileNotFoundException($"No root file found ('{rootFileName}') and could not create it:\n" + ex);
						}
					}
					else
					{
						throw new FileNotFoundException($"No root file found ('{rootFileName}'). Cannot determine root path.");
					}
				}
				else // if (rootFiles.Length == 1)
				{
					rootFilePath = rootFiles[0];
				}
			}

			return Directory.GetParent(rootFilePath).FullName;
		}

		public static string NaiveNormalize(string path)
		{
			if (path == null)
			{
				return null;
			}

			return path
				.Replace(Path.DirectorySeparatorChar, NaiveSeparatorChar)
				.Replace(Path.AltDirectorySeparatorChar, NaiveSeparatorChar)
				.TrimEnd(NaiveSeparatorChar);
		}

		public static bool NaiveCompare(string a, string b)
		{
			return a == b || NaiveNormalize(a).ToUpperInvariant() == NaiveNormalize(b).ToUpperInvariant();
		}

		public static string NaiveParent(string path)
		{
			Ensure.That(nameof(path)).IsNotNull(path);

			path = NaiveNormalize(path);

			if (!path.Contains(NaiveSeparatorChar))
			{
				return string.Empty;
			}

			return path.PartBeforeLast(NaiveSeparatorChar);
		}

		public static bool NaiveContains(string parentPath, string childPath, bool recursive = false)
		{
			Ensure.That(nameof(parentPath)).IsNotNull(parentPath);
			Ensure.That(nameof(childPath)).IsNotNull(childPath);

			parentPath = NaiveNormalize(parentPath);
			childPath = NaiveNormalize(childPath);
			var childParentPath = NaiveParent(childPath);

			if (parentPath == childParentPath)
			{
				return true;
			}

			if (recursive && childParentPath.StartsWith(parentPath + NaiveSeparatorChar))
			{
				return true;
			}
			
			return false;
		}

		public const char NaiveSeparatorChar = '/';

		public static bool IsInFirstPassFolder(string path)
		{
			path = path.Replace('\\', '/');

			return path.Contains("/Plugins/") || path.Contains("/Standard Assets/") || path.Contains("/Pro Standard Assets/");
		}
	}
}