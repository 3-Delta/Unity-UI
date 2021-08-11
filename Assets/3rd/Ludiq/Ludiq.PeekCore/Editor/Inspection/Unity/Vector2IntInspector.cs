using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Vector2Int), typeof(Vector2IntInspector))]

namespace Ludiq.PeekCore
{
	public class Vector2IntInspector : VectorInspector
	{
		public Vector2IntInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Vector2Int newValue;
			
			if (adaptiveWidth)
			{
				newValue = LudiqGUI.AdaptiveVector2IntField(position, GUIContent.none, (Vector2Int)accessor.value);
			}
			else if (position.width <= Styles.compactThreshold)
			{
				newValue = LudiqGUI.CompactVector2IntField(position, GUIContent.none, (Vector2Int)accessor.value);
			}
			else
			{
				newValue = EditorGUI.Vector2IntField(position, GUIContent.none, (Vector2Int)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			var vector = (Vector2Int)accessor.value;

			return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing;
		}
	}
}