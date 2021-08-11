using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ludiq.OdinSerializer;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public class SerializationPolicy : ISerializationPolicy
	{
		public static SerializationPolicy instance { get; } = new SerializationPolicy();

		public string ID => "Ludiq";

		public bool AllowNonSerializableTypes => true;

		public string GetMemberName(MemberInfo member)
		{
			var serializeAsAttribute = member.GetAttribute<SerializeAsAttribute>();

			if (serializeAsAttribute != null)
			{
				return serializeAsAttribute.Name;
			}

			return member.Name;
		}
		
		public bool ShouldSerializeMember(MemberInfo member)
		{
			var cache = AttributeUtility.GetAttributeCache(member);
			
			if (cache.HasAttribute<DoNotSerializeAttribute>() ||
			    cache.HasAttribute<NonSerializedAttribute>())
			{
				return false;
			}

			if (cache.HasAttribute<SerializeAttribute>() ||
				cache.HasAttribute<SerializeAsAttribute>() ||
			    cache.HasAttribute<SerializeField>() ||
			    cache.HasAttribute<OdinSerializeAttribute>())
			{
				return true;
			}

			if (member is FieldInfo field)
			{
				if (!field.IsPublic)
				{
					return false;
				}

				if (field.IsStatic)
				{
					return false;
				}

				if (cache.HasAttribute<CompilerGeneratedAttribute>(false))
				{
					return false;
				}
				
				if (typeof(Delegate).IsAssignableFrom(field.FieldType))
				{
					return false;
				}

				return true;
			}
			else if (member is PropertyInfo property)
			{
				// Inlining helpers that require the underlying methods to reuse them for performance
				var getMethod = property.GetGetMethod(false); // Property has to be publicly gettable
				var setMethod = property.GetSetMethod(true); // Can be privately settable

				if (getMethod == null || setMethod == null)
				{
					return false;
				}

				if (getMethod.IsStatic || setMethod.IsStatic)
				{
					return false;
				}
				
				if (!getMethod.HasAttribute<CompilerGeneratedAttribute>()) // Has to be an auto-property
				{
					return false;
				}
				
				if (property.IsIndexer())
				{
					return false;
				}
				
				if (typeof(Delegate).IsAssignableFrom(property.PropertyType))
				{
					return false;
				}
				
				return true;
			}

			return false;
		}

		private static bool IsAutoProperty(PropertyInfo property)
		{
			// Odin's implementation is weirdly restrictive and expensive, using FullSerializer's
			return property.CanWrite && property.CanRead && property.GetGetMethod(true).HasAttribute<CompilerGeneratedAttribute>();
		}
	}
}
 