using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class NullableInspector : Inspector
	{
		public NullableInspector(Accessor accessor) : base(accessor)
		{
			var underlyingType = Nullable.GetUnderlyingType(accessor.definedType);

			underlyingAccessor = accessor.Lambda("__underlying", accessor.value ?? Activator.CreateInstance(underlyingType), underlyingType);
		}

		private readonly Accessor underlyingAccessor;

		private Inspector underlyingInspector => ChildInspector(underlyingAccessor);
		
		protected override float GetControlHeight(float width)
		{
			return underlyingInspector.FieldHeight(width);
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var togglePosition = new Rect
			(
				position.x,
				position.y,
				EditorGUIUtility.singleLineHeight,
				position.height
			);

			var fieldPosition = new Rect
			(
				togglePosition.xMax + Styles.toggleSpacing,
				position.y,
				position.width - togglePosition.width - Styles.toggleSpacing,
				position.height
			);

			var hasValue = EditorGUI.Toggle(togglePosition, GUIContent.none, accessor.value != null);

			EditorGUI.BeginDisabledGroup(!hasValue);
			underlyingInspector.DrawControl(fieldPosition);
			EditorGUI.EndDisabledGroup();

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = hasValue ? underlyingAccessor.value : null;
			}
		}

		public static class Styles
		{
			public static readonly int toggleSpacing = 3;
		}
	}
}