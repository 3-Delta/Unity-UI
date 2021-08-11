using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class TransformOperations
	{
		public static void Bake(Transform target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);
			
			CacheChildrenTransforms(target, out var globalPositions, out var globalRotations, out var globalScales);

			Undo.RecordObject(target, $"Bake {target.name}");
			target.localPosition = Vector3.zero;
			target.localRotation = Quaternion.identity;
			target.localScale = Vector3.one;
			
			ApplyChildrenTransforms(target, globalPositions, globalRotations, globalScales);
		}

		public static void CenterOnPivots(Transform target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);

			CacheChildrenTransforms(target, out var globalPositions, out var globalRotations, out var globalScales);
			
			Undo.RecordObject(target, $"Center {target.name}");

			var center = Vector3.zero;

			for (var i = 0; i < target.childCount; i++)
			{
				center += target.GetChild(i).position;
			}

			center /= target.childCount;
			target.position = center;

			ApplyChildrenTransforms(target, globalPositions, globalRotations, globalScales);
		}

		public static void CenterOnBounds(Transform target)
		{
			Ensure.That(nameof(target)).IsNotNull(target);

			var rectTransform = target.GetComponent<RectTransform>();

			if (rectTransform != null)
			{
				Debug.LogWarning("RectTransforms cannot be reliably centered on bounds.\nTry centering on pivots instead.");
				return;
			}

			CacheChildrenTransforms(target, out var globalPositions, out var globalRotations, out var globalScales);

			target.gameObject.CalculateBounds(out var bounds, space: Space.World);
			Undo.RecordObject(target, $"Center {target.name}");
			target.position = bounds.center;

			ApplyChildrenTransforms(target, globalPositions, globalRotations, globalScales);
		}
		
		public static bool IsRestructurable(Transform target)
		{
			var go = target.gameObject;

			if (PrefabUtility.IsPartOfPrefabInstance(go) && !PrefabUtility.IsOutermostPrefabInstanceRoot(go))
			{
				return false;
			}

			return true;
		}

		public static bool IsRestructurable(Transform[] targets)
		{
			Ensure.That(nameof(targets)).IsNotNull(targets);

			foreach (var target in targets)
			{
				if (!IsRestructurable(target))
				{
					return false;
				}
			}

			return true;
		}
		
		public static bool WarnRestructurable(Transform target)
		{
			if (!IsRestructurable(target))
			{
				EditorUtility.DisplayDialog("Cannot restructure Prefab instance", "Children of a Prefab instance cannot be deleted or moved, and components cannot be reordered.\n\nYou can open the Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its prefab connection.", "OK");
				return false;
			}

			return true;
		}

		public static bool WarnRestructurable(Transform[] targets)
		{
			if (!IsRestructurable(targets))
			{
				EditorUtility.DisplayDialog("Cannot restructure Prefab instance", "Children of a Prefab instance cannot be deleted or moved, and components cannot be reordered.\n\nYou can open the Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its prefab connection.", "OK");
				return false;
			}

			return true;
		}

		public static Transform FindShallowest(Transform[] targets)
		{
			var shallowest = int.MaxValue;
			Transform shallowestTarget = null;

			foreach (var target in targets)
			{
				var depth = 0;
				var transform = target;

				while (transform != null)
				{
					depth++;
					transform = transform.parent;
				}

				if (depth < shallowest)
				{
					shallowest = depth;
					shallowestTarget = target;
				}
			}

			return shallowestTarget;
		}

		public static Scene? FindCommonScene(Transform[] targets)
		{
			Scene? scene = null;

			foreach (var target in targets)
			{
				if (scene == null)
				{
					scene = target.gameObject.scene;
				}
				else if (scene != target.gameObject.scene)
				{
					return null;
				}
			}

			return scene;
		}

		private static void CacheChildrenTransforms(Transform target, out Vector3[] globalPositions, out Quaternion[] globalRotations, out Vector3[] globalScales)
		{
			globalPositions = new Vector3[target.childCount];
			globalRotations = new Quaternion[target.childCount];
			globalScales = new Vector3[target.childCount];

			for (var i = 0; i < target.childCount; i++)
			{
				var child = target.GetChild(i);

				globalPositions[i] = target.TransformPoint(child.localPosition);
				globalRotations[i] = target.TransformQuaternion(child.localRotation);
				globalScales[i] = child.lossyScale;
			}
		}

		private static void ApplyChildrenTransforms(Transform target, Vector3[] globalPositions, Quaternion[] globalRotations, Vector3[] globalScales)
		{
			for (var i = 0; i < target.childCount; i++)
			{
				var child = target.GetChild(i);

				Undo.RecordObject(child, $"Realign {target.name}");
				child.position = globalPositions[i];
				child.rotation = globalRotations[i];
				child.SetGlobalScale(globalScales[i]);
			}
		}

		private static Quaternion TransformQuaternion(this Transform transform, Quaternion localRotation)
		{
			return transform.rotation * localRotation;
		}

		// https://forum.unity.com/threads/solved-why-is-transform-lossyscale-readonly.363594/#post-2356866
		private static void SetGlobalScale(this Transform transform, Vector3 globalScale)
		{
			transform.localScale = Vector3.one;
			var matrix = transform.worldToLocalMatrix;
			matrix.SetColumn(0, new Vector4(matrix.GetColumn(0).magnitude, 0, 0, 0));
			matrix.SetColumn(1, new Vector4(0, matrix.GetColumn(1).magnitude, 0, 0));
			matrix.SetColumn(2, new Vector4(0, 0, matrix.GetColumn(2).magnitude, 0));
			matrix.SetColumn(3, new Vector4(0, 0, 0, 1));
			transform.localScale = matrix.MultiplyPoint(globalScale);
		}
	}
}