using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: RegisterInspector(typeof(MouseShortcut), typeof(MouseShortcutInspector))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class MouseShortcutInspector : Inspector
	{
		public MouseShortcutInspector(Accessor accessor) : base(accessor) { }

		private Accessor enabledAccessor => accessor[nameof(MouseShortcut.enabled)];

		private Accessor buttonAccessor => accessor[nameof(MouseShortcut.button)];

		private Accessor actionAccessor => accessor[nameof(MouseShortcut.action)];

		private Accessor modifiersAccessor => accessor[nameof(MouseShortcut.modifiers)];

		private Inspector enabledInspector => ChildInspector(enabledAccessor);

		private Inspector buttonInspector => ChildInspector(buttonAccessor);

		private Inspector actionInspector => ChildInspector(actionAccessor);

		private Inspector modifiersInspector => ChildInspector(modifiersAccessor);

		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		protected override void OnControlGUI(Rect position)
		{
			var spacing = 2;

			var enabledPosition = new Rect
			(
				position.x,
				position.y,
				16,
				EditorGUIUtility.singleLineHeight
			);

			var elementWidth = (position.width - 16 - spacing * 3) / 3;

			var modifiersPosition = new Rect
			(
				enabledPosition.xMax + spacing,
				position.y,
				elementWidth,
				EditorGUIUtility.singleLineHeight
			);

			var buttonPosition = new Rect
			(
				modifiersPosition.xMax + spacing,
				position.y,
				elementWidth,
				EditorGUIUtility.singleLineHeight
			);

			var actionPosition = new Rect
			(
				buttonPosition.xMax + spacing,
				position.y,
				elementWidth,
				EditorGUIUtility.singleLineHeight
			);

			enabledInspector.DrawControl(enabledPosition);

			EditorGUI.BeginDisabledGroup(!(bool)enabledAccessor.value);

			modifiersInspector.DrawControl(modifiersPosition);
			buttonInspector.DrawControl(buttonPosition);
			actionInspector.DrawControl(actionPosition);

			EditorGUI.EndDisabledGroup();
		}
	}
}