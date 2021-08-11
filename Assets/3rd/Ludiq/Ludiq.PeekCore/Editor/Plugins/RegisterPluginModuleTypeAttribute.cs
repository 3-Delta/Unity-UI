using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterPluginModuleTypeAttribute : Attribute, ITypeRegistrationAttribute
	{
		public RegisterPluginModuleTypeAttribute(Type type, bool required)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			this.type = type;
			this.required = required;
		}

		public Type type { get; }
		public bool required { get; }
	}
}