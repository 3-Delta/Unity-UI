using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorRangeAttribute : Attribute
	{
		public InspectorRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float min { get; private set; }
		public float max { get; private set; }
	}
}