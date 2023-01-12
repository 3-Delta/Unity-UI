using System;
using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Guid), typeof(GuidInspector))]

namespace Ludiq.PeekCore
{
	public sealed class GuidInspector : Inspector
	{
		public GuidInspector(Accessor accessor) : base(accessor) { }
		
		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			Guid newGuid;

			var fieldPosition = new Rect
			(
				position.x,
				position.y,
				position.width - Styles.buttonSpacing - Styles.buttonWidth,
				EditorGUIUtility.singleLineHeight
			);

			var buttonPosition = new Rect
			(
				position.xMax - Styles.buttonWidth,
				position.y,
				Styles.buttonWidth,
				EditorGUIUtility.singleLineHeight
			);

			try
			{
				newGuid = new Guid(EditorGUI.DelayedTextField(fieldPosition, ((Guid)accessor.value).ToString()));
			}
			catch
			{
				newGuid = (Guid)accessor.value;
			}

			if (GUI.Button(buttonPosition, "New GUID"))
			{
				newGuid = Guid.NewGuid();
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newGuid;
			}
		}

		public static class Styles
		{
			public static readonly float buttonWidth = 70;
			public static readonly float buttonSpacing = 3;
		}
	}
}