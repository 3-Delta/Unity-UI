using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorFieldWidthAttribute : Attribute
	{
		public InspectorFieldWidthAttribute(float width)
		{
			this.width = width;
		}

		public float width { get; private set; }
	}
}