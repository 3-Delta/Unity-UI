using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class TypeSetAttribute : Attribute
	{
		public TypeSetAttribute(TypeSet typeSet)
		{
			this.typeSet = typeSet;
		}

		public TypeSet typeSet { get; }
	}
}