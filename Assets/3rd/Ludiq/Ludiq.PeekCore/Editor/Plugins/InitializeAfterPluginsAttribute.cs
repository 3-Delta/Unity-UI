using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class InitializeAfterPluginsAttribute : Attribute, ITypeRegistrationAttribute
	{
		public InitializeAfterPluginsAttribute(Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);
			this.type = type;
		}

		public Type type { get; }
	}
}