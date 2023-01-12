using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Vector3Int), typeof(Vector3IntInspector))]

namespace Ludiq.PeekCore
{
	public class Vector3IntInspector : VectorInspector
	{
		public Vector3IntInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Vector3Int newValue;
			
			if (adaptiveWidth)
			{
				newValue = LudiqGUI.AdaptiveVector3IntField(position, GUIContent.none, (Vector3Int)accessor.value);
			}
			else if (position.width <= Styles.compactThreshold)
			{
				newValue = LudiqGUI.CompactVector3IntField(position, GUIContent.none, (Vector3Int)accessor.value);
			}
			else
			{
				newValue = EditorGUI.Vector3IntField(position, GUIContent.none, (Vector3Int)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			var vector = (Vector3Int)accessor.value;

			return LudiqGUI.GetTextFieldAdaptiveWidth(vector.x) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.y) + LudiqStyles.compactHorizontalSpacing +
				   LudiqGUI.GetTextFieldAdaptiveWidth(vector.z) + LudiqStyles.compactHorizontalSpacing;
		}
	}
}