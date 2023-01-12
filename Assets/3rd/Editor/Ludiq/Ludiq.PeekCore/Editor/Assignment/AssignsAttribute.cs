using System;
using JetBrains.Annotations;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	[MeansImplicitUse]
	public sealed class AssignsAttribute : Attribute
	{
		public AssignsAttribute() { }

		public AssignsAttribute(string memberName)
		{
			this.memberName = memberName;
		}

		public string memberName { get; }

		public bool cache { get; set; } = true;
	}
}