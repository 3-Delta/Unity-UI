using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Bounds), typeof(BoundsInspector))]

namespace Ludiq.PeekCore
{
	public class BoundsInspector : Inspector
	{
		public BoundsInspector(Accessor accessor) : base(accessor) { }

		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight * 2;
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = EditorGUI.BoundsField(position, (Bounds)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}