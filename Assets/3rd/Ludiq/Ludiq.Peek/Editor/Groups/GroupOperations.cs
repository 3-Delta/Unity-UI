using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class GroupOperations
	{
		private static bool WarnTransformType(Transform[] targets)
		{
			if (GroupUtility.InferTransformType(targets) == null)
			{
				EditorUtility.DisplayDialog("Cannot group objects", "Cannot create a group containing both Transforms and RectTransforms.", "OK");
				return false;
			}

			return true;
		}

		public static void GroupLocally(Transform[] targets, string defaultName = "Group")
		{
			if (!TransformOperations.WarnRestructurable(targets))
			{
				return;
			}

			if (!WarnTransformType(targets))
			{
				return;
			}

			if (GroupNamePrompt.Prompt(out var name, defaultName))
			{
				var group = GroupUtility.GroupLocally(targets, name);
				Selection.activeTransform = group;
			}
		}

		public static void GroupGlobally(Transform[] targets, string defaultName = "Group")
		{
			if (!TransformOperations.WarnRestructurable(targets))
			{
				return;
			}

			if (!WarnTransformType(targets))
			{
				return;
			}

			if (GroupNamePrompt.Prompt(out var name, defaultName))
			{
				var group = GroupUtility.GroupGlobally(targets, name);
				Selection.activeTransform = group;
			}
		}

		public static void Ungroup(Transform[] targets)
		{
			if (!TransformOperations.WarnRestructurable(targets))
			{
				return;
			}

			var ungrouped = GroupUtility.Ungroup(targets);
			Selection.objects = ungrouped.Select(t => t.gameObject).ToArray();
		}

		public static void Ungroup(Transform target)
		{
			if (!TransformOperations.WarnRestructurable(target))
			{
				return;
			}

			var ungrouped = GroupUtility.Ungroup(target);
			Selection.objects = ungrouped.Select(t => t.gameObject).ToArray();
		}
	}
}
