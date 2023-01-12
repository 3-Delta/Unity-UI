using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class NullMeansSelfAttribute : Attribute
	{
		public NullMeansSelfAttribute() { }
	}
}