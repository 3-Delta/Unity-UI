using UnityEditor;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class UnknownEditor : Editor
	{
		public UnknownEditor(Accessor accessor) : base(accessor) { }

		private string message => $"No editor for '{accessor.definedType.DisplayName()}'.";

		protected override void OnInnerGUI(Rect position)
		{
			EditorGUI.HelpBox(position, message, MessageType.Warning);
		}

		protected override float GetInnerHeight(float width)
		{
			return LudiqGUIUtility.GetHelpBoxHeight(message, MessageType.Warning, width);
		}
	}
}