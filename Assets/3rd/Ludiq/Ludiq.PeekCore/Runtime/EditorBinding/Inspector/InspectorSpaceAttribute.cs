using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorSpaceAttribute : Attribute
	{
		public InspectorSpaceAttribute(float above, float below)
		{
			this.above = above;
			this.below = below;
		}

		public float above { get;}

		public float below { get;}
	}
}