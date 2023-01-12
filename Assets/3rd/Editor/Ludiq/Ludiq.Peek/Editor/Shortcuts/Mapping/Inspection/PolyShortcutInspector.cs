using Ludiq.Peek;
using Ludiq.PeekCore;
using UnityEditor;
using UnityEngine;

[assembly: RegisterInspector(typeof(PolyShortcut), typeof(PolyShortcutInspector))]

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	public class PolyShortcutInspector : Inspector
	{
		public PolyShortcutInspector(Accessor accessor) : base(accessor) { }

		private Accessor keyboardShortcutAccessor => accessor[nameof(PolyShortcut.keyboardShortcut)];

		private Accessor mouseShortcutAccessor => accessor[nameof(PolyShortcut.mouseShortcut)];
		
		private Inspector keyboardShortcutInspector => ChildInspector(keyboardShortcutAccessor);

		private Inspector mouseShortcutInspector => ChildInspector(mouseShortcutAccessor);
		
		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}

		protected override void OnControlGUI(Rect position)
		{
			var keyboardPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			var mousePosition = new Rect
			(
				position.x,
				keyboardPosition.yMax + EditorGUIUtility.standardVerticalSpacing,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			keyboardShortcutInspector.DrawControl(keyboardPosition);
			mouseShortcutInspector.DrawControl(mousePosition);
		}
	}
}