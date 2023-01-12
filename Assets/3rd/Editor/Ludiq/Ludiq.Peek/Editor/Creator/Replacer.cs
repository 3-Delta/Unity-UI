using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class Replacer
	{
		public static FuzzyWindow Open(GameObject[] targets, Rect activatorPosition)
		{
			if (!TransformOperations.WarnRestructurable(targets.Select(go => go.transform).ToArray()))
			{
				return null;
			}

			// GameObject menu creators change the selection, so we need to cache it
			var selectionSnapshot = Selection.objects;

			LudiqGUI.FuzzyDropdown
			(
				activatorPosition,
				new CreateGameObjectOptionTree("Replace with..."),
				null,
				(_instance) =>
				{
					var template = (GameObject)_instance;

					var allSelected = new HashSet<GameObject>();

					foreach (var target in targets)
					{
						var selected = selectionSnapshot.Contains(target);
						var position = target.transform.position;
						var rotation = target.transform.rotation;
						var scale = target.transform.localScale;
						var parent = target.transform.parent;
						var siblingIndex = target.transform.GetSiblingIndex();
						var scene = target.scene;

						Undo.DestroyObjectImmediate(target);
						var replacement = DuplicateGameObject(template);
						Undo.MoveGameObjectToScene(replacement, scene, "Move Replacement To Scene");

						replacement.transform.position = position;
						replacement.transform.rotation = rotation;

						if (PeekPlugin.Configuration.preserveScaleOnReplace)
						{
							replacement.transform.localScale = scale;
						}

						replacement.transform.SetParent(parent, true);
						replacement.transform.SetSiblingIndex(siblingIndex);

						if (selected)
						{
							allSelected.Add(replacement);
						}
					}

					Selection.objects = allSelected.ToArray();

					UnityObject.DestroyImmediate(template);
				}
			);

			return FuzzyWindow.instance;
		}

		private static GameObject DuplicateGameObject(GameObject original)
		{
			UnityObject prefabRoot = PrefabUtility.GetCorrespondingObjectFromSource(original);

			GameObject result;

			if (prefabRoot != null)
			{
				result = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot, original.scene);
			}
			else
			{
				result = (GameObject)UnityObject.Instantiate(original);
				result.name = original.name;
				Undo.MoveGameObjectToScene(result, original.scene, "Duplicate " + result.name);
			}

			Undo.RegisterCreatedObjectUndo(result, "Duplicate " + result.name);

			return result;
		}
	}
}