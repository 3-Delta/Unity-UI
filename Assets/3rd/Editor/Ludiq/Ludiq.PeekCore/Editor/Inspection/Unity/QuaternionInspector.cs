using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Quaternion), typeof(QuaternionInspector))]

namespace Ludiq.PeekCore
{
	public class QuaternionInspector : Inspector
	{
		public QuaternionInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var value = (Quaternion)accessor.value;

			var vector = new Vector4(value.x, value.y, value.z, value.w);

			Vector4 newVector;

			if (adaptiveWidth)
			{
				newVector = LudiqGUI.AdaptiveVector4Field(position, GUIContent.none, vector);
			}
			else if (position.width <= VectorInspector.Styles.compactThreshold)
			{
				newVector = LudiqGUI.CompactVector4Field(position, GUIContent.none, vector);
			}
			else
			{
				newVector = EditorGUI.Vector4Field(position, GUIContent.none, vector);
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				var newValue = new Quaternion(newVector.x, newVector.y, newVector.z, newVector.w);

				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}