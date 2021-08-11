using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class InspectableIfAttribute : Attribute, IInspectableAttribute
	{
		public InspectableIfAttribute(string conditionMemberName)
		{
			this.conditionMemberName = conditionMemberName;
		}

		public int order { get; set; }

		public string conditionMemberName { get; }
	}
}