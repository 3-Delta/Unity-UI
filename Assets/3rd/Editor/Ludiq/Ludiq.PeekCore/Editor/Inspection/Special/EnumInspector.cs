using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class EnumInspector : Inspector
	{
		public EnumInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			accessor.instantiate = true;

			base.Initialize();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var enumType = accessor.value.GetType();

			Enum newValue;

			if (enumType.IsPseudoFlagsEnum())
			{
				newValue = EditorGUI.EnumFlagsField(position, (Enum)accessor.value);
			}
			else
			{
				newValue = EditorGUI.EnumPopup(position, (Enum)accessor.value);
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override float GetControlWidth()
		{
			return Mathf.Max(18, EditorStyles.popup.CalcSize(LudiqGUI.GetEnumPopupContent((Enum)accessor.value)).x + 1);
		}
	}
}