using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public abstract class MultiDecoratorProvider<TDecorated, TDecorator, TAttribute>
		where TAttribute : Attribute, IRegisterDecoratorAttribute
	{
		protected MultiDecoratorProvider()
		{
			definedDecoratorTypes = new Dictionary<Type, HashSet<Type>>();
			resolvedDecoratorTypes = new Dictionary<Type, HashSet<Type>>();
			decorators = new Dictionary<TDecorated, HashSet<TDecorator>>();

			foreach (var registration in RuntimeCodebase.GetAssemblyAttributes<TAttribute>(registrationAssemblies))
			{
				if (!definedDecoratorTypes.TryGetValue(registration.decoratedType, out var _definedDecoratorTypes))
				{
					_definedDecoratorTypes = new HashSet<Type>();
					definedDecoratorTypes.Add(registration.decoratedType, _definedDecoratorTypes);
				}

				_definedDecoratorTypes.Add(registration.decoratorType);
			}
		}

		protected virtual IEnumerable<Assembly> registrationAssemblies => Codebase.ludiqEditorAssemblies;

		private readonly Dictionary<TDecorated, HashSet<TDecorator>> decorators;
		protected readonly Dictionary<Type, HashSet<Type>> definedDecoratorTypes;
		private readonly Dictionary<Type, HashSet<Type>> resolvedDecoratorTypes;

		protected virtual IEnumerable<TDecorator> CreateDecorators(TDecorated decorated)
		{
			return GetDecoratorTypes(GetDecoratedType(decorated)).Select(t => (TDecorator)t.Instantiate(false, decorated));
		}

		protected virtual Type GetDecoratedType(TDecorated decorated)
		{
			var type = decorated as Type;

			if (type != null)
			{
				return type;
			}

			return decorated.GetType();
		}

		protected virtual bool ShouldInvalidateDecorators(TDecorated decorated)
		{
			return false;
		}

		public IEnumerable<TDecorator> GetDecorators(TDecorated decorated)
		{
			lock (decorators)
			{
				if (decorators.ContainsKey(decorated) && ShouldInvalidateDecorators(decorated))
				{
					foreach (var disposableDecorator in decorators[decorated].OfType<IDisposable>())
					{
						disposableDecorator.Dispose();
					}

					decorators.Remove(decorated);
				}

				if (!decorators.ContainsKey(decorated))
				{
					decorators.Add(decorated, CreateDecorators(decorated).ToHashSet());
				}

				return decorators[decorated];
			}
		}

		public IEnumerable<TSpecificDecorator> GetDecorators<TSpecificDecorator>(TDecorated decorated) where TSpecificDecorator : TDecorator
		{
			return GetDecorators(decorated).OfType<TSpecificDecorator>();
		}

		public virtual void ClearCache()
		{
			lock (decorators)
			{
				foreach (var decorator in decorators.SelectMany(kvp => kvp.Value).OfType<IDisposable>())
				{
					decorator.Dispose();
				}

				decorators.Clear();
			}
		}

		public bool HasDecorator(Type type)
		{
			lock (decorators)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if (!resolvedDecoratorTypes.TryGetValue(type, out var resolved))
				{
					resolved = ResolveDecoratorTypes(type).ToHashSet();
					resolvedDecoratorTypes.Add(type, resolved);
				}

				return resolved.Count > 0;
			}
		}

		public IEnumerable<Type> GetDecoratorTypes(Type type)
		{
			lock (decorators)
			{
				if (type == null)
				{
					throw new ArgumentNullException(nameof(type));
				}

				if (!HasDecorator(type))
				{
					// Nope, that's fine.
					// throw new NotSupportedException($"Found no decorator type for {type}.");
				}

				return resolvedDecoratorTypes[type];
			}
		}

		protected virtual IEnumerable<Type> ResolveDecoratorTypes(Type type)
		{
			return ResolveDecoratorTypesByHierarchy(type, true);
		}

		protected IEnumerable<Type> ResolveDecoratorTypesByHierarchy(Type type, bool inherit)
		{
			// We traverse the tree recursively and manually instead of
			// using Linq to find any "assignable from" type in the defined
			// decorators list in order to preserve priority. 

			// For example, in an A : B : C chain where we have decorators
			// for B and C, this method will always map A to B, not A to C.
			
			var found = false;

			foreach (var directResolved in DirectResolve(type))
			{
				yield return directResolved;
				found = true;
			}

			if (found)
			{
				yield break;
			}

			foreach (var genericResolved in GenericResolve(type))
			{
				yield return genericResolved;
				found = true;
			}

			if (found)
			{
				yield break;
			}

			if (inherit)
			{
				foreach (var baseTypeOrInterface in type.BaseTypeAndInterfaces(false))
				{
					foreach (var baseResolved in ResolveDecoratorTypesByHierarchy(baseTypeOrInterface, false))
					{
						yield return baseResolved;
						found = true;
					} 

					if (found)
					{
						yield break;
					}
				}

				if (type.BaseType != null)
				{
					foreach (var baseResolved in ResolveDecoratorTypesByHierarchy(type.BaseType, true))
					{
						yield return baseResolved;
						found = true;
					}

					if (found)
					{
						yield break;
					}
				}
			}
		}

		protected IEnumerable<Type> DirectResolve(Type type)
		{
			if (definedDecoratorTypes.TryGetValue(type, out var directMatches))
			{
				return directMatches;
			}

			return Enumerable.Empty<Type>();
		}

		protected IEnumerable<Type> GenericResolve(Type type)
		{
			if (type.IsGenericType)
			{
				var typeDefinition = type.GetGenericTypeDefinition();

				if (definedDecoratorTypes.TryGetValue(typeDefinition, out var genericMatches))
				{
					foreach (var definedDecoratorType in genericMatches)
					{
						// For example: [Decorator(List<>)] ListDecorator<T> gets passed T of the item
						if (definedDecoratorType.ContainsGenericParameters)
						{
							yield return definedDecoratorType.MakeGenericType(type.GetGenericArguments());
						}
						else
						{
							yield return definedDecoratorType;
						}
					}
				}
			}
		}
	}
}