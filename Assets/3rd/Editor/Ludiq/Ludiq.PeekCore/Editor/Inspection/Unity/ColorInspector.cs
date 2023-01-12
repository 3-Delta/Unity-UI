using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Color), typeof(ColorInspector))]

namespace Ludiq.PeekCore
{
	public class ColorInspector : Inspector
	{
		public ColorInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = EditorGUI.ColorField(position, (Color)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}