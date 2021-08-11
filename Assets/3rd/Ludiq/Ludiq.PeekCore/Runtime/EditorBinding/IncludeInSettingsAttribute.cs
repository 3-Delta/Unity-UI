using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	[Obsolete("Use the Extract attribute instead.")]
	public sealed class IncludeInSettingsAttribute : Attribute
	{
		public IncludeInSettingsAttribute(bool include)
		{
			this.include = include;
		}

		public bool include { get; private set; }
	}
}