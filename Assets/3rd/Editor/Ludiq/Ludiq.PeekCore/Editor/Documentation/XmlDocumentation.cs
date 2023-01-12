using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public static class XmlDocumentation
	{
		static XmlDocumentation()
		{
			documentations = new Dictionary<string, XmlDocumentationTags>();
			documentationsCache = new Dictionary<object, XmlDocumentationTags>();
		}

		private static readonly Dictionary<string, XmlDocumentationTags> documentations;

		private static readonly Dictionary<object, XmlDocumentationTags> documentationsCache;

		private static readonly object @lock = new object();

		public static void Clear()
		{
			lock (@lock)
			{
				documentations.Clear();
				documentationsCache.Clear();
				isLoaded = false;
			}
		}

		public static void Load(Dictionary<string, XmlDocumentationTags> documentations)
		{
			Ensure.That(nameof(documentations)).IsNotNull(documentations);

			Clear();

			lock (@lock)
			{
				foreach (var documentation in documentations)
				{
					if (!XmlDocumentation.documentations.ContainsKey(documentation.Key))
					{
						XmlDocumentation.documentations.Add(documentation.Key, documentation.Value);
					}
				}

				isLoaded = true;

				onLoaded?.Invoke();
			}
		}

		public static bool isLoaded { get; private set; }

		public static event Action onLoaded;

		public static XmlDocumentationTags Documentation(this MemberInfo memberInfo)
		{
			if (memberInfo is Type type)
			{
				return type.Documentation();
			}
			else if (memberInfo is MethodInfo methodInfo)
			{
				return methodInfo.Documentation();
			}
			else if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.Documentation();
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.Documentation();
			}
			else if (memberInfo is ConstructorInfo constructorInfo)
			{
				return constructorInfo.Documentation();
			}

			return null;
		}

		private static XmlDocumentationTags Documentation(this Type type)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}

				if (!documentationsCache.TryGetValue(type, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(type, 'T', null, null);
					documentationsCache.Add(type, documentation);
				}

				return documentation;
			}
		}
		
		private static XmlDocumentationTags Documentation(this FieldInfo fieldInfo)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}

				if (!documentationsCache.TryGetValue(fieldInfo, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(fieldInfo.DeclaringType, 'F', fieldInfo.Name, null);
					documentationsCache.Add(fieldInfo, documentation);
				}

				return documentation;
			}
		}

		private static XmlDocumentationTags Documentation(this PropertyInfo propertyInfo)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}

				if (!documentationsCache.TryGetValue(propertyInfo, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(propertyInfo.DeclaringType, 'P', propertyInfo.Name, null);
					documentationsCache.Add(propertyInfo, documentation);
				}

				return documentation;
			}
		}

		private static XmlDocumentationTags Documentation(this MethodInfo methodInfo)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}
				
				if (!documentationsCache.TryGetValue(methodInfo, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(methodInfo.DeclaringType, 'M', methodInfo.Name, methodInfo.GetParameters());
					documentation?.CompleteWithMethodBase(methodInfo);
					documentationsCache.Add(methodInfo, documentation);
				}

				return documentation;
			}
		}

		private static XmlDocumentationTags Documentation(this ConstructorInfo constructorInfo)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}
				
				if (!documentationsCache.TryGetValue(constructorInfo, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(constructorInfo.DeclaringType, 'M', "#ctor", constructorInfo.GetParameters());
					documentation?.CompleteWithMethodBase(constructorInfo);
					documentationsCache.Add(constructorInfo, documentation);
				}

				return documentation;
			}
		}
		
		public static XmlDocumentationTags Documentation(this Enum @enum)
		{
			lock (@lock)
			{
				if (!isLoaded)
				{
					return null;
				}
				
				if (!documentationsCache.TryGetValue(@enum, out var documentation))
				{
					documentation = GetDocumentationFromNameInherited(@enum.GetType(), 'F', @enum.ToString(), null);
					documentationsCache.Add(@enum, documentation);
				}

				return documentation;
			}
		}
		
		public static string ParameterSummary(this MethodBase methodBase, ParameterInfo parameterInfo)
		{
			return methodBase.Documentation()?.ParameterSummary(parameterInfo);
		}

		public static string Summary(this MemberInfo memberInfo)
		{
			return memberInfo.Documentation()?.summary;
		}

		public static string Summary(this Enum @enum)
		{
			return @enum.Documentation()?.summary;
		}

		private static XmlDocumentationTags GetDocumentationFromNameInherited(Type type, char prefix, string memberName, IEnumerable<ParameterInfo> parameterTypes)
		{
			var documentation = GetDocumentationFromName(type, prefix, memberName, parameterTypes);

			if (documentation != null && documentation.inherit)
			{
				foreach (var implementedType in type.BaseTypeAndInterfaces())
				{
					var implementedDocumentation = GetDocumentationFromNameInherited(implementedType, prefix, memberName, parameterTypes);

					if (implementedDocumentation != null)
					{
						return implementedDocumentation;
					}
				}

				return null;
			}

			return documentation;
		}

		private static XmlDocumentationTags GetDocumentationFromName(Type type, char prefix, string memberName, IEnumerable<ParameterInfo> parameterTypes)
		{
			if (type.IsGenericType)
			{
				type = type.GetGenericTypeDefinition();
			}

			var fullName = $"{prefix}:{type.Namespace}{(type.Namespace != null ? "." : "")}{type.Name.Replace('+', '.')}";

			if (!string.IsNullOrEmpty(memberName))
			{
				fullName += "." + memberName;

				if (parameterTypes != null && parameterTypes.Any())
				{
					fullName += "(" + string.Join(",", parameterTypes.Select(p => p.ParameterType.ToString() + (p.IsOut || p.ParameterType.IsByRef ? "@" : "")).ToArray()) + ")";
				}
			}

			if (!documentations.TryGetValue(fullName, out var documentation))
			{
				return null;
			}

			return documentation;
		}
	}
}