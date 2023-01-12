using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(LooseAssemblyName), typeof(LooseAssemblyNameInspector))]

namespace Ludiq.PeekCore
{
	public sealed class LooseAssemblyNameInspector : Inspector
	{
		public LooseAssemblyNameInspector(Accessor accessor) : base(accessor) { }
		
		private IFuzzyOptionTree GetOptions()
		{
			return new LooseAssemblyNameOptionTree();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();
			
			var newAssemblyName = LudiqGUI.AssemblyField(position, GUIContent.none, (LooseAssemblyName)accessor.value, GetOptions);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newAssemblyName;
			}
		}
	}
}