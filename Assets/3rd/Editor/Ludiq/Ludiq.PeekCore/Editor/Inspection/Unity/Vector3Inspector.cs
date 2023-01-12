using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Vector3), typeof(Vector3Inspector))]

namespace Ludiq.PeekCore
{
	public class Vector3Inspector : VectorInspector
	{
		public Vector3Inspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Vector3 newValue;
			
			if (adaptiveWidth)
			{
				newValue = LudiqGUI.AdaptiveVector3Field(position, GUIContent.none, (Vector3)accessor.value);
			}
			else if (position.width <= Styles.compactThreshold)
			{
				newValue = LudiqGUI.CompactVector3Field(position, GUIContent.none, (Vector3)accessor.value);
			}
			else
			{
				newValue = EditorGUI.Vector3Field(position, GUIContent.none, (Vector3)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			var vector = (Vector3)accessor.value;

			return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.z) + LudiqStyles.compactHorizontalSpacing;
		}
	}
}