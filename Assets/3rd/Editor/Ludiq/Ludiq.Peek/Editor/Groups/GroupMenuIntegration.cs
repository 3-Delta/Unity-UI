using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class GroupMenuIntegration
	{
		[MenuItem("Edit/Group Selection %g", true)]
		private static bool ValidateGroupSelection(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true, false);
		}

		[MenuItem("Edit/Group Selection Globally %&g", true)]
		private static bool ValidateGroupSelectionGlobally(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true, false);
		}

		[MenuItem("Edit/Ungroup Selection %#g", true)]
		private static bool ValidateUngroupSelection(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true, false);
		}

		[MenuItem("Edit/Group Selection %g", false, 200)]
		private static void GroupSelection(MenuCommand menuCommand)
		{
			var targets = GetTargets(menuCommand);
			GroupOperations.GroupLocally(targets);
		}

		[MenuItem("Edit/Group Selection Globally %&g", false, 201)]
		private static void GroupSelectionGlobally(MenuCommand menuCommand)
		{
			var targets = GetTargets(menuCommand);
			GroupOperations.GroupGlobally(targets);
		}

		[MenuItem("Edit/Ungroup Selection %#g", false, 202)]
		private static void UngroupSelection(MenuCommand menuCommand)
		{
			var targets = GetTargets(menuCommand);
			GroupOperations.Ungroup(targets);
		}

		private static bool ValidateTarget(MenuCommand menuCommand, bool allowMultiple, bool requireChildren)
		{
			if (Selection.activeTransform != null)
			{
				if (Selection.transforms.Length > 1)
				{
					return allowMultiple;
				}

				return !requireChildren || Selection.activeTransform.childCount > 0;
			}

			return false;
		}

		private static Transform[] GetTargets(MenuCommand menuCommand)
		{
			return Selection.transforms;
		}
	}
}
