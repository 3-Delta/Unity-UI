using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class TransformMenuIntegration
	{
		// Note: MenuItem names cannot have "..." in the string, otherwise MenuCommand.context returns null.
		
		[MenuItem("CONTEXT/Transform/Bake", true)]
		private static bool ValidateBake(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, false, true);
		}

		[MenuItem("CONTEXT/Transform/Center on Pivots", true)]
		private static bool ValidateCenterOnPivots(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true, true);
		}

		[MenuItem("CONTEXT/Transform/Center on Bounds", true)]
		private static bool ValidateCenterOnBounds(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true, true);
		}

		[MenuItem("CONTEXT/Transform/Bake", false, 200)]
		private static void Bake(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { TransformOperations.Bake(target); });
		}

		[MenuItem("CONTEXT/Transform/Center on Pivots", false, 201)]
		private static void CenterOnPivots(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { TransformOperations.CenterOnPivots(target); });
		}

		[MenuItem("CONTEXT/Transform/Center on Bounds", false, 202)]
		private static void CenterOnBounds(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { TransformOperations.CenterOnBounds(target); });
		}

		private static bool ValidateTarget(MenuCommand menuCommand, bool allowMultiple, bool requireChildren)
		{
			var target = menuCommand.context as Transform;

			if (target != null)
			{
				return target.IsSceneBound() && (!requireChildren || target.childCount > 0);
			}

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

		private static Transform GetTarget(MenuCommand menuCommand)
		{
			return menuCommand.context as Transform ?? Selection.activeTransform;
		}

		private static Transform[] GetTargets(MenuCommand menuCommand)
		{
			return Selection.transforms;
		}
	}
}