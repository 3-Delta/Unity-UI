using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class InspectorPlaceholderLabelAttribute : Attribute
	{
		public InspectorPlaceholderLabelAttribute(string text)
		{
			this.text = text;
		}

		public string text { get; }
	}
}