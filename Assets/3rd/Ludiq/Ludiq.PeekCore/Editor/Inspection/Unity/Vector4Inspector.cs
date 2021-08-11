using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Vector4), typeof(Vector4Inspector))]

namespace Ludiq.PeekCore
{
	public class Vector4Inspector : VectorInspector
	{
		public Vector4Inspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Vector4 newValue;

			if (adaptiveWidth)
			{
				newValue = LudiqGUI.AdaptiveVector4Field(position, GUIContent.none, (Vector4)accessor.value);
			}
			else if (position.width <= Styles.compactThreshold)
			{
				newValue = LudiqGUI.CompactVector4Field(position, GUIContent.none, (Vector4)accessor.value);
			}
			else
			{
				newValue = EditorGUI.Vector4Field(position, GUIContent.none, (Vector4)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			var vector = (Vector4)accessor.value;

			return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.z) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.w) + LudiqStyles.compactHorizontalSpacing;
		}
	}
}