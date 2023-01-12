using System;
using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class DiscreteNumberInspector<T> : Inspector
	{
		private readonly InspectorRangeAttribute rangeAttribute;

		protected DiscreteNumberInspector(Accessor accessor) : base(accessor)
		{
			rangeAttribute = accessor.GetAttribute<InspectorRangeAttribute>();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			T newValue;
			var oldValue = Convert.ToInt32(accessor.value);

			if (rangeAttribute != null)
			{
				newValue = (T)Convert.ChangeType(EditorGUI.IntSlider(position, oldValue, (int)rangeAttribute.min, (int)rangeAttribute.max), typeof(T));
			}
			else
			{
				newValue = (T)Convert.ChangeType(LudiqGUI.DraggableIntField(position, oldValue), typeof(T));
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