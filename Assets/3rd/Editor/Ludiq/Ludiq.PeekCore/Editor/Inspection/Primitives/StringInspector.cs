using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(string), typeof(StringInspector))]

namespace Ludiq.PeekCore
{
	public class StringInspector : Inspector
	{
		private readonly InspectorTextAreaAttribute textAreaAttribute;
		private readonly InspectorWrapAttribute wrapAttribute;
		private readonly InspectorDelayedAttribute delayedAttribute;
		private readonly InspectorPlaceholderLabelAttribute placeholderLabelAttribute;

		public StringInspector(Accessor accessor) : base(accessor)
		{
			textAreaAttribute = accessor.GetAttribute<InspectorTextAreaAttribute>();
			wrapAttribute = accessor.GetAttribute<InspectorWrapAttribute>();
			delayedAttribute = accessor.GetAttribute<InspectorDelayedAttribute>();
			placeholderLabelAttribute = accessor.GetAttribute<InspectorPlaceholderLabelAttribute>();
		}

		private string _placeholder;

		public string placeholder
		{
			get => _placeholder ?? placeholderLabelAttribute?.text;
			set => _placeholder = value;
		}

		protected override bool cacheControlHeight => textAreaAttribute != null;

		protected override float GetControlHeight(float width)
		{
			if (textAreaAttribute != null)
			{
				var height = LudiqStyles.textAreaWordWrapped.CalcHeight(new GUIContent((string)accessor.value), width);

				if (textAreaAttribute.hasMinLines)
				{
					var minHeight = EditorStyles.textArea.lineHeight * textAreaAttribute.minLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

					height = Mathf.Max(height, minHeight);
				}

				if (textAreaAttribute.hasMaxLines)
				{
					var maxHeight = EditorStyles.textArea.lineHeight * textAreaAttribute.maxLines + EditorStyles.textArea.padding.top + EditorStyles.textArea.padding.bottom;

					height = Mathf.Min(height, maxHeight);
				}

				return height;
			}
			else if (wrapAttribute != null)
			{
				return LudiqStyles.textFieldWordWrapped.CalcHeight(new GUIContent((string)accessor.value), width);
			}
			else
			{
				return EditorGUIUtility.singleLineHeight;
			}
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			string newValue;

			if (textAreaAttribute != null)
			{
				newValue = EditorGUI.TextArea(position, (string)accessor.value, EditorStyles.textArea);
			}
			else
			{
				var style = wrapAttribute != null ? LudiqStyles.textFieldWordWrapped : EditorStyles.textField;

				if (delayedAttribute != null)
				{
					newValue = EditorGUI.DelayedTextField(position, (string)accessor.value, style);
				}
				else
				{
					newValue = EditorGUI.TextField(position, (string)accessor.value, style);
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}

			if (placeholder != null && string.IsNullOrEmpty(newValue))
			{
				GUI.Label(position, placeholder, Styles.placeholder);
			}
		}

		protected override float GetControlWidth()
		{
			var value = (string)accessor.value;

			if (placeholder != null && string.IsNullOrEmpty(value))
			{
				value = placeholder;
			}

			return LudiqGUI.GetTextFieldAdaptiveWidth(value);
		}

		public static class Styles
		{
			static Styles()
			{
				placeholder = new GUIStyle(EditorStyles.label);
				placeholder.normal.textColor = EditorStyles.centeredGreyMiniLabel.normal.textColor;
				placeholder.alignment = TextAnchor.MiddleLeft;
				placeholder.padding = new RectOffset(4, 4, 0, 0);
			}

			public static readonly GUIStyle placeholder;
		}
	}
}