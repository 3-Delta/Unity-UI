using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorReorderableAttribute : Attribute
	{
		public InspectorReorderableAttribute(bool reorderable = true)
		{
			this.reorderable = reorderable;
		}

		public bool reorderable { get; private set; }
	}
}