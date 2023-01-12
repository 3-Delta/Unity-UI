using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(bool), typeof(BoolInspector))]

namespace Ludiq.PeekCore
{
	public class BoolInspector : Inspector
	{
		private readonly InspectorToggleLeftAttribute toggleLeftAttribute;

		public BoolInspector(Accessor accessor) : base(accessor)
		{
			toggleLeftAttribute = accessor.GetAttribute<InspectorToggleLeftAttribute>();
		}

		protected override float GetControlWidth()
		{
			return 14;
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = EditorGUI.Toggle(position, (bool)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}

		protected override void OnFieldGUI(Rect position)
		{
			if (toggleLeftAttribute != null)
			{
				var togglePosition = position.VerticalSection(ref y, EditorGUIUtility.singleLineHeight);

				EditorGUI.BeginChangeCheck();

				bool newValue = EditorGUI.ToggleLeft(togglePosition, label, (bool)accessor.value, labelStyle);

				if (EditorGUI.EndChangeCheck())
				{
					accessor.RecordUndo();
					accessor.value = newValue;
				}
			}
			else
			{
				base.OnFieldGUI(position);
			}
		}

		protected override GUIStyle LabelStyle(GUIStyle original)
		{
			var labelStyle = base.LabelStyle(original);

			if (toggleLeftAttribute != null)
			{
				labelStyle = new GUIStyle(labelStyle);
				labelStyle.padding.left = 2;
			}

			return labelStyle;
		}
	}
}