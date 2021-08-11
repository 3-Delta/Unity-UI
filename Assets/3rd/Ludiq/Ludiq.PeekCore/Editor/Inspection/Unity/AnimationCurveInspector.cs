using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(AnimationCurve), typeof(AnimationCurveInspector))]

namespace Ludiq.PeekCore
{
	public class AnimationCurveInspector : Inspector
	{
		public AnimationCurveInspector(Accessor accessor) : base(accessor) { }

		public override void Initialize()
		{
			accessor.instantiate = true;
			accessor.instantiator = typeof(AnimationCurve).PseudoDefault;

			base.Initialize();
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var newValue = EditorGUI.CurveField(position, (AnimationCurve)accessor.value);

			if (EditorGUI.EndChangeCheck())
			{
				accessor.RecordUndo();
				accessor.value = newValue;
			}
		}
	}
}