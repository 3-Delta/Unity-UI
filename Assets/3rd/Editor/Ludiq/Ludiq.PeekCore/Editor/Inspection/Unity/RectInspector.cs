using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Rect), typeof(RectInspector))]

namespace Ludiq.PeekCore
{
	public class RectInspector : Inspector
	{
		public RectInspector(Accessor accessor) : base(accessor) { }

		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight * 2;
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();
			
			var newValue = EditorGUI.RectField(position, GUIContent.none, (Rect)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			return 125;
		}
	}
}