using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ludiq.PeekCore;

[assembly: RegisterInspector(typeof(KeyValuePair<,>), typeof(KeyValuePairInspector))]

namespace Ludiq.PeekCore
{
	public sealed class KeyValuePairInspector : Inspector
	{
		public KeyValuePairInspector(Accessor accessor) : base(accessor) { }
		
		private Inspector keyInspector => ChildInspector(nameof(KeyValuePair<object,object>.Key));
		private Inspector valueInspector => ChildInspector(nameof(KeyValuePair<object,object>.Value));
		
		protected override float GetControlHeight(float width)
		{
			return Mathf.Max(keyInspector.FieldHeight(width), valueInspector.FieldHeight(width)) + Styles.topPadding;
		}

		protected override void OnControlGUI(Rect position)
		{
			EditorGUI.BeginChangeCheck();

			var keyPosition = new Rect
			(
				position.x,
				position.y + Styles.topPadding,
				(position.width - Styles.spacing) / 2,
				keyInspector.FieldHeight(position.width)
			);

			var valuePosition = new Rect
			(
				keyPosition.xMax + Styles.spacing,
				position.y + Styles.topPadding,
				(position.width - Styles.spacing) / 2,
				valueInspector.FieldHeight(position.width)
			);

			EditorGUI.BeginDisabledGroup(true);
			OnKeyGUI(keyPosition);
			OnValueGUI(valuePosition);
			EditorGUI.EndDisabledGroup();

			EndBlock();
		}

		public void OnKeyGUI(Rect keyPosition)
		{
			keyInspector.DrawControl(keyPosition);
		}

		public void OnValueGUI(Rect valuePosition)
		{
			valueInspector.DrawControl(valuePosition);
		}

		public static class Styles
		{
			public static readonly float topPadding = 2;
			public static readonly float spacing = 5;
		}
	}
}