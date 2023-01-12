using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: RegisterInspector(typeof(ShortcutModifiers), typeof(ShortcutModifiersInspector))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class ShortcutModifiersInspector : Inspector
	{
		public ShortcutModifiersInspector(Accessor accessor) : base(accessor) { }
		
		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		protected override void OnControlGUI(Rect position)
		{
			var value = (ShortcutModifiers)accessor.value;
			var action = value.HasFlagFast(ShortcutModifiers.Action);
			var shift = value.HasFlagFast(ShortcutModifiers.Shift);
			var alt = value.HasFlagFast(ShortcutModifiers.Alt);

			var position1 = new Rect
			(
				position.x,
				position.y,
				position.width / 3,
				EditorGUIUtility.singleLineHeight
			);

			var position2 = new Rect
			(
				position1.xMax,
				position.y,
				position.width / 3,
				EditorGUIUtility.singleLineHeight
			);

			var position3 = new Rect
			(
				position2.xMax,
				position.y,
				position.width / 3,
				EditorGUIUtility.singleLineHeight
			);

			EditorGUI.BeginChangeCheck();

			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				alt = GUI.Toggle(position1, alt, ShortcutModifiers.Alt.ToNiceString(), EditorStyles.miniButtonMid);
				shift = GUI.Toggle(position2, shift, ShortcutModifiers.Shift.ToNiceString(), EditorStyles.miniButtonRight);
				action = GUI.Toggle(position3, action, ShortcutModifiers.Action.ToNiceString(), EditorStyles.miniButtonLeft);
			}
			else
			{
				action = GUI.Toggle(position1, action, ShortcutModifiers.Action.ToNiceString(), EditorStyles.miniButtonLeft);
				alt = GUI.Toggle(position2, alt, ShortcutModifiers.Alt.ToNiceString(), EditorStyles.miniButtonMid);
				shift = GUI.Toggle(position3, shift, ShortcutModifiers.Shift.ToNiceString(), EditorStyles.miniButtonRight);
			}

			if (EditorGUI.EndChangeCheck())
			{
				var newValue = ShortcutModifiers.None;

				if (action)
				{
					newValue |= ShortcutModifiers.Action;
				}

				if (alt)
				{
					newValue |= ShortcutModifiers.Alt;
				}

				if (shift)
				{
					newValue |= ShortcutModifiers.Shift;
				}

				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}