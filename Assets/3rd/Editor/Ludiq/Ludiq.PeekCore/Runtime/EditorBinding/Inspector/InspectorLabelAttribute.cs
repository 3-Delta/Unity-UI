using System;
using UnityEngine;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorLabelAttribute : Attribute
	{
		public InspectorLabelAttribute(string text)
		{
			this.text = text;
		}

		public InspectorLabelAttribute(string text, string tooltip)
		{
			this.text = text;
			this.tooltip = tooltip;
		}

		public string text { get; private set; }
		public string tooltip { get; private set; }
		public Texture image { get; set; }
	}
}