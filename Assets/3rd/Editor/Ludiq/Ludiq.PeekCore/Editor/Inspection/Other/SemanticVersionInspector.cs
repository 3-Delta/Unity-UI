using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(SemanticVersion), typeof(SemanticVersionInspector))]

namespace Ludiq.PeekCore
{
	public sealed class SemanticVersionInspector : Inspector
	{
		public SemanticVersionInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			if (!SemanticVersion.TryParse(EditorGUI.DelayedTextField(position, ((SemanticVersion)accessor.value).ToString()), out var newSemanticVersion))
			{
				newSemanticVersion = (SemanticVersion)accessor.value;
			}

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newSemanticVersion;
			}
		}
	}
}