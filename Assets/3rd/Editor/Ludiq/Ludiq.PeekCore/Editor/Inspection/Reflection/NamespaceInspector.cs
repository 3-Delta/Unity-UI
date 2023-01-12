using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Namespace), typeof(NamespaceInspector))]

namespace Ludiq.PeekCore
{
	public class NamespaceInspector : Inspector
	{
		public NamespaceInspector(Accessor accessor) : base(accessor) { }
		
		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = (Namespace)EditorGUI.TextField(position, ((Namespace)accessor.value)?.FullName);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}