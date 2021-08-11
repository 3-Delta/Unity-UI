using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterProductAttribute : Attribute, ITypeRegistrationAttribute
	{
		public RegisterProductAttribute(Type type, string id)
		{
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(id)).IsNotNull(id);

			this.type = type;
			this.id = id;
		}

		public Type type { get; }
		public string id { get; }
	}
}