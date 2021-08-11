using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: RegisterInspector(typeof(KeyboardShortcut), typeof(KeyboardShortcutInspector))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class KeyboardShortcutInspector : Inspector
	{
		public KeyboardShortcutInspector(Accessor accessor) : base(accessor) { }

		private Accessor enabledAccessor => accessor[nameof(KeyboardShortcut.enabled)];

		private Accessor keyCodeAccessor => accessor[nameof(KeyboardShortcut.keyCode)];

		private Accessor actionAccessor => accessor[nameof(KeyboardShortcut.action)];

		private Accessor modifiersAccessor => accessor[nameof(KeyboardShortcut.modifiers)];

		private Inspector enabledInspector => ChildInspector(enabledAccessor);

		private Inspector keyCodeInspector => ChildInspector(keyCodeAccessor);

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

			var keyCodePosition = new Rect
			(
				modifiersPosition.xMax + spacing,
				position.y,
				elementWidth,
				EditorGUIUtility.singleLineHeight
			);

			var actionPosition = new Rect
			(
				keyCodePosition.xMax + spacing,
				position.y,
				elementWidth,
				EditorGUIUtility.singleLineHeight
			);

			enabledInspector.DrawControl(enabledPosition);

			EditorGUI.BeginDisabledGroup(!(bool)enabledAccessor.value);

			modifiersInspector.DrawControl(modifiersPosition);
			keyCodeInspector.DrawControl(keyCodePosition);
			actionInspector.DrawControl(actionPosition);

			EditorGUI.EndDisabledGroup();
		}
	}
}