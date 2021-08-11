using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DoNotSerializeAttribute : Attribute
	{
		public DoNotSerializeAttribute() { }
	}
}