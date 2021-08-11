using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(LayerMask), typeof(LayerMaskInspector))]

namespace Ludiq.PeekCore
{
	public class LayerMaskInspector : Inspector
	{
		public LayerMaskInspector(Accessor accessor) : base(accessor) { }

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();
			
			var newValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUI.MaskField
			(
				position,
				InternalEditorUtility.LayerMaskToConcatenatedLayersMask((LayerMask)accessor.value),
				InternalEditorUtility.layers
			));

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}