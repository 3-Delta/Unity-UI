using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class RuntimeCodebase
	{
		private static readonly object @lock = new object();

		private static readonly List<Type> _types = new List<Type>();

		public static IEnumerable<Type> types => _types;

		private static readonly List<Assembly> _assemblies = new List<Assembly>();

		public static IEnumerable<Assembly> assemblies => _assemblies;

		private static readonly Dictionary<string, Type> typeSerializations = new Dictionary<string, Type>();

		private static Dictionary<string, string> _renamedTypes = null;

		private static Dictionary<string, string> _renamedNamespaces = null;

		private static readonly Dictionary<Type, Dictionary<string, string>> _renamedMembers = new Dictionary<Type, Dictionary<string, string>>();

		static RuntimeCodebase()
		{
			lock (@lock)
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						if (assembly.IsDynamic)
						{
							continue;
						}

						_assemblies.Add(assembly);

						foreach (var assemblyType in assembly.GetTypesSafely())
						{
							_types.Add(assemblyType);
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"Failed to analyze assembly '{assembly}':\n{ex}");
					}
				}
			}
		}



		#region Assembly Attributes

		public static IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
		{
			return GetAssemblyAttributes(attributeType, assemblies);
		}

		public static IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType, IEnumerable<Assembly> assemblies)
		{
			Ensure.That(nameof(attributeType)).IsNotNull(attributeType);
			Ensure.That(nameof(assemblies)).IsNotNull(assemblies);

			foreach (var assembly in assemblies)
			{
				foreach (var attribute in assembly.GetAttributes(attributeType))
				{
					if (attributeType.IsInstanceOfType(attribute))
					{
						yield return attribute;
					}
				}
			}
		}

		public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>(IEnumerable<Assembly> assemblies) where TAttribute : Attribute
		{
			return GetAssemblyAttributes(typeof(TAttribute), assemblies).Cast<TAttribute>();
		}

		public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>() where TAttribute : Attribute
		{
			return GetAssemblyAttributes(typeof(TAttribute)).Cast<TAttribute>();
		}

		#endregion



		#region Serialization

		public static void PrewarmTypeDeserialization(Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			var serialization = SerializeType(type);

			// Some are duplicates which this will overwrite, but almost always compiler generated stuff.
			// Safe to ignore, and anyway what would we even do to deserialize them properly?
			typeSerializations[serialization] = type;
		}

		public static string SerializeType(Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			// If this type is not a fully-closed generic type, then discard all type parameters.
			// This is because List<List<T>> can't be serialized/deserialized, but List<T> can be.
			// We lose some information doing this, but we don't can't serialize partially-constructed types without our own serialization method.
			if (type.ContainsGenericParameters)
			{
				type = type.GetGenericTypeDefinition();
			}

			return TypeName.SimplifyFast(type.AssemblyQualifiedName);
		}

		public static bool TryDeserializeType(string typeName, out Type type)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				type = null;
				return false;
			}

			lock (@lock)
			{
				if (!TryCachedTypeLookup(typeName, out type))
				{
					if (!TrySystemTypeLookup(typeName, out type))
					{
						if (!TryRenamedTypeLookup(typeName, out type))
						{
							return false;
						}
					}

					typeSerializations.Add(typeName, type);
				}

				return true;
			}
		}

		public static Type DeserializeType(string typeName)
		{
			if (!TryDeserializeType(typeName, out var type))
			{
				throw new SerializationException($"Unable to find type: '{typeName ?? "(null)"}'.");
			}

			return type;
		}

		public static bool ContainsTypeMap(string typeName)
		{
			return typeSerializations.ContainsKey(typeName);
		}

		private static bool TryCachedTypeLookup(string typeName, out Type type)
		{
			return typeSerializations.TryGetValue(typeName, out type);
		}

		private static bool TrySystemTypeLookup(string typeName, out Type type)
		{
			// Try for an assembly-qualified match first
			type = Type.GetType(typeName);

			if (type != null)
			{
				return true;
			}

			// Fallback to looping over all assemblies
			foreach (var assembly in _assemblies)
			{
				type = assembly.GetType(typeName);

				if (type != null)
				{
					return true;
				}
			}

			type = null;
			return false;
		}

		private static bool TryRenamedTypeLookup(string previousTypeName, out Type type)
		{
			string newTypeName;

			// Try for an exact match in our renamed types dictionary. 
			// That should work for every non-generic type.
			// If we can't get an exact match, we'll try parsing the previous type name,
			// replacing all the renamed types we can find, then reconstructing it.
			if (!renamedTypes.TryGetValue(previousTypeName, out newTypeName))
			{
				var parsedTypeName = TypeName.Parse(previousTypeName);

				foreach (var renamedType in renamedTypes)
				{
					parsedTypeName.Replace(renamedType.Key, renamedType.Value);
				}

				foreach (var renamedNamespace in renamedNamespaces)
				{
					parsedTypeName.ReplaceNamespace(renamedNamespace.Key, renamedNamespace.Value);
				}

				newTypeName = parsedTypeName.ToLooseString();
			}

			// Run the system lookup
			if (TrySystemTypeLookup(newTypeName, out type))
			{
				return true;
			}

			type = null;
			return false;
		}

		public static TypeData SerializeTypeData(Type type)
		{
			var data = new TypeData();

			if (type == null)
			{
				data.code = TypeCode.Empty;
			}
			else
			{
				// GetTypeCode will return the underlying type for enums,
				// which means we'd actually lose type precision -- make sure
				// to treat enums as objects in that case.
				data.code = type.IsEnum ? TypeCode.Object : Type.GetTypeCode(type);

				if (data.code == TypeCode.Object)
				{
					data.name = SerializeType(type);
				}
			}

			return data;
		}

		public static TypeData SerializeTypeData(Type type, string typeNameFallback)
		{
			if (type != null)
			{
				return SerializeTypeData(type);
			}
			else if (typeNameFallback != null)
			{
				return new TypeData
				{
					code = TypeCode.Object,
					name = typeNameFallback
				};
			}
			else
			{
				return new TypeData {code = TypeCode.Empty};
			}
		}

		public static Type DeserializeTypeData(TypeData data)
		{
			if (data.code == TypeCode.Object)
			{
				return DeserializeType(data.name);
			}
			else
			{
				return data.code.ToType();
			}
		}

		#endregion



		#region Renaming

		public static Dictionary<string, string> renamedNamespaces
		{
			get
			{
				if (_renamedNamespaces == null)
				{
					_renamedNamespaces = FetchRenamedNamespaces();
				}

				return _renamedNamespaces;
			}
		}

		public static Dictionary<string, string> renamedTypes
		{
			get
			{
				if (_renamedTypes == null)
				{
					_renamedTypes = FetchRenamedTypes();
				}

				return _renamedTypes;
			}
		}

		public static Dictionary<string, string> RenamedMembers(Type type)
		{
			Dictionary<string, string> renamedMembers;

			if (!_renamedMembers.TryGetValue(type, out renamedMembers))
			{
				renamedMembers = FetchRenamedMembers(type);
				_renamedMembers.Add(type, renamedMembers);
			}

			return renamedMembers;
		}

		private static Dictionary<string, string> FetchRenamedMembers(Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			var renamedMembers = new Dictionary<string, string>();

			var members = type.GetExtendedMembers(Member.SupportedBindingFlags);

			foreach (var member in members)
			{
				IEnumerable<RenamedFromAttribute> renamedFromAttributes;

				try
				{
					renamedFromAttributes = Attribute.GetCustomAttributes(member, typeof(RenamedFromAttribute), false).Cast<RenamedFromAttribute>();
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"Failed to fetch RenamedFrom attributes for member '{member}':\n{ex}");
					continue;
				}

				var newMemberName = member.Name;

				foreach (var renamedFromAttribute in renamedFromAttributes)
				{
					var previousMemberName = renamedFromAttribute.previousName;

					if (renamedMembers.ContainsKey(previousMemberName))
					{
						Debug.LogWarning($"Multiple members on '{type}' indicate having been renamed from '{previousMemberName}'.\nIgnoring renamed attributes for '{member}'.");

						continue;
					}

					renamedMembers.Add(previousMemberName, newMemberName);
				}
			}

			return renamedMembers;
		}

		private static Dictionary<string, string> FetchRenamedNamespaces()
		{
			var renamedNamespaces = new Dictionary<string, string>();

			foreach (var renamedNamespaceAttribute in GetAssemblyAttributes<RenamedNamespaceAttribute>())
			{
				var previousNamespaceName = renamedNamespaceAttribute.previousName;
				var newNamespaceName = renamedNamespaceAttribute.newName;

				if (renamedNamespaces.ContainsKey(previousNamespaceName))
				{
					Debug.LogWarning($"Multiple new names have been provided for namespace '{previousNamespaceName}'.\nIgnoring new name '{newNamespaceName}'.");

					continue;
				}

				renamedNamespaces.Add(previousNamespaceName, newNamespaceName);
			}

			return renamedNamespaces;
		}

		private static Dictionary<string, string> FetchRenamedTypes()
		{
			var renamedTypes = new Dictionary<string, string>();

			foreach (var assembly in assemblies)
			foreach (var type in assembly.GetTypesSafely())
			{
				IEnumerable<RenamedFromAttribute> renamedFromAttributes;

				try
				{
					renamedFromAttributes = Attribute.GetCustomAttributes(type, typeof(RenamedFromAttribute), false).Cast<RenamedFromAttribute>();
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"Failed to fetch RenamedFrom attributes for type '{type}':\n{ex}");
					continue;
				}

				var newTypeName = type.FullName;

				foreach (var renamedFromAttribute in renamedFromAttributes)
				{
					var previousTypeName = renamedFromAttribute.previousName;

					if (renamedTypes.ContainsKey(previousTypeName))
					{
						Debug.LogWarning($"Multiple types indicate having been renamed from '{previousTypeName}'.\nIgnoring renamed attributes for '{type}'.");

						continue;
					}

					renamedTypes.Add(previousTypeName, newTypeName);
				}
			}

			return renamedTypes;
		}

		#endregion
	}
}