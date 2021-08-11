using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class Codebase
	{
		static Codebase()
		{
			using (ProfilingUtility.SampleBlock("Codebase initialization"))
			{
				_assemblies = new List<Assembly>();
				_runtimeAssemblies = new List<Assembly>();
				_editorAssemblies = new List<Assembly>();
				_ludiqAssemblies = new List<Assembly>();
				_ludiqRuntimeAssemblies = new List<Assembly>();
				_ludiqEditorAssemblies = new List<Assembly>();

				_types = new List<Type>();
				_runtimeTypes = new List<Type>();
				_editorTypes = new List<Type>();
				_ludiqTypes = new List<Type>();
				_ludiqRuntimeTypes = new List<Type>();
				_ludiqEditorTypes = new List<Type>();

				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					try
					{
						if (assembly.IsDynamic)
						{
							continue;
						}

						var isRuntimeAssembly = IsRuntimeAssembly(assembly);
						var isEditorAssembly = IsEditorDependentAssembly(assembly);
						var isLudiqAssembly = IsLudiqRuntimeDependentAssembly(assembly) || IsLudiqEditorDependentAssembly(assembly);
						var isLudiqEditorAssembly = IsLudiqEditorDependentAssembly(assembly);
						var isLudiqRuntimeAssembly = IsLudiqRuntimeDependentAssembly(assembly) && !IsLudiqEditorDependentAssembly(assembly);

						_assemblies.Add(assembly);

						if (isRuntimeAssembly)
						{
							_runtimeAssemblies.Add(assembly);
						}

						if (isEditorAssembly)
						{
							_editorAssemblies.Add(assembly);
						}

						if (isLudiqAssembly)
						{
							_ludiqAssemblies.Add(assembly);
						}

						if (isLudiqEditorAssembly)
						{
							_ludiqEditorAssemblies.Add(assembly);
						}

						if (isLudiqRuntimeAssembly)
						{
							_ludiqRuntimeAssemblies.Add(assembly);
						}

						foreach (var type in assembly.GetTypesSafely())
						{
							_types.Add(type);

							// RuntimeCodebase.PrewarmTypeDeserialization(type);

							if (isRuntimeAssembly)
							{
								_runtimeTypes.Add(type);
							}

							if (isEditorAssembly)
							{
								_editorTypes.Add(type);
							}

							if (isLudiqAssembly)
							{
								_ludiqTypes.Add(type);
							}

							if (isLudiqEditorAssembly)
							{
								_ludiqEditorTypes.Add(type);
							}

							if (isLudiqRuntimeAssembly)
							{
								_ludiqRuntimeTypes.Add(type);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"Failed to analyze assembly '{assembly}':\n{ex}");
					}
				}

				assemblies = _assemblies.AsReadOnly();
				runtimeAssemblies = _runtimeAssemblies.AsReadOnly();
				editorAssemblies = _editorAssemblies.AsReadOnly();
				ludiqAssemblies = _ludiqAssemblies.AsReadOnly();
				ludiqRuntimeAssemblies = _ludiqRuntimeAssemblies.AsReadOnly();
				ludiqEditorAssemblies = _ludiqEditorAssemblies.AsReadOnly();

				types = _types.AsReadOnly();
				runtimeTypes = _runtimeTypes.AsReadOnly();
				editorTypes = _editorTypes.AsReadOnly();
				ludiqTypes = _ludiqTypes.AsReadOnly();
				ludiqRuntimeTypes = _ludiqRuntimeTypes.AsReadOnly();
				ludiqEditorTypes = _ludiqEditorTypes.AsReadOnly();
			}
		}

		private static readonly List<Assembly> _assemblies;
		private static readonly List<Assembly> _runtimeAssemblies;
		private static readonly List<Assembly> _editorAssemblies;
		private static readonly List<Assembly> _ludiqAssemblies;
		private static readonly List<Assembly> _ludiqRuntimeAssemblies;
		private static readonly List<Assembly> _ludiqEditorAssemblies;
		private static readonly List<Type> _types;
		private static readonly List<Type> _runtimeTypes;
		private static readonly List<Type> _editorTypes;
		private static readonly List<Type> _ludiqTypes;
		private static readonly List<Type> _ludiqRuntimeTypes;
		private static readonly List<Type> _ludiqEditorTypes;
		
		#region Serialization
		
		public static string SerializeType(Type type)
		{
			return RuntimeCodebase.SerializeType(type);
		}

		public static bool TryDeserializeType(string typeName, out Type type)
		{
			return RuntimeCodebase.TryDeserializeType(typeName, out type);
		}

		public static Type DeserializeType(string typeName)
		{
			return RuntimeCodebase.DeserializeType(typeName);
		}

		public static TypeData SerializeTypeData(Type type)
		{
			return RuntimeCodebase.SerializeTypeData(type);
		}

		public static Type DeserializeTypeData(TypeData data)
		{
			return RuntimeCodebase.DeserializeTypeData(data);
		}
		
		#endregion
		
		// NETUP: IReadOnlyCollection

		public static ReadOnlyCollection<Assembly> assemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> runtimeAssemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> editorAssemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> ludiqAssemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> ludiqRuntimeAssemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> ludiqEditorAssemblies { get; private set; }

		public static ReadOnlyCollection<Assembly> settingsAssemblies { get; private set; }

		public static ReadOnlyCollection<Type> types { get; private set; }

		public static ReadOnlyCollection<Type> runtimeTypes { get; private set; }

		public static ReadOnlyCollection<Type> editorTypes { get; private set; }

		public static ReadOnlyCollection<Type> ludiqTypes { get; private set; }

		public static ReadOnlyCollection<Type> ludiqRuntimeTypes { get; private set; }

		public static ReadOnlyCollection<Type> ludiqEditorTypes { get; private set; }

		private static readonly Dictionary<Assembly, AssemblyName[]> assemblyReferences = new Dictionary<Assembly, AssemblyName[]>();

		private static AssemblyName[] GetReferencedAssembliesCached(this Assembly assembly)
		{
			if (!assemblyReferences.TryGetValue(assembly, out var referencedAssemblies))
			{
				referencedAssemblies = assembly.GetReferencedAssemblies();
				assemblyReferences.Add(assembly, referencedAssemblies);
			}

			return referencedAssemblies;
		}
		
		private static bool IsEditorAssembly(AssemblyName assemblyName)
		{
			var name = assemblyName.Name;

			return
				name == "Assembly-CSharp-Editor" ||
				name == "Assembly-CSharp-Editor-firstpass" ||
				name == "UnityEditor";
		}

		private static bool IsUserAssembly(AssemblyName assemblyName)
		{
			var name = assemblyName.Name;
			
			return
				name == "Assembly-CSharp" ||
				name == "Assembly-CSharp-firstpass";
		}

		private static bool IsUserAssembly(Assembly assembly)
		{
			return IsUserAssembly(assembly.GetName());
		}

		private static bool IsEditorAssembly(Assembly assembly)
		{
			if (Attribute.IsDefined(assembly, typeof(AssemblyIsEditorAssembly)))
			{
				return true;
			}

			return IsEditorAssembly(assembly.GetName());
		}

		private static bool IsRuntimeAssembly(Assembly assembly)
		{
			// User assemblies refer to the editor when they include
			// a using UnityEditor / #if UNITY_EDITOR, but they should still
			// be considered runtime.
			return IsUserAssembly(assembly) || !IsEditorDependentAssembly(assembly);
		}

		private static bool IsEditorDependentAssembly(Assembly assembly)
		{
			if (IsEditorAssembly(assembly))
			{
				return true;
			}

			foreach (var dependency in assembly.GetReferencedAssembliesCached())
			{
				if (IsEditorAssembly(dependency))
				{
					return true;
				}
			}

			return false;
		}
		
		private static bool IsLudiqRuntimeDependentAssembly(Assembly assembly)
		{
			if (assembly.GetName().Name == "Ludiq.PeekCore.Runtime")
			{
				return true;
			}

			foreach (var dependency in assembly.GetReferencedAssembliesCached())
			{
				if (dependency.Name == "Ludiq.PeekCore.Runtime")
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsLudiqEditorDependentAssembly(Assembly assembly)
		{
			if (assembly.GetName().Name == "Ludiq.PeekCore.Editor")
			{
				return true;
			}

			foreach (var dependency in assembly.GetReferencedAssembliesCached())
			{
				if (dependency.Name == "Ludiq.PeekCore.Editor")
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsEditorType(Type type)
		{
			var rootNamespace = type.RootNamespace();

			return IsEditorAssembly(type.Assembly) ||
				   rootNamespace == "UnityEditor" ||
				   rootNamespace == "UnityEditorInternal";
		}

		public static bool IsInternalType(Type type)
		{
			var rootNamespace = type.RootNamespace();

			return rootNamespace == "UnityEngineInternal" ||
				   rootNamespace == "UnityEditorInternal";
		}

		public static bool IsRuntimeType(Type type)
		{
			return !IsEditorType(type) && !IsInternalType(type);
		}

		private static string RootNamespace(this Type type)
		{
			return type.Namespace?.PartBefore('.');
		}

		public static CodebaseSubset Subset(IEnumerable<Type> types, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
		{
			return new CodebaseSubset(types, memberFilter, memberTypeFilter);
		}

		public static CodebaseSubset Subset(IEnumerable<Type> typeSet, TypeFilter typeFilter, MemberFilter memberFilter, TypeFilter memberTypeFilter = null)
		{
			return new CodebaseSubset(typeSet, typeFilter, memberFilter, memberTypeFilter);
		}

		#region Assembly Attributes
		
		public static IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
		{
			// Faster version than RuntimeCodebase:
			// Usually no need to check in other assemblies as our custom attributes are defined in Ludiq
			return RuntimeCodebase.GetAssemblyAttributes(attributeType, ludiqAssemblies); 
		}

		public static IEnumerable<TAttribute> GetAssemblyAttributes<TAttribute>() where TAttribute : Attribute
		{
			return GetAssemblyAttributes(typeof(TAttribute)).Cast<TAttribute>();
		}

		public static IEnumerable<ITypeRegistrationAttribute> GetTypeRegistrations(Type attributeType)
		{
			return GetAssemblyAttributes(attributeType).OfType<ITypeRegistrationAttribute>();
		}

		public static IEnumerable<TAttribute> GetTypeRegistrations<TAttribute>() where TAttribute : ITypeRegistrationAttribute
		{
			return GetTypeRegistrations(typeof(TAttribute)).Cast<TAttribute>();
		}

		public static IEnumerable<Type> GetRegisteredTypes(Type attributeType)
		{
			return GetTypeRegistrations(attributeType).Select(a => a.type);
		}

		public static IEnumerable<Type> GetRegisteredTypes<TAttribute>() where TAttribute : ITypeRegistrationAttribute
		{
			return GetTypeRegistrations(typeof(TAttribute)).Select(a => a.type);
		}

		#endregion
	}
}