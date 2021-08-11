using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RenamedNamespaceAttribute : Attribute
	{
		public RenamedNamespaceAttribute(string previousName, string newName)
		{
			this.previousName = previousName;
			this.newName = newName;
		}

		public string previousName { get; }

		public string newName { get; }
	}
}
