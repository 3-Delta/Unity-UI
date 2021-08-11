using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class AssetUtility
	{
		public static IEnumerable<T> FindAllAssetsOfType<T>(bool includeHidden = false)
		{
			return Resources.FindObjectsOfTypeAll<UnityObject>().Where(uo => includeHidden || !uo.IsHidden()).OfType<T>().Where(o => AssetDatabase.Contains((UnityObject)(object)o));
		}

		public static IEnumerable<UnityObject> FindAllAssetsOfType(Type type, bool includeHidden = false)
		{
			return Resources.FindObjectsOfTypeAll(type).Where(uo => includeHidden || !uo.IsHidden()).Where(AssetDatabase.Contains);
		}

		public static IEnumerable<T> LoadAllAssetsOfType<T>()
		{
			if (typeof(UnityObject).IsAssignableFrom(typeof(T)))
			{
				return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
				                    .Select(AssetDatabase.GUIDToAssetPath)
				                    .Select(AssetDatabase.LoadMainAssetAtPath)
				                    .OfType<T>();
			}
			else
			{
				// GetAllAssetPaths is undocumented and sometimes returns
				// paths that are outside the assets folder, hence the where filter.
				var result = AssetDatabase.GetAllAssetPaths()
				                          .Where(p => p.StartsWith("Assets"))
				                          .Select(AssetDatabase.LoadMainAssetAtPath)
				                          .OfType<T>();

				EditorUtility.UnloadUnusedAssetsImmediate();
				return result;
			}
		}

		public static string GetSelectedFolderPath()
		{
			foreach (UnityObject uo in Selection.GetFiltered(typeof(UnityObject), SelectionMode.Assets))
			{
				var assetPath = AssetDatabase.GetAssetPath(uo);

				if (AssetDatabase.IsValidFolder(assetPath))
				{
					return Path.Combine(Paths.project, assetPath);
				}
			}

			return null;
		}

		public static bool TryLoad<T>(string path, out T asset) where T : ScriptableObject
		{
			var assetDatabasePath = PathUtility.FromProject(path);
			
			if (File.Exists(path))
			{
				// Try loading the existing asset file.
				asset = AssetDatabase.LoadAssetAtPath<T>(assetDatabasePath);

				if (asset == null)
				{
					// The file exists, but it isn't a valid asset.
					// Warn and leave the asset as is to prevent losing its serialized contents
					// because we might be able to salvage them by deserializing later on.
					// Return a new empty instance in the mean time.
					Debug.LogWarning($"Loading {typeof(T).FullName} failed:\n{assetDatabasePath}");
					asset = ScriptableObject.CreateInstance<T>();
					return false;
				}
			}
			else
			{
				// The file doesn't exist, so create a new asset and save it.
				asset = ScriptableObject.CreateInstance<T>();
				PathUtility.CreateParentDirectoryIfNeeded(path);
				AssetDatabase.CreateAsset(asset, assetDatabasePath);
				AssetDatabase.SaveAssets();
			}
			
			return true;
		}
	}
}
