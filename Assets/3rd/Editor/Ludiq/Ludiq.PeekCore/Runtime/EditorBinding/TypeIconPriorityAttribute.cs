using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class TypeIconPriorityAttribute : Attribute
	{
		public TypeIconPriorityAttribute(int priority)
		{
			this.priority = priority;
		}

		public TypeIconPriorityAttribute()
		{
			priority = 0;
		}

		public int priority { get; }
	}
}