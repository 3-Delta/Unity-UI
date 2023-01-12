using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class MapToPluginAttribute : Attribute, ITypeRegistrationAttribute
	{
		public MapToPluginAttribute(Type type, string pluginId)
		{
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(pluginId)).IsNotNull(pluginId);

			this.type = type;
			this.pluginId = pluginId;
		}

		public Type type { get; }
		public string pluginId { get; }
	}
}