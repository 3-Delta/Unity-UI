using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class MapToProductAttribute : Attribute, ITypeRegistrationAttribute
	{
		public MapToProductAttribute(Type type, string productId)
		{
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(productId)).IsNotNull(productId);

			this.type = type;
			this.productId = productId;
		}

		public Type type { get; }
		public string productId { get; }
	}
}