using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(char), typeof(CharInspector))]

namespace Ludiq.PeekCore
{
	public class CharInspector : Inspector
	{
		public CharInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newString = EditorGUI.TextField(position, ((char)accessor.value).ToString());

			char newValue;

			if (string.IsNullOrEmpty(newString))
			{
				newValue = (char)0;
			}
			else
			{
				newValue = newString[0];
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}