using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public sealed class RenamedFromAttribute : Attribute
	{
		public RenamedFromAttribute(string previousName)
		{
			this.previousName = previousName;
		}

		public string previousName { get; }
	}
}
