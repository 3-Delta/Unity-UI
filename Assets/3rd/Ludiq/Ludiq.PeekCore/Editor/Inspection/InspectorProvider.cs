using System;
using System.Collections;
using System.Diagnostics;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public class InspectorProvider : SingleDecoratorProvider<Accessor, Inspector, RegisterInspectorAttribute>
	{
		protected override bool cache => false;

		protected override Type GetDecoratedType(Accessor accessor)
		{
			var inspectedType = accessor.definedType;
			
			foreach (var attribute in accessor.GetAttributes<Attribute>())
			{
				var attributeType = attribute.GetType();

				if (HasInspector(attributeType))
				{
					inspectedType = attributeType;
				}
			}

			if (inspectedType == null)
			{
				throw new InvalidOperationException("Accessor has no defined type nor any attribute with an inspector.");
			}

			return inspectedType;
		}

		protected override Type ResolveDecoratorType(Type decoratedType)
		{
			return
				ResolveDecoratorTypeByHierarchy(decoratedType) ??
				UnityObjectReferenceInspector(decoratedType) ??
				EnumInspector(decoratedType) ??
				NullableInspector(decoratedType) ??
				ListInspector(decoratedType) ??
				DictionaryInspector(decoratedType) ??
				SystemObjectInspector(decoratedType) ??
				CustomPropertyDrawerInspector(decoratedType) ??
				AutomaticReflectedInspector(decoratedType) ??
				typeof(UnknownInspector);
		}

		protected Type UnityObjectReferenceInspector(Type type)
		{
			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return typeof(UnityObjectInspector);
			}

			return null;
		}

		private Type EnumInspector(Type type)
		{
			if (type.IsEnum)
			{
				return typeof(EnumInspector);
			}

			return null;
		}

		private Type ListInspector(Type type)
		{
			if (type.IsArray || (typeof(IList).IsAssignableFrom(type) && type.IsConcrete()))
			{
				return typeof(ListInspector);
			}

			return null;
		}

		private Type DictionaryInspector(Type type)
		{
			if (typeof(IDictionary).IsAssignableFrom(type) && type.IsConcrete())
			{
				return typeof(DictionaryInspector);
			}

			return null;
		}

		private Type NullableInspector(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return typeof(NullableInspector);
			}

			return null;
		}

		private Type SystemObjectInspector(Type type)
		{
			if (type == typeof(object))
			{
				return typeof(SystemObjectInspector);
			}

			return null;
		}

		private Type AutomaticReflectedInspector(Type type)
		{
			if (type.HasAttribute<InspectableAttribute>())
			{
				return typeof(AutomaticReflectedInspector);
			}

			return null;
		}

		private Type CustomPropertyDrawerInspector(Type type)
		{
			if (SerializedPropertyProviderProvider.instance.HasDecorator(type))
			{
				return typeof(CustomPropertyDrawerInspector);
			}

			return null;
		}

		public bool HasInspector(Type type)
		{
			return GetDecoratorType(type) != typeof(UnknownInspector);
		}

		static InspectorProvider()
		{
			instance = new InspectorProvider();
		}

		public static InspectorProvider instance { get; private set; }
	}

	public static class XInspectorProvider
	{
		public static Inspector CreateUninitializedInspector(this Accessor accessor)
		{
			return InspectorProvider.instance.GetDecorator(accessor);
		}
		
		public static TInspector CreateUninitializedInspector<TInspector>(this Accessor accessor) where TInspector : Inspector
		{
			return InspectorProvider.instance.GetDecorator<TInspector>(accessor);
		}

		public static Inspector CreateInitializedInspector(this Accessor accessor)
		{
			var inspector = CreateUninitializedInspector(accessor);
			inspector.Initialize();
			return inspector;
		}
		
		public static TInspector CreateInitializedInspector<TInspector>(this Accessor accessor) where TInspector : Inspector
		{
			var inspector = CreateUninitializedInspector<TInspector>(accessor);
			inspector.Initialize();
			return inspector;
		}

		public static bool HasInspector(this Type type)
		{
			return InspectorProvider.instance.HasInspector(type);
		}

		public static bool HasInspector(this Accessor accessor)
		{
			return InspectorProvider.instance.HasInspector(accessor.definedType);
		}
	}
}