using System;

namespace Ludiq.PeekCore
{
	public interface IRegisterDecoratorAttribute
	{
		Type decoratedType { get; }
		Type decoratorType { get; }
	}
}