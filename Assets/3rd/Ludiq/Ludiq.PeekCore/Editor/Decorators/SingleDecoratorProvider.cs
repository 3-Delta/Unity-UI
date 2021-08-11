using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public abstract class SingleDecoratorProvider<TDecorated, TDecorator, TAttribute>
		where TAttribute : Attribute, IRegisterDecoratorAttribute
	{
		protected readonly object typesLock = new object();
		protected readonly object instancesLock = new object();

		protected SingleDecoratorProvider()
		{
			definedDecoratorTypes = new Dictionary<Type, Type>();
			resolvedDecoratorTypes = new Dictionary<Type, Type>();

			decorators = new Dictionary<TDecorated, TDecorator>(decoratedComparer);
			decorateds = new Dictionary<TDecorator, TDecorated>();

			MapAttributeTypes();

			Freed();

			EditorApplication.update += FreeIfNeeded;

			if (cache)
			{
				EditorApplicationUtility.onEnterEditMode += FreeAll; // Assemblies don't get reloaded
			}
		}

		protected virtual IEqualityComparer<TDecorated> decoratedComparer => null;
		
		protected virtual TDecorator CreateDecorator(Type decoratorType, TDecorated decorated)
		{
			return decoratorType.Instantiate(false, decorated).CastTo<TDecorator>();
		}
		
		private TDecorator CreateDecorator(TDecorated decorated)
		{
			if (!IsValid(decorated, out var reason))
			{
				throw new InvalidOperationException($"Decorated object is not valid: {decorated}.\n{reason}");
			}

			return CreateDecorator(GetDecoratorType(GetDecoratedType(decorated)), decorated);
		}



		#region Type Resolution
		
		protected virtual IEnumerable<Assembly> registrationAssemblies => Codebase.ludiqEditorAssemblies;
		
		protected readonly Dictionary<Type, Type> definedDecoratorTypes;

		protected readonly Dictionary<Type, Type> resolvedDecoratorTypes;
		
		private void MapAttributeTypes()
		{
			foreach (var registration in RuntimeCodebase.GetAssemblyAttributes<TAttribute>(registrationAssemblies))
			{
				if (definedDecoratorTypes.ContainsKey(registration.decoratedType))
				{
					Debug.LogWarning($"Multiple '{typeof(TDecorator)}' for '{registration.decoratedType}'. Ignoring '{registration.decoratorType}'.");
					continue;
				}

				definedDecoratorTypes.Add(registration.decoratedType, registration.decoratorType);
			}
		}

		public bool HasDecorator(Type decoratedType)
		{
			return TryGetDecoratorType(decoratedType, out var decoratorType);
		}

		public bool TryGetDecoratorType(Type decoratedType, out Type decoratorType)
		{
			lock (typesLock)
			{
				Ensure.That(nameof(decoratedType)).IsNotNull(decoratedType);

				if (!resolvedDecoratorTypes.TryGetValue(decoratedType, out decoratorType))
				{
					decoratorType = ResolveDecoratorType(decoratedType);
					resolvedDecoratorTypes.Add(decoratedType, decoratorType);
				}

				return decoratorType != null;
			}
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

		public Type GetDecoratorType(Type decoratedType)
		{
			lock (typesLock)
			{
				Ensure.That(nameof(decoratedType)).IsNotNull(decoratedType);
				
				if (!TryGetDecoratorType(decoratedType, out var decoratorType))
				{
					throw new NotSupportedException(NoDecoratorMessage(decoratedType));
				}

				return decoratorType;
			}
		}

		protected virtual string NoDecoratorMessage(Type decoratedType)
		{
			return $"Cannot decorate '{decoratedType}' with '{typeof(TDecorator)}'.";
		}

		protected virtual Type ResolveDecoratorType(Type decoratedType)
		{
			return ResolveDecoratorTypeByHierarchy(decoratedType);
		}

		protected Type ResolveDecoratorTypeByHierarchy(Type decoratedType, bool inherit = true)
		{
			return ResolveDecoratorTypeByHierarchy(decoratedType, decoratedType, inherit);
		}

		private Type ResolveDecoratorTypeByHierarchy(Type resolvingDecoratedType, Type initialDecoratedType, bool inherit = true)
		{
			// We traverse the tree recursively and manually instead of
			// using Linq to find any "assignable from" type in the defined
			// decorators list in order to preserve priority. 

			// For example, in an A : B : C chain where we have decorators
			// for B and C, this method will always map A to B, not A to C.
			
			var resolved = DirectResolve(resolvingDecoratedType, initialDecoratedType) ?? GenericResolve(resolvingDecoratedType);

			if (resolved != null)
			{
				return resolved;
			}

			if (inherit)
			{
				foreach (var baseTypeOrInterface in resolvingDecoratedType.BaseTypeAndInterfaces(false))
				{
					var baseResolved = ResolveDecoratorTypeByHierarchy(baseTypeOrInterface, initialDecoratedType, false);

					if (baseResolved != null)
					{
						return baseResolved;
					}
				}

				if (resolvingDecoratedType.BaseType != null)
				{
					var baseResolved = ResolveDecoratorTypeByHierarchy(resolvingDecoratedType.BaseType, initialDecoratedType, true);

					if (baseResolved != null)
					{
						return baseResolved;
					}
				}
			}

			if (resolvingDecoratedType.IsEnum)
			{
				var enumResolved = DirectResolve(typeof(Enum));
				if (enumResolved != null)
				{
					return enumResolved;
				}
			}

			return null;
		}

		private Type DirectResolve(Type resolvingDecoratedType, Type initialDecoratedType = null)
		{
			if (definedDecoratorTypes.ContainsKey(resolvingDecoratedType))
			{
				var definedDecoratorType = definedDecoratorTypes[resolvingDecoratedType];
				
				// Try to close-constructor the decorator
				// For example: [Decorator(Decorated)] Decorator<TDecorated> gets properly closed-constructed with type
				// Important to make sure we're close constructing with the initial decorated type, not the type we're currently resolving in the hierarchy
				if (definedDecoratorType.IsGenericTypeDefinition)
				{
					var arguments = definedDecoratorType.GetGenericArguments();
					
					var argumentType = initialDecoratedType ?? resolvingDecoratedType;

					if (arguments.Length == 1 && arguments[0].CanMakeGenericTypeVia(argumentType))
					{
						return definedDecoratorType.MakeGenericType(argumentType);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return definedDecoratorTypes[resolvingDecoratedType];
				}
			}

			return null;
		}

		private Type GenericResolve(Type decoratedType)
		{
			if (decoratedType.IsGenericType)
			{
				var typeDefinition = decoratedType.GetGenericTypeDefinition();

				if (definedDecoratorTypes.ContainsKey(typeDefinition))
				{
					var definedDecoratorType = definedDecoratorTypes[typeDefinition];

					// For example: [Decorator(List<>)] ListDecorator<T> gets passed T of the item
					if (definedDecoratorType.ContainsGenericParameters)
					{
						return definedDecoratorType.MakeGenericType(decoratedType.GetGenericArguments());
					}
					else
					{
						return definedDecoratorType;
					}
				}
			}

			return null;
		}

		#endregion


		#region Cache
		
		protected readonly Dictionary<TDecorated, TDecorator> decorators;

		protected readonly Dictionary<TDecorator, TDecorated> decorateds;

		protected abstract bool cache { get; }

		public bool IsValid(TDecorated decorated)
		{
			return IsValid(decorated, out var reason);
		}

		public virtual bool IsValid(TDecorated decorated, out string reason)
		{
			reason = null;

			if (decorated is IObservableDisposable disposable && disposable.IsDisposed)
			{
				reason = "Object has been disposed.";
				return false;
			}

			if (decorated.IsUnityNull())
			{
				reason = "Unity object has been destroyed.";
				return false;
			}

			return true;
		}
		
		public TDecorator GetDecorator(TDecorated decorated)
		{
			Ensure.That(nameof(decorated)).IsNotNull(decorated);

			if (!cache)
			{
				var decorator = CreateDecorator(decorated);
				(decorator as IInitializable)?.Initialize();
				return decorator;
			}

			lock (instancesLock)
			{
				var decoratorExists = decorators.TryGetValue(decorated, out var decorator);

				if (decoratorExists && !IsValid(decorateds[decorator]))
				{
					Free(decorator);

					decoratorExists = false;
				}

				if (!decoratorExists)
				{
					decorator = CreateDecorator(decorated);

					decorators.Add(decorated, decorator);
					decorateds.Add(decorator, decorated);
					
					(decorator as IInitializable)?.Initialize();
				}

				return decorator;
			}
		}

		public T GetDecorator<T>(TDecorated decorated) where T : TDecorator
		{
			return GetDecorator(decorated).CastTo<T>();
		}



		#endregion



		#region Collection
		
		private DateTime lastFreeTime;

		protected virtual TimeSpan freeInterval => TimeSpan.FromSeconds(5);

		private void Freed()
		{
			lastFreeTime = DateTime.UtcNow;
		}

		private bool shouldFree => cache && DateTime.UtcNow > lastFreeTime + freeInterval;

		private void FreeIfNeeded()
		{
			if (shouldFree)
			{
				FreeInvalid();
			}
		}

		public void Free(TDecorator decorator)
		{
			lock (instancesLock)
			{
				if (decorateds.ContainsKey(decorator))
				{
					(decorator as IDisposable)?.Dispose();
					var decorated = decorateds[decorator];

					decorateds.Remove(decorator);

					if (!decorators.Remove(decorated))
					{
						Debug.LogWarning($"Could not remove decorators mapped to '{decorated.ToSafeString()}'.");
					}
				}
			}
		}

		public void Free(IEnumerable<TDecorator> decorators)
		{
			foreach (var decorator in decorators)
			{
				Free(decorator);
			}
		}

		public void FreeInvalid()
		{
			if (!cache)
			{
				Debug.LogWarning($"Trying to free a decorator provider without caching: {this}");

				return;
			}

			lock (instancesLock)
			{
				Free(decorators.Where(d => !IsValid(d.Key)).Select(d => d.Value).ToArray());
				Freed();
			}
		}

		public void FreeAll()
		{
			if (!cache)
			{
				Debug.LogWarning($"Trying to free a decorator provider without caching: {this}");

				return;
			}

			lock (instancesLock)
			{
				foreach (var decorator in decorators.Values)
				{
					(decorator as IDisposable)?.Dispose();
				}

				decorators.Clear();
				decorateds.Clear();
				
				Freed();
			}
		}

		#endregion
	}
}