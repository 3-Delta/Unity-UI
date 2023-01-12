using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore
{
	public class FactoryProvider : MultiDecoratorProvider<Type, IFactory, RegisterFactoryAttribute>
	{
		public static FactoryProvider instance { get; } = new FactoryProvider();

		protected override IEnumerable<Type> ResolveDecoratorTypes(Type type)
		{
			var found = false;

			foreach (var baseResolved in base.ResolveDecoratorTypes(type))
			{
				yield return baseResolved;
				found = true;
			}

			if (found)
			{
				// Not sure we should stop?
				// yield break;
			}

			foreach (var childrenResolved in ChildrenResolve(type))
			{
				yield return childrenResolved;
				found = true;
			}
		}
		
		protected IEnumerable<Type> ChildrenResolve(Type type)
		{
			foreach (var definedDecoratorType in definedDecoratorTypes)
			{
				var decoratedType = definedDecoratorType.Key;
				var decoratorTypes = definedDecoratorType.Value;

				if (type.IsAssignableFrom(decoratedType))
				{
					foreach (var decoratorType in decoratorTypes)
					{
						yield return decoratorType;
					}
				}
			}
		}
	}

	public static class XFactoryProvider
	{
		public static IEnumerable<IFactory> Factories(this Type t)
		{
			return FactoryProvider.instance.GetDecorators(t);
		}

		public static IEnumerable<T> Factories<T>(this Type t) where T : IFactory
		{
			return FactoryProvider.instance.GetDecorators<T>(t);
		}
	}
}
