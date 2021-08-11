using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorObjectTypeAttribute : Attribute
	{
		public InspectorObjectTypeAttribute(string memberName)
		{
			this.memberName = memberName;
		}

		public string memberName { get; }
	}
}