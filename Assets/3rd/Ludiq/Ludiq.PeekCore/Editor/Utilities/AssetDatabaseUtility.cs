using System;
using System.Collections.Generic;
using Ludiq.PeekCore.ReflectionMagic;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class AssetDatabaseUtility
	{
		public static IEnumerable<HierarchyPropertyCache> FindAssets(string query)
		{
			// AssetDatabase.FindAssets returns GUIDs, which is not specific enough to identify sub-assets.
			// Internally, it uses another method that returns a IEnumerable<HierarchyProperty>, then selects the GUID from that.
			// But HierarchyProperty contains a lot more info, like instanceID, which is guaranteed to point to a single
			// UnityEngine.Object, even for sub assets. We'll use that instead to target Sprites.

			var searchFilter = ((Type)UnityEditorDynamic.SearchFilter).Instantiate().AsDynamic();
			UnityEditorDynamic.SearchUtility.ParseSearchString(query, searchFilter);
			var hierarchyProperties = UnityEditorDynamic.AssetDatabase.FindAllAssets(searchFilter);

			foreach (var hierarchyProperty in hierarchyProperties)
			{
				yield return new HierarchyPropertyCache((HierarchyProperty)hierarchyProperty);
			}
		}
	}
}