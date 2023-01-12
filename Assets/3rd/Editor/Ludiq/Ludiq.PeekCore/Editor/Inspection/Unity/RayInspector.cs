using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Ray), typeof(RayInspector))]

namespace Ludiq.PeekCore
{
	public class RayInspector : Inspector
	{
		public RayInspector(Accessor accessor) : base(accessor) { }

		protected override float GetControlHeight(float width)
		{
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
		
		private Accessor originAccessor => accessor[nameof(Ray2D.origin)];
		private Accessor directionAccessor => accessor[nameof(Ray2D.direction)];

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var originPosition = new Rect
			(
				position.x,
				position.y,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			var directionPosition = new Rect
			(
				position.x,
				originPosition.yMax + EditorGUIUtility.standardVerticalSpacing,
				position.width,
				EditorGUIUtility.singleLineHeight
			);

			using (LudiqGUIUtility.labelWidth.Override(16))
			{
				originPosition = PrefixLabel(originPosition, new GUIContent("O", "Origin"));
				directionPosition = PrefixLabel(directionPosition, new GUIContent("D", "Direction"));
			}

			Vector3 newOrigin;
			Vector3 newDirection;

			if (wideMode)
			{
				newOrigin = EditorGUI.Vector3Field(originPosition, GUIContent.none, (Vector3)originAccessor.value);
				newDirection = EditorGUI.Vector3Field(directionPosition, GUIContent.none, (Vector3)directionAccessor.value);
			}
			else
			{
				newOrigin = LudiqGUI.CompactVector3Field(originPosition, GUIContent.none, (Vector3)originAccessor.value);
				newDirection = LudiqGUI.CompactVector3Field(directionPosition, GUIContent.none, (Vector3)directionAccessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = new Ray(newOrigin, newDirection);
			}
		}

		protected override float GetControlWidth()
		{
			return 125;
		}
	}
}