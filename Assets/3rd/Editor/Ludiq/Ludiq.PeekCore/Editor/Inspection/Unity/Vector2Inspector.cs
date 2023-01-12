using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Vector2), typeof(Vector2Inspector))]

namespace Ludiq.PeekCore
{
	public class Vector2Inspector : VectorInspector
	{
		public Vector2Inspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Vector2 newValue;
			
			if (adaptiveWidth)
			{
				newValue = LudiqGUI.AdaptiveVector2Field(position, GUIContent.none, (Vector2)accessor.value);
			}
			else if (position.width <= Styles.compactThreshold)
			{
				newValue = LudiqGUI.CompactVector2Field(position, GUIContent.none, (Vector2)accessor.value);
			}
			else
			{
				newValue = EditorGUI.Vector2Field(position, GUIContent.none, (Vector2)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			var vector = (Vector2)accessor.value;

			return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing;
		}
	}
}