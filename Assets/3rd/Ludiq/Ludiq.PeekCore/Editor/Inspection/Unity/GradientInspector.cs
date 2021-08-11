using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(Gradient), typeof(GradientInspector))]

namespace Ludiq.PeekCore
{
	public class GradientInspector : Inspector
	{
		public GradientInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			accessor.instantiate = true;
			accessor.instantiator = typeof(Gradient).PseudoDefault;

			base.Initialize();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = EditorGUI.GradientField(position, (Gradient)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}