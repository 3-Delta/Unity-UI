using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class MemberUtility
	{
		static MemberUtility() { }



		#region Operators

		private static Dictionary<Type, HashSet<MethodInfo>> operators;

		private static readonly object operatorsLock = new object();

		private static void FetchAllOperators()
		{
			operators = new Dictionary<Type, HashSet<MethodInfo>>();

			foreach (var type in RuntimeCodebase.types)
			{
				foreach (var method in type.GetMethods())
				{
					if (method.IsOperator())
					{
						if (!operators.TryGetValue(type, out var typeOperators))
						{
							typeOperators = new HashSet<MethodInfo>();
							operators.Add(type, typeOperators);
						}

						typeOperators.Add(method);

						var parameters = method.GetParameters();

						if (parameters.Length == 2)
						{
							var leftType = parameters[0].ParameterType;
							var rightType = parameters[1].ParameterType;

							if (leftType != type)
							{
								if (!operators.TryGetValue(leftType, out var leftTypeOperators))
								{
									leftTypeOperators = new HashSet<MethodInfo>();
									operators.Add(leftType, leftTypeOperators);
								}

								leftTypeOperators.Add(method);
							}
							else if (rightType != type)
							{
								if (!operators.TryGetValue(rightType, out var rightTypeOperators))
								{
									rightTypeOperators = new HashSet<MethodInfo>();
									operators.Add(rightType, rightTypeOperators);
								}

								rightTypeOperators.Add(method);
							}
						}
					}
				}
			}
		}

		public static bool IsOperator(this MethodInfo method)
		{
			return method.IsSpecialName && OperatorUtility.operatorSymbols.ContainsKey(method.Name);
		}

		public static bool IsOperator(this MemberInfo memberInfo)
		{
			return memberInfo is MethodInfo methodInfo && methodInfo.IsOperator();
		}

		public static OperatorCategory GetOperatorCategory(this MethodInfo method)
		{
			if (method.IsSpecialName)
			{
				if (OperatorUtility.TryGetUnaryByMethodName(method.Name, out var unaryOperator))
				{
					return unaryOperator.GetOperatorCategory();
				}

				if (OperatorUtility.TryGetBinaryByMethodName(method.Name, out var binaryOperator))
				{
					return binaryOperator.GetOperatorCategory();
				}
			}

			return OperatorCategory.None;
		}

		public static OperatorCategory GetOperatorCategory(this MemberInfo memberInfo)
		{
			if (memberInfo is MethodInfo methodInfo)
			{
				return methodInfo.GetOperatorCategory();
			}

			return OperatorCategory.None;
		}

		public static bool IsUserDefinedConversion(this MethodInfo method)
		{
			return method.IsSpecialName && (method.Name == "op_Implicit" || method.Name == "op_Explicit");
		}

		public static IEnumerable<MethodInfo> GetOperators(this Type type)
		{
			lock (operatorsLock)
			{
				if (operators == null)
				{
					FetchAllOperators();
				}
			}

			if (!operators.TryGetValue(type, out var typeOperators))
			{
				return Enumerable.Empty<MethodInfo>();
			}

			return typeOperators;
		}

		#endregion



		#region Extension Methods

		private static Dictionary<Type, HashSet<MethodInfo>> definedExtensionMethods = null;

		private static readonly Dictionary<Type, HashSet<MethodInfo>> resolvedExtensionMethods = new Dictionary<Type, HashSet<MethodInfo>>();

		private static readonly HashSet<MethodInfo> genericExtensionMethods = new HashSet<MethodInfo>();

		private static void FetchDefinedExtensionMethods()
		{
			var extensionMethodDeclarers = RuntimeCodebase.types.Where(type => type.IsStatic() && !type.IsNested && !type.IsGenericType);

			definedExtensionMethods = new Dictionary<Type, HashSet<MethodInfo>>();

			foreach (var extensionMethodDeclarer in extensionMethodDeclarers)
			{
				var extensionMethods = extensionMethodDeclarer.GetMethods().Where(m => m.IsExtension());

				foreach (var extensionMethod in extensionMethods)
				{
					var thisParameterType = extensionMethod.GetThisParameter(false).ParameterType;

					if (!definedExtensionMethods.TryGetValue(thisParameterType, out var extensionMethodsCache))
					{
						extensionMethodsCache = new HashSet<MethodInfo>();
						definedExtensionMethods.Add(thisParameterType, extensionMethodsCache);
					}

					extensionMethodsCache.Add(extensionMethod);
				}
			}
		}

		/// <remarks>This may return an open-constructed method as well.</remarks>
		public static MethodInfo MakeGenericMethodVia(this MethodInfo openConstructedMethod, params Type[] closedConstructedParameterTypes)
		{
			using (ProfilingUtility.SampleBlock(nameof(MakeGenericMethodVia)))
			{
				Ensure.That(nameof(openConstructedMethod)).IsNotNull(openConstructedMethod);
				Ensure.That(nameof(closedConstructedParameterTypes)).IsNotNull(closedConstructedParameterTypes);

				if (!openConstructedMethod.ContainsGenericParameters)
				{
					// The method contains no generic parameters,
					// it is by definition already resolved.
					return openConstructedMethod;
				}

				var openConstructedParameterTypes = openConstructedMethod.GetParameters().Select(p => p.ParameterType).ToArray();

				if (openConstructedParameterTypes.Length != closedConstructedParameterTypes.Length)
				{
					throw new ArgumentOutOfRangeException(nameof(closedConstructedParameterTypes));
				}

				var resolvedGenericParameters = new Dictionary<Type, Type>();

				for (var i = 0; i < openConstructedParameterTypes.Length; i++)
				{
					// Resolve each open-constructed parameter type via the equivalent
					// closed-constructed parameter type.

					var openConstructedParameterType = openConstructedParameterTypes[i];
					var closedConstructedParameterType = closedConstructedParameterTypes[i];

					openConstructedParameterType.MakeGenericTypeVia(closedConstructedParameterType, resolvedGenericParameters);
				}

				// Construct the final closed-constructed method from the resolved arguments

				var openConstructedGenericArguments = openConstructedMethod.GetGenericArguments();
				var closedConstructedGenericArguments = openConstructedGenericArguments.Select(openConstructedGenericArgument =>
				{
					// If the generic argument has been successfully resolved, use it;
					// otherwise, leave the open-constructed argument in place.

					if (resolvedGenericParameters.ContainsKey(openConstructedGenericArgument))
					{
						return resolvedGenericParameters[openConstructedGenericArgument];
					}
					else
					{
						return openConstructedGenericArgument;
					}
				}).ToArray();

				return openConstructedMethod.MakeGenericMethod(closedConstructedGenericArguments);
			}
		}

		public static bool IsGenericExtension(this MethodInfo methodInfo)
		{
			return genericExtensionMethods.Contains(methodInfo);
		}

		public static IEnumerable<MethodInfo> GetExtensionMethods(this Type thisArgumentType)
		{
			lock (resolvedExtensionMethods)
			{
				if (!MemberUtility.resolvedExtensionMethods.TryGetValue(thisArgumentType, out var resolvedExtensionMethods))
				{
					resolvedExtensionMethods = new HashSet<MethodInfo>(ResolveExtensionMethods(thisArgumentType));
					MemberUtility.resolvedExtensionMethods.Add(thisArgumentType, resolvedExtensionMethods);
				}

				return resolvedExtensionMethods;
			}
		}

		public const bool supportGenericExtensionMethods = false;

		private static IEnumerable<MethodInfo> ResolveExtensionMethods(this Type thisArgumentType)
		{
			if (definedExtensionMethods == null)
			{
				FetchDefinedExtensionMethods();
			}

			foreach (var kvp in definedExtensionMethods)
			{
				var definedThisParameterType = kvp.Key;
				var definedMethods = kvp.Value;

				var exactThis = false;
				var closeConstructingThis = false;
				var inheritedThis = false;

				if (definedThisParameterType == thisArgumentType)
				{
					exactThis = true;
				}
				else if (supportGenericExtensionMethods && definedThisParameterType.CanMakeGenericTypeVia(thisArgumentType))
				{
					closeConstructingThis = true;
				}
				else if (definedThisParameterType.IsAssignableFrom(thisArgumentType))
				{
					inheritedThis = true;
				}

				var compatibleThis = exactThis || closeConstructingThis || inheritedThis;

				if (!compatibleThis)
				{
					continue;
				}

				foreach (var definedMethod in definedMethods)
				{
					if (closeConstructingThis && definedMethod.ContainsGenericParameters)
					{
						var closedConstructedParameterTypes = thisArgumentType.Yield().Concat(definedMethod.GetParametersWithoutThis().Select(p => p.ParameterType));

						var closedConstructedMethod = definedMethod.MakeGenericMethodVia(closedConstructedParameterTypes.ToArray());

						genericExtensionMethods.Add(closedConstructedMethod);

						yield return closedConstructedMethod;
					}
					else
					{
						yield return definedMethod;
					}
				}
			}

			yield break;
		}

		public static bool CanDefineExtensionMethods(this Type type)
		{
			return type.IsStatic() && type.IsPublic;
		}

		public static bool IsExtension(this MethodInfo methodInfo)
		{
			// Fetching attributes is really slow, so we avoid it by checking the type if possible.
			if (!methodInfo.DeclaringType.CanDefineExtensionMethods())
			{
				return false;
			}

			return methodInfo.HasAttribute<ExtensionAttribute>(false);
		}

		public static bool IsExtensionMethod(this MemberInfo memberInfo)
		{
			return memberInfo is MethodInfo methodInfo && methodInfo.IsExtension();
		}

		#endregion



		public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType)
		{
			return Delegate.CreateDelegate(delegateType, methodInfo);
		}

		public static bool IsAccessor(this MemberInfo memberInfo)
		{
			return memberInfo is FieldInfo || memberInfo is PropertyInfo;
		}

		public static MethodInfo GetInvokeMethod(this Type delegateType)
		{
			if (!delegateType.IsDelegate())
			{
				throw new ArgumentException("Type must be a delegate.", nameof(delegateType));
			}

			return delegateType.GetMethod("Invoke");
		}

		public static bool IsDelegateInvokeMethod(this MethodInfo methodInfo)
		{
			return methodInfo.Name == "Invoke" && methodInfo.DeclaringType.IsDelegate();
		}

		public static Type GetAccessorType(this MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.FieldType;
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.PropertyType;
			}
			else
			{
				return null;
			}
		}

		public static bool IsPubliclyGettable(this MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.IsPublic;
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.CanRead && propertyInfo.GetGetMethod(false) != null;
			}
			else if (memberInfo is MethodInfo methodInfo)
			{
				return methodInfo.IsPublic;
			}
			else if (memberInfo is ConstructorInfo constructorInfo)
			{
				return constructorInfo.IsPublic;
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static Type DeclaringOrExtendedType(this MemberInfo memberInfo)
		{
			if (memberInfo is MethodInfo methodInfo && methodInfo.IsExtension())
			{
				return methodInfo.GetParameters()[0].ParameterType;
			}
			else
			{
				return memberInfo.DeclaringType;
			}
		}

		public static MethodInfo GetIndicativeMethod(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetGetMethod(true) ?? propertyInfo.GetSetMethod(true);
		}

		public static MethodInfo GetIndicativeMethod(this EventInfo eventInfo)
		{
			return eventInfo.GetAddMethod(true) ?? eventInfo.GetRemoveMethod(true);
		}

		public static bool IsStatic(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetIndicativeMethod().IsStatic;
		}

		public static bool IsStatic(this EventInfo eventInfo)
		{
			return eventInfo.GetIndicativeMethod().IsStatic;
		}

		public static bool IsPublic(this EventInfo eventInfo)
		{
			return eventInfo.GetIndicativeMethod().IsPublic;
		}

		public static bool IsVirtual(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetIndicativeMethod().IsVirtual;
		}

		public static bool IsVirtual(this EventInfo eventInfo)
		{
			return eventInfo.GetIndicativeMethod().IsVirtual;
		}

		public static bool IsStatic(this MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.IsStatic;
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.IsStatic();
			}
			else if (memberInfo is MethodBase methodBase)
			{
				return methodBase.IsStatic;
			}
			else if (memberInfo is EventInfo eventInfo)
			{
				return eventInfo.IsStatic();
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static bool IsVirtual(this MemberInfo memberInfo)
		{
			if (memberInfo is FieldInfo fieldInfo)
			{
				return false;
			}
			else if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.IsVirtual();
			}
			else if (memberInfo is MethodBase methodBase)
			{
				return methodBase.IsVirtual;
			}
			else if (memberInfo is EventInfo eventInfo)
			{
				return eventInfo.IsVirtual();
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public static bool IsIndexer(this PropertyInfo propertyInfo)
		{
			return propertyInfo.GetIndexParameters().Length > 0;
		}

		public static ParameterInfo GetThisParameter(this MethodInfo methodInfo, bool safe = true)
		{
			if (safe && !methodInfo.IsExtension())
			{
				throw new InvalidOperationException("Cannot get this parameter of a non-extension method.");
			}

			return methodInfo.GetParameters()[0];
		}

		public static IEnumerable<ParameterInfo> GetParametersWithoutThis(this MethodBase methodBase)
		{
			return methodBase.GetParameters().Skip(methodBase.IsExtensionMethod() ? 1 : 0);
		}

		public static Type UnderlyingParameterType(this ParameterInfo parameterInfo)
		{
			if (parameterInfo.ParameterType.IsByRef)
			{
				return parameterInfo.ParameterType.GetElementType();
			}
			else
			{
				return parameterInfo.ParameterType;
			}
		}

		// https://stackoverflow.com/questions/9977530/
		// https://stackoverflow.com/questions/16186694
		public static bool HasDefaultValue(this ParameterInfo parameterInfo)
		{
			return (parameterInfo.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;
		}

		public static object DefaultValue(this ParameterInfo parameterInfo)
		{
			if (parameterInfo.HasDefaultValue())
			{
				var defaultValue = parameterInfo.DefaultValue;

				// https://stackoverflow.com/questions/45393580
				if (defaultValue == null && parameterInfo.ParameterType.IsValueType)
				{
					defaultValue = parameterInfo.ParameterType.Default();
				}

				return defaultValue;
			}
			else
			{
				return parameterInfo.UnderlyingParameterType().Default();
			}
		}

		public static object PseudoDefaultValue(this ParameterInfo parameterInfo)
		{
			if (parameterInfo.HasDefaultValue())
			{
				var defaultValue = parameterInfo.DefaultValue;

				// https://stackoverflow.com/questions/45393580
				if (defaultValue == null && parameterInfo.ParameterType.IsValueType)
				{
					defaultValue = parameterInfo.ParameterType.PseudoDefault();
				}

				return defaultValue;
			}
			else
			{
				return parameterInfo.UnderlyingParameterType().PseudoDefault();
			}
		}

		public static bool AllowsNull(this ParameterInfo parameterInfo)
		{
			var type = parameterInfo.ParameterType;

			return (type.IsReferenceType() && parameterInfo.HasAttribute<AllowsNullAttribute>()) || Nullable.GetUnderlyingType(type) != null;
		}

		public static bool CanWrite(this FieldInfo fieldInfo)
		{
			return !(fieldInfo.IsInitOnly || fieldInfo.IsLiteral);
		}

		public static Member ToMember(this MemberInfo memberInfo)
		{
			return memberInfo.ToMember(memberInfo.DeclaringType);
		}

		public static Member ToMember(this MemberInfo memberInfo, Type targetType)
		{
			if (memberInfo is FieldInfo fieldInfo)
			{
				return fieldInfo.ToMember(targetType);
			}

			if (memberInfo is PropertyInfo propertyInfo)
			{
				return propertyInfo.ToMember(targetType);
			}

			if (memberInfo is MethodInfo methodInfo)
			{
				return methodInfo.ToMember(targetType);
			}

			if (memberInfo is ConstructorInfo constructorInfo)
			{
				return constructorInfo.ToMember(targetType);
			}

			if (memberInfo is EventInfo eventInfo)
			{
				return eventInfo.ToMember(targetType);
			}

			throw new InvalidOperationException();
		}

		public static Member ToMember(this FieldInfo fieldInfo, Type targetType)
		{
			return new Member(targetType, fieldInfo);
		}

		public static Member ToMember(this PropertyInfo propertyInfo, Type targetType)
		{
			return new Member(targetType, propertyInfo);
		}

		public static Member ToMember(this MethodInfo methodInfo, Type targetType)
		{
			return new Member(targetType, methodInfo);
		}

		public static Member ToMember(this ConstructorInfo constructorInfo, Type targetType)
		{
			return new Member(targetType, constructorInfo);
		}

		public static Member ToMember(this EventInfo eventInfo, Type targetType)
		{
			return new Member(targetType, eventInfo);
		}

		public static ConstructorInfo GetConstructorAccepting(this Type type, Type[] paramTypes, bool nonPublic)
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

			if (nonPublic)
			{
				bindingFlags |= BindingFlags.NonPublic;
			}

			return type
				.GetConstructors(bindingFlags)
				.FirstOrDefault(constructor =>
				{
					var parameters = constructor.GetParameters();

					if (parameters.Length != paramTypes.Length)
					{
						return false;
					}

					for (var i = 0; i < parameters.Length; i++)
					{
						if (paramTypes[i] == null)
						{
							if (!parameters[i].ParameterType.IsNullable())
							{
								return false;
							}
						}
						else
						{
							if (!parameters[i].ParameterType.IsAssignableFrom(paramTypes[i]))
							{
								return false;
							}
						}
					}

					return true;
				});
		}

		public static ConstructorInfo GetConstructorAccepting(this Type type, params Type[] paramTypes)
		{
			return GetConstructorAccepting(type, paramTypes, true);
		}

		public static ConstructorInfo GetPublicConstructorAccepting(this Type type, params Type[] paramTypes)
		{
			return GetConstructorAccepting(type, paramTypes, false);
		}

		public static ConstructorInfo GetDefaultConstructor(this Type type)
		{
			return GetConstructorAccepting(type);
		}

		public static ConstructorInfo GetPublicDefaultConstructor(this Type type)
		{
			return GetPublicConstructorAccepting(type);
		}

		public static MemberInfo[] GetExtendedMember(this Type type, string name, MemberTypes types, BindingFlags flags)
		{
			using (ProfilingUtility.SampleBlock("GetExtendedMember"))
			{
				var members = type.GetMember(name, types, flags).ToList();

				using (ProfilingUtility.SampleBlock("GetExtendedMember.Extensions"))
				{
					if (types.HasFlag(MemberTypes.Method)) // Check for extension methods
					{
						members.AddRange(type.GetExtensionMethods()
							.Where(extension => extension.Name == name)
							.Cast<MemberInfo>());
					}
				}

				return members.ToArray();
			}
		}

		public static MemberInfo[] GetExtendedMembers(this Type type, BindingFlags flags)
		{
			var members = type.GetMembers(flags).ToHashSet();

			foreach (var extensionMethod in type.GetExtensionMethods())
			{
				members.Add(extensionMethod);
			}

			return members.ToArray();
		}



		#region Signature Disambiguation

		private static bool NameMatches(this MemberInfo member, string name)
		{
			return member.Name == name;
		}

		private static bool ParameterTypesMatch(this PropertyInfo propertyInfo, IEnumerable<Type> parameterTypes)
		{
			Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

			return propertyInfo.GetIndexParameters().Select(paramInfo => paramInfo.ParameterType).SequenceEqual(parameterTypes);
		}

		private static bool ParameterTypesMatch(this MethodBase methodBase, IEnumerable<Type> parameterTypes, bool withoutThis = false, bool legacyParameters = false)
		{
			Ensure.That(nameof(methodBase)).IsNotNull(methodBase);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

			var methodParameters = withoutThis ? methodBase.GetParametersWithoutThis() : methodBase.GetParameters();
			parameterTypes = withoutThis && !legacyParameters && methodBase.IsExtensionMethod() ? parameterTypes.Skip(1) : parameterTypes;

			return methodParameters.Select(paramInfo => paramInfo.ParameterType).SequenceEqual(parameterTypes);
		}

		private static bool ParameterOpenTypeNamesMatch(this PropertyInfo propertyInfo, IEnumerable<string> parameterTypeNames)
		{
			Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);
			Ensure.That(nameof(parameterTypeNames)).IsNotNull(parameterTypeNames);

			return propertyInfo.GetIndexParameters().Select(paramInfo => paramInfo.ParameterType.ToString()).SequenceEqual(parameterTypeNames);
		}

		private static bool ParameterOpenTypeNamesMatch(this MethodBase methodBase, IEnumerable<string> parameterTypeNames, bool withoutThis = false)
		{
			Ensure.That(nameof(methodBase)).IsNotNull(methodBase);
			Ensure.That(nameof(parameterTypeNames)).IsNotNull(parameterTypeNames);

			var methodParameters = withoutThis ? methodBase.GetParametersWithoutThis() : methodBase.GetParameters();
			parameterTypeNames = withoutThis && methodBase.IsExtensionMethod() ? parameterTypeNames.Skip(1) : parameterTypeNames;

			return methodParameters.Select(paramInfo => paramInfo.ParameterType.ToString()).SequenceEqual(parameterTypeNames);
		}

		private static bool GenericArgumentsMatch(this MethodInfo method, IEnumerable<Type> genericArgumentTypes)
		{
			Ensure.That(nameof(genericArgumentTypes)).IsNotNull(genericArgumentTypes);

			if (method.ContainsGenericParameters)
			{
				return false;
			}

			return method.GetGenericArguments().SequenceEqual(genericArgumentTypes);
		}

		public static PropertyInfo GetPropertyUnambiguous(this Type type, string name, BindingFlags flags)
		{
			return GetPropertyUnambiguous(type, name, flags, Empty<Type>.array);
		}

		public static PropertyInfo GetPropertyUnambiguous(this Type type, string name, BindingFlags flags, Type[] parameterTypes)
		{
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(name)).IsNotNull(name);

			flags |= BindingFlags.DeclaredOnly;

			while (type != null)
			{
				var property = type.GetProperty(name, flags, null, null, parameterTypes, null);

				if (property != null)
				{
					return property;
				}

				type = type.BaseType;
			}

			return null;
		}

		private static TMemberInfo DisambiguateHierarchy<TMemberInfo>(this IEnumerable<TMemberInfo> members, Type type) where TMemberInfo : MemberInfo
		{
			var _members = members.ToArrayPooled();

			try
			{
				if (_members.Length == 1)
				{
					return _members[0];
				}

				while (type != null)
				{
					foreach (var member in members)
					{
						if (member.DeclaringOrExtendedType() == type)
						{
							return member;
						}
					}

					type = type.BaseType;
				}

				return null;
			}
			finally
			{
				_members.Free();
			}
		}

		public static FieldInfo Disambiguate(this IEnumerable<FieldInfo> fields, Type type)
		{
			Ensure.That(nameof(fields)).IsNotNull(fields);
			Ensure.That(nameof(type)).IsNotNull(type);
			
			var _fields = fields.ToArrayPooled();

			try
			{
				if (_fields.Length == 1)
				{
					return _fields[0];
				}

				return _fields.DisambiguateHierarchy(type);
			}
			finally
			{
				_fields.Free();
			}
		}

		public static PropertyInfo Disambiguate(this IEnumerable<PropertyInfo> properties, Type type)
		{
			return Disambiguate(properties, type, Enumerable.Empty<Type>());
		}

		public static PropertyInfo Disambiguate(this IEnumerable<PropertyInfo> properties, Type type, IEnumerable<Type> parameterTypes)
		{
			Ensure.That(nameof(properties)).IsNotNull(properties);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);
			
			var _properties = properties.ToArrayPooled();

			try
			{
				if (_properties.Length == 1)
				{
					return _properties[0];
				}

				return properties.Where(p => p.ParameterTypesMatch(parameterTypes)).DisambiguateHierarchy(type);
			}
			finally
			{
				_properties.Free();
			}
		}

		public static PropertyInfo Disambiguate(this IEnumerable<PropertyInfo> properties, Type type, IEnumerable<string> parameterTypes)
		{
			Ensure.That(nameof(properties)).IsNotNull(properties);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);
			
			var _properties = properties.ToArrayPooled();

			try
			{
				if (_properties.Length == 1)
				{
					return _properties[0];
				}

				return _properties.Where(p => p.ParameterOpenTypeNamesMatch(parameterTypes)).DisambiguateHierarchy(type);
			}
			finally
			{
				_properties.Free();
			}
		}

		public static ConstructorInfo Disambiguate(this IEnumerable<ConstructorInfo> constructors, Type type, IEnumerable<Type> parameterTypes, bool withoutThis = false, bool legacyParameters = false)
		{
			Ensure.That(nameof(constructors)).IsNotNull(constructors);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);
			
			var _constructors = constructors.ToArrayPooled();

			try
			{
				if (_constructors.Length == 1)
				{
					return _constructors[0];
				}

				return _constructors.Where(m => m.ParameterTypesMatch(parameterTypes, withoutThis, legacyParameters)).DisambiguateHierarchy(type);
			}
			finally
			{
				_constructors.Free();
			}
		}

		public static ConstructorInfo Disambiguate(this IEnumerable<ConstructorInfo> constructors, Type type, IEnumerable<string> parameterOpenTypeNames, bool withoutThis = false)
		{
			Ensure.That(nameof(constructors)).IsNotNull(constructors);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterOpenTypeNames)).IsNotNull(parameterOpenTypeNames);
			
			var _constructors = constructors.ToArrayPooled();

			try
			{
				if (_constructors.Length == 1)
				{
					return _constructors[0];
				}

				return _constructors.Where(m => m.ParameterOpenTypeNamesMatch(parameterOpenTypeNames, withoutThis)).DisambiguateHierarchy(type);
			}
			finally
			{
				_constructors.Free();
			}
		}

		public static MethodInfo Disambiguate(this IEnumerable<MethodInfo> methods, Type type, IEnumerable<Type> parameterTypes, bool withoutThis = false, bool legacyParameters = false)
		{
			Ensure.That(nameof(methods)).IsNotNull(methods);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterTypes)).IsNotNull(parameterTypes);

			var _methods = methods.ToArrayPooled();

			try
			{
				if (_methods.Length == 1)
				{
					return _methods[0];
				}

				return _methods
					.Where(m => m.ParameterTypesMatch(parameterTypes, withoutThis, legacyParameters))
					.DisambiguateHierarchy(type);
			}
			finally
			{
				_methods.Free();
			}
		}

		public static MethodInfo Disambiguate(this IEnumerable<MethodInfo> methods, Type type, IEnumerable<string> parameterOpenTypeNames, int methodArity, bool withoutThis = false)
		{
			Ensure.That(nameof(methods)).IsNotNull(methods);
			Ensure.That(nameof(type)).IsNotNull(type);
			Ensure.That(nameof(parameterOpenTypeNames)).IsNotNull(parameterOpenTypeNames);

			var _methods = methods.ToArrayPooled();

			try
			{
				if (_methods.Length == 1)
				{
					return _methods[0];
				}

				return _methods
					.Where(m => (m.IsGenericMethod ? m.GetGenericArguments().Length : 0) == methodArity)
					.Where(m => m.ParameterOpenTypeNamesMatch(parameterOpenTypeNames, withoutThis))
					.DisambiguateHierarchy(type);
			}
			finally
			{
				_methods.Free();
			}
		}

		public static EventInfo Disambiguate(this IEnumerable<EventInfo> events, Type type)
		{
			Ensure.That(nameof(events)).IsNotNull(events);
			Ensure.That(nameof(type)).IsNotNull(type);
			
			var _events = events.ToArrayPooled();

			try
			{
				if (_events.Length == 1)
				{
					return _events[0];
				}

				return _events.DisambiguateHierarchy(type);
			}
			finally
			{
				_events.Free();
			}
		}

		#endregion
	}
}