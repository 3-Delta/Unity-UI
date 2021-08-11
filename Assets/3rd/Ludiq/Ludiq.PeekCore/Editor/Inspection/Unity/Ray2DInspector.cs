using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Ray2D), typeof(Ray2DInspector))]

namespace Ludiq.PeekCore
{
	public class Ray2DInspector : Inspector
	{
		public Ray2DInspector(Accessor accessor) : base(accessor) { }

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

			Vector2 newOrigin;
			Vector2 newDirection;

			if (wideMode)
			{
				newOrigin = EditorGUI.Vector2Field(originPosition, GUIContent.none, (Vector2)originAccessor.value);
				newDirection = EditorGUI.Vector2Field(directionPosition, GUIContent.none, (Vector2)directionAccessor.value);
			}
			else
			{
				newOrigin = LudiqGUI.CompactVector2Field(originPosition, GUIContent.none, (Vector2)originAccessor.value);
				newDirection = LudiqGUI.CompactVector2Field(directionPosition, GUIContent.none, (Vector2)directionAccessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = new Ray2D(newOrigin, newDirection);
			}
		}

		protected override float GetControlWidth()
		{
			return 100;
		}
	}
}