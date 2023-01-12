using System;

namespace Ludiq.PeekCore
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RegisterFactoryAttribute : Attribute, IRegisterDecoratorAttribute
	{
		public RegisterFactoryAttribute(Type decoratedType, Type decoratorType)
		{
			Ensure.That(nameof(decoratedType)).IsNotNull(decoratedType);
			Ensure.That(nameof(decoratorType)).IsNotNull(decoratorType);

			this.decoratedType = decoratedType;
			this.decoratorType = decoratorType;
		}

		public Type decoratedType { get; }
		public Type decoratorType { get; }
	}
}