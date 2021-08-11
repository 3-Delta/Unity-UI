using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class UnknownInspector : Inspector
	{
		public UnknownInspector(Accessor accessor) : base(accessor) { }

		private string GetMessage(GUIContent label)
		{
			var labelText = label.text;

			if (!string.IsNullOrEmpty(labelText))
			{
				return $"{labelText}: No inspector for '{accessor.definedType.DisplayName()}'.";
			}
			else
			{
				return $"No inspector for '{accessor.definedType.DisplayName()}'.";
			}
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.HelpBox(position, GetMessage(label), MessageType.Warning);
		}

		protected override float GetControlHeight(float width)
		{
			return LudiqGUIUtility.GetHelpBoxHeight(GetMessage(label), MessageType.Warning, width);
		}
	}
}