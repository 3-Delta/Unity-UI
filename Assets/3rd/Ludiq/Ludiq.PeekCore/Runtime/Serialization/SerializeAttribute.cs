using System;
using JetBrains.Annotations;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	[MeansImplicitUse]
	public class SerializeAttribute : Attribute
	{
		public SerializeAttribute() { }
	}
}