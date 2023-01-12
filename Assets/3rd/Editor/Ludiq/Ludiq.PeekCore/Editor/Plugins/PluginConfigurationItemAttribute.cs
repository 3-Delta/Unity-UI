using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class PluginConfigurationItemAttribute : Attribute
	{
		protected PluginConfigurationItemAttribute() { }

		protected PluginConfigurationItemAttribute(string key)
		{
			this.key = key;
		}

		public string key { get; }

		public bool visible { get; set; } = true;
		public bool editable { get; set; } = true;
		public bool resettable { get; set; } = true;

		public string visibleCondition { get; set; } = null;
		public string editableCondition { get; set; } = null;
		public string resettableCondition { get; set; } = null;
	}
}