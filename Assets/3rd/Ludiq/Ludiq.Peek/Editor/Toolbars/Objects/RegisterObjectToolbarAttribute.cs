using System;
using UnityObject = UnityEngine.Object;

namespace Ludiq.Peek
{
	// ReSharper disable once RedundantUsingDirective
	using PeekCore;

	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RegisterObjectToolbarAttribute : Attribute, IRegisterDecoratorAttribute
	{
		public RegisterObjectToolbarAttribute(Type decoratedType, Type decoratorType)
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