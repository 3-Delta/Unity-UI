using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorDelayedAttribute : Attribute { }
}