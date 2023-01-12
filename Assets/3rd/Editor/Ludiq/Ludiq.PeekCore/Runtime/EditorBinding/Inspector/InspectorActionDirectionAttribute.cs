using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorActionDirectionAttribute : Attribute
	{
		public InspectorActionDirectionAttribute(MemberAction action)
		{
			this.action = action;
		}

		public MemberAction action { get; private set; }
	}
}