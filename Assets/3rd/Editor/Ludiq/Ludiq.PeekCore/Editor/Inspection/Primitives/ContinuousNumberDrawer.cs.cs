using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class ContinuousNumberInspector<T> : Inspector
	{
		private readonly RangeAttribute rangeAttribute;

		protected ContinuousNumberInspector(Accessor accessor) : base(accessor)
		{
			rangeAttribute = accessor.GetAttribute<RangeAttribute>();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			T newValue;
			var oldValue = Convert.ToSingle(accessor.value);

			if (rangeAttribute != null)
			{
				newValue = (T)Convert.ChangeType(EditorGUI.Slider(position, oldValue, rangeAttribute.min, rangeAttribute.max), typeof(T));
			}
			else
			{
				newValue = (T)Convert.ChangeType(LudiqGUI.DraggableFloatField(position, oldValue), typeof(T));
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
		
		protected override float GetControlWidth()
		{
			if (rangeAttribute != null)
			{
				return 100;
			}
			else
			{
				return LudiqGUI.GetTextFieldAdaptiveWidth(accessor.value);
			}
		}
	}
}