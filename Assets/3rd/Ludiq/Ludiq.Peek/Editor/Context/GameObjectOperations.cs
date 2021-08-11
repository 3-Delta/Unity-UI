using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	// Unity doesn't expose a way to open the GO context menu, so we're reimplementing most of it
	public static class GameObjectOperations
	{
		public static bool Validate(GameObject[] targets)
		{
			Ensure.That(nameof(targets)).IsNotNull(targets);

			foreach (var target in targets)
			{
				if (!Validate(target))
				{
					return false;
				}
			}

			return true;
		}

		public static bool Validate(GameObject target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);

			if (!target.IsSceneBound())
			{
				return false;
			}

			return true;
		}

		public static void Copy(GameObject[] targets)
		{
			Validate(targets);
			var previousSelection = Selection.objects;
			Selection.objects = targets;
			Unsupported.CopyGameObjectsToPasteboard();
			Selection.objects = previousSelection;
		}

		public static void Paste(GameObject[] targets)
		{
			Validate(targets);
			var previousSelection = Selection.objects;
			Selection.objects = targets;
			Unsupported.CopyGameObjectsToPasteboard();
			Selection.objects = previousSelection;
		}

		public static void Duplicate(GameObject[] targets)
		{
			Validate(targets);
			Selection.objects = targets;
			Unsupported.DuplicateGameObjectsUsingPasteboard();
		}

		public static void Delete(GameObject[] targets)
		{
			Validate(targets);
			var previousSelection = Selection.objects;
			Selection.objects = targets;
			Unsupported.DeleteGameObjectSelection();
			Selection.objects = previousSelection;
		}

		public static void ShowHierarchy(GameObject[] targets, Rect activatorPosition)
		{
			Validate(targets);
			HierarchyPopup.Show(targets, activatorPosition);
		}

		public static bool CreatePrefab(GameObject target)
		{
			if (PrefabUtility.IsPartOfPrefabInstance(target))
			{
				switch (EditorUtility.DisplayDialogComplex("Create Prefab", "Would you like to create a new original Prefab or a variant of this Prefab?", "Original Prefab", "Cancel", "Prefab Variant"))
				{
					case 0: return CreateOriginalPrefab(target);
					case 1: return false;
					case 2: return CreatePrefabVariant(target);
				}
			}

			var prefabPath = EditorUtility.SaveFilePanelInProject("Create Prefab", target.name, "prefab", null);

			if (!string.IsNullOrEmpty(prefabPath))
			{
				PrefabUtility.SaveAsPrefabAssetAndConnect(target, prefabPath, InteractionMode.UserAction);
				return true;
			}

			return false;
		}

		public static bool CreatePrefabVariant(GameObject target)
		{
			if (!PrefabUtility.IsPartOfPrefabInstance(target))
			{
				throw new InvalidOperationException("Use CreatePrefab for objects that are not prefab instances.");
			}

			var variantPath = EditorUtility.SaveFilePanelInProject("Create Prefab Variant", target.name, "prefab", null);

			if (!string.IsNullOrEmpty(variantPath))
			{
				PrefabUtility.SaveAsPrefabAssetAndConnect(target, variantPath, InteractionMode.UserAction);
				return true;
			}

			return false;
		}

		public static bool CreateOriginalPrefab(GameObject target)
		{
			if (!PrefabUtility.IsPartOfPrefabInstance(target))
			{
				throw new InvalidOperationException("Use CreatePrefab for objects that are not prefab instances.");
			}

			// https://forum.unity.com/threads/solved-creating-prefab-variant-with-script.546358/#post-3605552
			var prefabPath = EditorUtility.SaveFilePanelInProject("Create Original Prefab", target.name, "prefab", null);

			if (!string.IsNullOrEmpty(prefabPath))
			{
				PrefabUtility.UnpackPrefabInstance(target, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
				PrefabUtility.SaveAsPrefabAssetAndConnect(target, prefabPath, InteractionMode.UserAction);
				return true;
			}

			return false;
		}

		public static void Replace(GameObject[] targets, Rect activatorPosition)
		{
			Validate(targets);
			Replacer.Open(targets, activatorPosition);
		}
		
		public static void CreateParent(GameObject[] targets, Rect activatorPosition)
		{
			Validate(targets);

			var transformTargets = targets.Select(t => t.transform).ToArray();

			if (!TransformOperations.WarnRestructurable(transformTargets))
			{
				return;
			}

			var shallowestTarget = TransformOperations.FindShallowest(transformTargets);

			CreateMenu
			(
				activatorPosition, created =>
				{
					Undo.SetTransformParent(created.transform, shallowestTarget.transform.parent, "Create Parent");

					foreach (var target in targets)
					{
						Undo.SetTransformParent(target.transform, created.transform, "Create Parent");
					}

					TransformOperations.CenterOnBounds(created.transform);

					Selection.activeGameObject = created;
					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
				}
			);
		}

		public static void CreateChild(GameObject target, Rect activatorPosition)
		{
			Validate(target);

			CreateMenu
			(
				activatorPosition, created =>
				{
					Undo.SetTransformParent(created.transform, target.transform, "Create Child");
					created.transform.localPosition = Vector3.zero;
					created.transform.localRotation = Quaternion.identity;
					Selection.activeGameObject = created;
					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
				}
			);
		}

		public static void CreateSibling(GameObject target, Rect activatorPosition)
		{
			Validate(target);

			CreateMenu
			(
				activatorPosition, created =>
				{
					Undo.SetTransformParent(created.transform, target.transform.parent, "Create Sibling");
					created.transform.position = target.transform.position;
					created.transform.rotation = target.transform.rotation;
					created.transform.SetSiblingIndex(target.transform.GetSiblingIndex());
					Selection.activeGameObject = created;
					Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
				}
			);
		}

		private static void CreateMenu(Rect activatorPosition, Action<GameObject> created)
		{
			LudiqGUI.FuzzyDropdown
			(
				activatorPosition,
				new CreateGameObjectOptionTree(),
				null,
				(_instance) => { created?.Invoke((GameObject)_instance); }
			);
		}

		public static bool AnyOutermostPrefabRoots(GameObject[] targets)
		{
			Validate(targets);

			foreach (var go in targets)
			{
				if (go != null && PrefabUtility.IsPartOfNonAssetPrefabInstance(go) && PrefabUtility.IsOutermostPrefabInstanceRoot(go))
				{
					return true;
				}
			}

			return false;
		}

		public static void UnpackPrefab(GameObject[] targets)
		{
			Validate(targets);

			foreach (var go in targets)
			{
				if (go != null && PrefabUtility.IsPartOfNonAssetPrefabInstance(go) && PrefabUtility.IsOutermostPrefabInstanceRoot(go))
				{
					PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
				}
			}
		}

		public static void UnpackPrefabCompletely(GameObject[] targets)
		{
			Validate(targets);

			foreach (var go in targets)
			{
				if (go != null && PrefabUtility.IsPartOfNonAssetPrefabInstance(go) && PrefabUtility.IsOutermostPrefabInstanceRoot(go))
				{
					PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.UserAction);
				}
			}
		}

		public static void OpenModel(GameObject target)
		{
			Validate(target);
			var asset = (GameObject)UnityEditorDynamic.PrefabUtility.GetOriginalSourceOrVariantRoot(target);
			AssetDatabase.OpenAsset(asset);
		}

		public static bool CanOpenPrefab(GameObject target)
		{
			if (!PrefabUtility.IsPartOfAnyPrefab(target))
			{
				return false;
			}

			if (!PrefabUtility.IsAnyPrefabInstanceRoot(target))
			{
				return false;
			}

			if (PrefabUtility.GetPrefabInstanceStatus(target) != PrefabInstanceStatus.Connected)
			{
				return false;
			}

			var source = (GameObject)UnityEditorDynamic.PrefabUtility.GetOriginalSourceOrVariantRoot(target);

			if (source == null || PrefabUtility.IsPartOfImmutablePrefab(source))
			{
				return false;
			}

			return true;
		}

		public static void OpenPrefab(GameObject target)
		{
			Validate(target);
			var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
			UnityEditorDynamic.PrefabStageUtility.OpenPrefab(assetPath, target);
		}

		public static void SelectPrefabAsset(GameObject target)
		{
			Validate(target);
			var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target);
			var prefabAsset = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
			Selection.activeObject = prefabAsset;
			EditorGUIUtility.PingObject(prefabAsset.GetInstanceID());
		}
	}
}