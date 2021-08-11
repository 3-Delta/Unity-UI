using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class GroupUtility
	{
		public static Type InferTransformType(Transform[] targets)
		{
			var has2D = false;
			var has3D = false;

			foreach (var target in targets)
			{
				if (target.gameObject.GetComponent<RectTransform>() != null)
				{
					has2D = true;
				}
				else
				{
					has3D = true;
				}

				if (has2D && has3D)
				{
					break;
				}
			}

			if (has3D && !has2D)
			{
				return typeof(Transform);
			}
			else if (has2D && !has3D)
			{
				return typeof(RectTransform);
			}
			else
			{
				return null;
			}
		}

		public static Type InferTransformTypeOrFail(Transform[] targets)
		{
			Ensure.That(nameof(targets)).IsNotNull(targets);

			var transformType = InferTransformType(targets);

			if (transformType == null)
			{
				throw new InvalidOperationException("Cannot create a group containing both Transforms and RectTransforms.");
			}

			return transformType;
		}

		private static GameObject CreateGroup(string name, Type transformType)
		{
			if (transformType == typeof(RectTransform))
			{
				return new GameObject(name, transformType);
			}
			else
			{
				return new GameObject(name);
			}
		}
		
		public static Transform GroupLocally(Transform[] targets, string name = "Group")
		{
			var transformType = InferTransformTypeOrFail(targets);
			var scene = TransformOperations.FindCommonScene(targets);
			var haveCommonScene = scene != null;
			scene = scene ?? EditorSceneManager.GetActiveScene();
			
			var firstSiblingIndex = targets.Select(t => t.GetSiblingIndex()).Min();

			var group = CreateGroup(name, transformType);
			Undo.RegisterCreatedObjectUndo(group, "Group");
			Undo.MoveGameObjectToScene(group.gameObject, scene.Value, "Group");

			if (haveCommonScene)
			{
				var shallowestTarget = TransformOperations.FindShallowest(targets);
				Undo.SetTransformParent(group.transform, shallowestTarget.transform.parent, "Group");
			}

			foreach (var target in targets.OrderBy(t => t.GetSiblingIndex()))
			{
				Undo.SetTransformParent(target.transform, null, "Group");
				Undo.MoveGameObjectToScene(target.gameObject, scene.Value, "Group");
				Undo.SetTransformParent(target.transform, group.transform, "Group");
			}
			
			Undo.RecordObject(group.transform, "Group");

			if (haveCommonScene)
			{
				group.transform.SetSiblingIndex(firstSiblingIndex);
			}

			if (transformType == typeof(RectTransform))
			{
				TransformOperations.CenterOnPivots(group.transform);
			}
			else
			{
				TransformOperations.CenterOnBounds(group.transform);
			}

			return group.transform;
		}

		public static Transform GroupGlobally(Transform[] targets, string name = "Group")
		{
			var transformType = InferTransformTypeOrFail(targets);
			var scene = TransformOperations.FindCommonScene(targets) ?? EditorSceneManager.GetActiveScene();

			var group = CreateGroup(name, transformType);
			Undo.RegisterCreatedObjectUndo(group, "Group");
			Undo.MoveGameObjectToScene(group.gameObject, scene, "Group");

			foreach (var target in targets.OrderBy(t => t.GetSiblingIndex()))
			{
				Undo.SetTransformParent(target.transform, null, "Group");
				Undo.MoveGameObjectToScene(target.gameObject, scene, "Group");
				Undo.SetTransformParent(target.transform, group.transform, "Group");
			}
			
			return group.transform;
		}

		public static Transform[] Ungroup(Transform[] targets)
		{
			var ungrouped = new List<Transform>();

			foreach (var target in targets)
			{
				foreach (var _ungrouped in Ungroup(target))
				{
					ungrouped.Add(_ungrouped);
				}
			}

			return ungrouped.ToArray();
		}

		public static Transform[] Ungroup(Transform target)
		{
			if (!IsGroup(target))
			{
				return new[] {target};
			}

			for (var i = 0; i < target.childCount; i++)
			{
				if (!TransformOperations.WarnRestructurable(target.GetChild(i)))
				{
					return new[] {target};
				}
			}

			var ungrouped = new Transform[target.childCount];

			var siblingIndex = target.GetSiblingIndex();

			var j = 0;

			while (target.childCount >  0)
			{
				var child = target.GetChild(0);
				Undo.SetTransformParent(child, target.parent, "Ungroup");
				Undo.RecordObject(child, "Ungroup");
				child.SetSiblingIndex(siblingIndex);
				ungrouped[j] = child;
				j++;
			}

			Undo.DestroyObjectImmediate(target.gameObject);

			return ungrouped;
		}

		public static bool IsGroup(Transform target)
		{
			// TODO: Tag? Better check?
			return target.childCount > 0 && target.gameObject.GetComponents<Component>().Length == 1;
		}
	}
}