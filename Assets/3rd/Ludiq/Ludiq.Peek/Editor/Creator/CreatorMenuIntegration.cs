using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public static class CreatorMenuIntegration
	{
		// HACK: For multiple targets, we receive the command once per object instead of altogether,
		// however we use the selection as targets to regroup them, which ends up in us enqueing
		// the same group command as many times as there are selected objects.
		private static bool enqueuedMultiTargetsCommand;

		// Note: MenuItem names cannot have "..." in the string, otherwise MenuCommand.context returns null.
		
		[MenuItem("GameObject/Create Prefab", true)]
		private static bool ValidateCreatePrefab(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, false);
		}

		[MenuItem("GameObject/Create Parent", true)]
		private static bool ValidateCreateParent(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true);
		}

		[MenuItem("GameObject/Create Sibling", true)]
		private static bool ValidateCreateSibling(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, false);
		}

		[MenuItem("GameObject/Create Child", true)]
		private static bool ValidateCreateChild(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, false);
		}

		[MenuItem("GameObject/Replace &r", true)]
		private static bool ValidateReplace(MenuCommand menuCommand)
		{
			return ValidateTarget(menuCommand, true);
		}
		
		[MenuItem("GameObject/Create Prefab", false, -10090)]
		private static void CreatePrefab(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { GameObjectOperations.CreatePrefab(target); });
		}

		[MenuItem("GameObject/Create Parent", false, -9999)]
		private static void CreateParent(MenuCommand menuCommand)
		{
			var targets = GetTargets(menuCommand);

			if (enqueuedMultiTargetsCommand)
			{
				return;
			}

			enqueuedMultiTargetsCommand = true;
			
			GuiCallback.Enqueue(() => 
			{ 
				GameObjectOperations.CreateParent(targets, activatorPosition);
				enqueuedMultiTargetsCommand = false;
			});
		}

		[MenuItem("GameObject/Create Sibling", false, -9998)]
		private static void CreateSibling(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { GameObjectOperations.CreateSibling(target, activatorPosition); });
		}

		[MenuItem("GameObject/Create Child", false, -9997)]
		private static void CreateChild(MenuCommand menuCommand)
		{
			var target = GetTarget(menuCommand);
			GuiCallback.Enqueue(() => { GameObjectOperations.CreateChild(target, activatorPosition); });
		}

		[MenuItem("GameObject/Replace &r", false, -9990)]
		private static void Replace(MenuCommand menuCommand)
		{
			var targets = GetTargets(menuCommand);

			if (enqueuedMultiTargetsCommand)
			{
				return;
			}

			enqueuedMultiTargetsCommand = true;

			GuiCallback.Enqueue(() => 
			{
				GameObjectOperations.Replace(targets, activatorPosition);
				enqueuedMultiTargetsCommand = false;
			});
		}

		private static bool ValidateTarget(MenuCommand menuCommand, bool allowMultiple)
		{
			if (Selection.activeTransform != null)
			{
				return allowMultiple || Selection.transforms.Length == 1;
			}

			var target = menuCommand.context as GameObject;
			return target != null && target.IsSceneBound();
		}

		private static GameObject GetTarget(MenuCommand menuCommand)
		{
			return menuCommand.context as GameObject ?? Selection.activeTransform?.gameObject;
		}

		private static GameObject[] GetTargets(MenuCommand menuCommand)
		{
			return Selection.transforms.Select(t => t.gameObject).ToArray();
		}

		private static Rect activatorPosition => LudiqGUIUtility.GUIToScreenRect(new Rect(Event.current.mousePosition, new Vector2(300, 0)));
	}
}