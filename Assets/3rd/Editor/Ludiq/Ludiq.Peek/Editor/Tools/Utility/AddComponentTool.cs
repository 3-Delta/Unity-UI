using System;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public sealed class AddComponentTool : Tool
	{
		public GameObject[] targets { get; }

		private ToolControl activatorControl;

		public AddComponentTool(GameObject[] targets)
		{
			this.targets = targets;
			label = "Add Component";
			icon = PeekPlugin.Icons.addComponent?[IconSize.Small];
			tooltip = "Add Component";
		}

		public override bool isActive => PopupWatcher.IsOpenOrJustClosed(window);
		
		private EditorWindow window;

		public override void Open(ToolControl control)
		{
			PopupWatcher.Release(window);

			activatorControl = control;
			var activatorPosition = control.activatorGuiPosition;
			activatorPosition.width = 330;
			UnityEditorDynamic.AddComponentWindow.Show(activatorPosition, targets);
			
#if UNITY_2019_1_OR_NEWER
			window = EditorWindow.focusedWindow;
#else
			window = UnityEditorDynamic.AddComponentWindow.s_AddComponentWindow;
#endif

			PopupWatcher.Watch(window);
		}

		public override void OnMove(ToolControl control)
		{
			if (isActive && control == activatorControl && GUIUtility.hotControl == 0)
			{
				window.position = window.GetDropdownPosition(activatorControl.activatorScreenPosition, window.position.size);
			}
		}

		public override void Close(ToolControl control)
		{
			window?.Close();
		}
	}
}