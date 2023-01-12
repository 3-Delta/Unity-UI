using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;
	
	using static GameObjectOperations;

	// Unity doesn't expose a way to open the GO context menu, so we're reimplementing most of it
	public static class GameObjectContextMenu
	{
		public static void Open(GameObject[] targets, Rect activatorPosition, GenericMenu.MenuFunction rename = null)
		{
			var menu = new GenericMenu();
			menu.allowDuplicateNames = true;
			Fill(targets, menu, activatorPosition, rename);
			menu.DropDown(GUIUtility.ScreenToGUIRect(activatorPosition));
		}

		public static void Fill(GameObject[] targets, GenericMenu menu, Rect activatorPosition, GenericMenu.MenuFunction rename = null)
		{
			if (!Validate(targets))
			{
				return;
			}

			menu.AddItem(new GUIContent("Copy %c"), false, () => Copy(targets));
			menu.AddItem(new GUIContent("Paste %v"), false, () => Paste(targets));
			menu.AddSeparator(string.Empty);

			if (rename != null)
			{
				menu.AddItem(new GUIContent("Rename _F2"), false, rename);
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Rename _F2"), false);
			}

			menu.AddItem(new GUIContent("Duplicate %d"), false, () => Duplicate(targets));

			var deleteLabel = "Delete";

			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				deleteLabel += " _Del";
			}

			menu.AddItem(new GUIContent(deleteLabel), false, () => Delete(targets));

			menu.AddSeparator(string.Empty);

			// Asset
			var addedAssetItem = false;

			if (targets.Length == 1)
			{
				var target = targets[0];

				var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);

				if (!string.IsNullOrEmpty(assetPath))
				{
					if (PrefabUtility.IsPartOfModelPrefab(target))
					{
						menu.AddItem(new GUIContent("Open Model"), false, () => OpenModel(target));
					}
					else
					{
						if (CanOpenPrefab(target))
						{
							menu.AddItem(new GUIContent("Open Prefab Asset"), false, () => OpenPrefab(target));
						}
						else
						{
							menu.AddDisabledItem(new GUIContent("Open Prefab Asset"), false);
						}
					}

					menu.AddItem(EditorGUIUtility.TrTextContent("Select Prefab Asset"), false, () => SelectPrefabAsset(target));
					addedAssetItem = true;
				}
			}

			if (AnyOutermostPrefabRoots(targets))
			{
				menu.AddItem(EditorGUIUtility.TrTextContent("Unpack Prefab"), false, () => UnpackPrefab(targets));
				menu.AddItem(EditorGUIUtility.TrTextContent("Unpack Prefab Completely"), false, () => UnpackPrefabCompletely(targets));
				addedAssetItem = true;
			}

			// Additional Peek prefab operations
			if (targets.Length == 1)
			{
				var target = targets[0];
				
				if (PrefabUtility.IsPartOfPrefabInstance(target))
				{
					menu.AddItem(new GUIContent("Create Prefab Variant..."), false, () => CreatePrefabVariant(target));
					menu.AddItem(new GUIContent("Create Original Prefab..."), false, () => CreateOriginalPrefab(target));
				}
				else
				{
					menu.AddItem(new GUIContent("Create Prefab..."), false, () => CreatePrefab(target));
				}

				addedAssetItem = true;
			}

			if (addedAssetItem)
			{
				menu.AddSeparator(string.Empty);
			}

			// Peek

			activatorPosition.width = 300;

			var hierarchyLabel = "Hierarchy...";

			menu.AddItem(new GUIContent(hierarchyLabel), false, () => ShowHierarchy(targets, activatorPosition));
			menu.AddItem(new GUIContent("Replace..."), false, () => Replace(targets, activatorPosition));

			if (targets.Length == 1)
			{
				var target = targets[0];
				menu.AddItem(new GUIContent("Create Parent..."), false, () => CreateParent(targets, activatorPosition));
				menu.AddItem(new GUIContent("Create Sibling..."), false, () => CreateSibling(target, activatorPosition));
				menu.AddItem(new GUIContent("Create Child..."), false, () => CreateChild(target, activatorPosition));
			}

			menu.AddSeparator(string.Empty);
			var transformTargets = targets.Select(go => go.transform).ToArray();
			menu.AddItem(new GUIContent("Group %g"), false, () => GroupOperations.GroupLocally(transformTargets));
			menu.AddItem(new GUIContent("Group Globally %&g"), false, () => GroupOperations.GroupGlobally(transformTargets));
			menu.AddItem(new GUIContent("Ungroup %#g"), false, () => GroupOperations.Ungroup(transformTargets));
		}
	}
}
