using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Obsolete("All units should now be AOT compatible via code generation.")]
	public sealed class AotIncompatibleAttribute : Attribute
	{
		public AotIncompatibleAttribute() : base() { }
	}
}
