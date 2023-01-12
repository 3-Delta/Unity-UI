using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class TypeUtility
	{
		private static readonly HashSet<Type> _numericTypes = new HashSet<Type>
		{
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal)
		};

		private static readonly HashSet<Type> _numericConstructTypes = new HashSet<Type>
		{
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Vector2Int),
			typeof(Vector3Int),
			typeof(Quaternion),
			typeof(Matrix4x4),
			typeof(Rect),
		};

		private static readonly HashSet<Type> typesWithShortStrings = new HashSet<Type>()
		{
			typeof(string),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Vector2Int),
			typeof(Vector3Int),
		};

		public static bool IsBasic(this Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			if (type == typeof(string) || type == typeof(decimal))
			{
				return true;
			}

			if (type.IsEnum)
			{
				return true;
			}

			if (type.IsPrimitive)
			{
				if (type == typeof(IntPtr) || type == typeof(UIntPtr))
				{
					return false;
				}

				return true;
			}

			return false;
		}

		public static bool IsNumeric(this Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			return _numericTypes.Contains(type);
		}

		public static bool IsNumericConstruct(this Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			return _numericConstructTypes.Contains(type);
		}

		public static Namespace Namespace(this Type type)
		{
			return PeekCore.Namespace.FromFullName(type.Namespace);
		}

		public static LooseAssemblyName LooseName(this Assembly assembly)
		{
			return (LooseAssemblyName)assembly.GetName();
		}

		public static Func<object> Instantiator(this Type type, bool nonPublic = true)
		{
			var instantiator = type.Instantiator(nonPublic, Empty<Type>.array);

			if (instantiator != null)
			{
				return () => instantiator.Invoke(Empty<object>.array);
			}

			return null;
		}

		public static Func<object[], object> Instantiator(this Type type, bool nonPublic = true, params Type[] parameterTypes)
		{
			// Unity objects cannot be instantiated via constructor
			if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return null;
			}

			// Value types don't have parameterless constructors at the IL level
			// http://stackoverflow.com/questions/3751519/
			if ((type.IsValueType || type.IsBasic()) && parameterTypes.Length == 0)
			{
				return (args) => type.PseudoDefault();
			}

			// Open-constructed generic types cannot be instantiated
			if (type.ContainsGenericParameters)
			{
				return null;
			}

			// Look for matching constructor
			var constructor = type.GetConstructorAccepting(parameterTypes, nonPublic);

			if (constructor != null)
			{
				return (args) => constructor.Invoke(args);
			}

			// Can't instantiate from given access and parameter types
			return null;
		}

		public static object TryInstantiate(this Type type, bool nonPublic = true, params object[] args)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			var instantiator = type.Instantiator(nonPublic, args.Select(arg => arg.GetType()).ToArray());

			return instantiator?.Invoke(args);
		}

		public static object Instantiate(this Type type, bool nonPublic = true, params object[] args)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			var parameterTypes = args.Select(arg => arg.GetType()).ToArray();

			var instantiator = type.Instantiator(nonPublic, parameterTypes);

			if (instantiator == null)
			{
				throw new ArgumentException($"Type {type} cannot be{(nonPublic ? "" : " publicly")} instantiated with the provided parameter types: {parameterTypes.ToCommaSeparatedString()}");
			}

			return instantiator(args);
		}

		public static object Default(this Type type)
		{
			Ensure.That(nameof(type)).IsNotNull(type);

			if (type.IsReferenceType())
			{
				return null;
			}

			if (!defaultPrimitives.TryGetValue(type, out var defaultPrimitive))
			{
				defaultPrimitive = Activator.CreateInstance(type);
			}

			return defaultPrimitive;
		}

		public static object PseudoDefault(this Type type)
		{
			if (type == typeof(Color))
			{
				return Color.white;
			}
			else if (type == typeof(string))
			{
				return string.Empty;
			}
			else if (type == typeof(AnimationCurve))
			{
				return AnimationCurve.Linear(0, 0, 1, 1);
			}
			else if (type == typeof(Gradient))
			{
				var gradient = new Gradient();

				gradient.SetKeys
				(
					new []
					{
						new GradientColorKey
						{
							color = Color.red,
							time = 0
						},

						new GradientColorKey
						{
							color = Color.blue,
							time = 1
						},
					},

					new []
					{
						new GradientAlphaKey
						{
							alpha = 1,
							time = 0,
						},
						new GradientAlphaKey
						{
							alpha = 1,
							time = 1,
						},
					}
				);

				return gradient;
			}
			else if (type.IsEnum)
			{
				// Support the [DefaultValue] attribute, fallback to zero-value
				// https://stackoverflow.com/questions/529929

				var values = Enum.GetValues(type);

				if (values.Length == 0)
				{
					Debug.LogWarning($"Empty enum: {type}\nThis may cause problems with serialization.");
					return Activator.CreateInstance(type);
				}

				var attribute = type.GetAttribute<DefaultValueAttribute>();

				if (attribute != null)
				{
					return attribute.Value;
				}

				return values.GetValue(0);
			}

			return type.Default();
		}

		private static readonly Dictionary<Type, object> defaultPrimitives = new Dictionary<Type, object>()
		{
			{typeof(int), default(int)},
			{typeof(uint), default(uint)},
			{typeof(long), default(long)},
			{typeof(ulong), default(ulong)},
			{typeof(short), default(short)},
			{typeof(ushort), default(ushort)},
			{typeof(byte), default(byte)},
			{typeof(sbyte), default(sbyte)},
			{typeof(float), default(float)},
			{typeof(double), default(double)},
			{typeof(decimal), default(decimal)},
			{typeof(Vector2), default(Vector2)},
			{typeof(Vector3), default(Vector3)},
			{typeof(Vector4), default(Vector4)},
			{typeof(Vector2Int), default(Vector2Int)},
			{typeof(Vector3Int), default(Vector3Int)},
		};

		public static bool IsStatic(this Type type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		public static bool IsAbstract(this Type type)
		{
			// Do not return true for static types
			return type.IsAbstract && !type.IsSealed;
		}

		public static bool IsConcrete(this Type type)
		{
			return !type.IsAbstract && !type.IsInterface && !type.ContainsGenericParameters;
		}

		public static IEnumerable<Type> GetInterfaces(this Type type, bool includeInherited)
		{
			if (includeInherited || type.BaseType == null)
			{
				return type.GetInterfaces();
			}
			else
			{
				return type.GetInterfaces().Except(type.BaseType.GetInterfaces());
			}
		}

		public static IEnumerable<Type> BaseTypeAndInterfaces(this Type type, bool inheritedInterfaces = true)
		{
			var types = Enumerable.Empty<Type>();

			if (type.BaseType != null)
			{
				types = types.Concat(type.BaseType.Yield());
			}

			types = types.Concat(type.GetInterfaces(inheritedInterfaces));

			return types;
		}

		public static IEnumerable<Type> Hierarchy(this Type type)
		{
			var baseType = type.BaseType;

			while (baseType != null)
			{
				yield return baseType;

				baseType = baseType.BaseType;
			}

			foreach (var @interface in type.GetInterfaces(true))
			{
				yield return @interface;
			}
		}

		public static IEnumerable<Type> AndBaseTypeAndInterfaces(this Type type)
		{
			return type.Yield().Concat(type.BaseTypeAndInterfaces());
		}

		public static IEnumerable<Type> AndInterfaces(this Type type)
		{
			return type.Yield().Concat(type.GetInterfaces());
		}

		public static IEnumerable<Type> AndHierarchy(this Type type)
		{
			return type.Yield().Concat(type.Hierarchy());
		}

		public static Type GetListElementType(Type listType, bool allowNonGeneric)
		{
			if (listType == null)
			{
				throw new ArgumentNullException(nameof(listType));
			}

			// http://stackoverflow.com/questions/4452590

			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			else if (typeof(IList).IsAssignableFrom(listType))
			{
				var genericListInterface =
					listType
						.AndInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));

				if (genericListInterface == null)
				{
					if (allowNonGeneric)
					{
						return typeof(object);
					}
					else
					{
						return null;
					}
				}

				return genericListInterface.GetGenericArguments()[0];
			}
			else
			{
				return null;
			}
		}

		public static Type GetEnumerableElementType(Type enumerableType, bool allowNonGeneric)
		{
			if (enumerableType == null)
			{
				throw new ArgumentNullException(nameof(enumerableType));
			}

			// http://stackoverflow.com/a/12728562

			if (typeof(IEnumerable).IsAssignableFrom(enumerableType))
			{
				var genericEnumerableInterface =
					enumerableType
						.AndInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

				if (genericEnumerableInterface == null)
				{
					if (allowNonGeneric)
					{
						return typeof(object);
					}
					else
					{
						return null;
					}
				}

				return genericEnumerableInterface.GetGenericArguments()[0];
			}
			else
			{
				return null;
			}
		}

		public static Type GetDictionaryItemType(Type dictionaryType, bool allowNonGeneric, int genericArgumentIndex)
		{
			if (dictionaryType == null)
			{
				throw new ArgumentNullException(nameof(dictionaryType));
			}

			if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
			{
				var genericDictionaryInterface =
					dictionaryType
						.AndInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

				if (genericDictionaryInterface == null)
				{
					if (allowNonGeneric)
					{
						return typeof(object);
					}
					else
					{
						return null;
					}
				}

				return genericDictionaryInterface.GetGenericArguments()[genericArgumentIndex];
			}
			else
			{
				return null;
			}
		}

		public static Type GetDictionaryKeyType(Type dictionaryType, bool allowNonGeneric)
		{
			return GetDictionaryItemType(dictionaryType, allowNonGeneric, 0);
		}

		public static Type GetDictionaryValueType(Type dictionaryType, bool allowNonGeneric)
		{
			return GetDictionaryItemType(dictionaryType, allowNonGeneric, 1);
		}

		public static bool IsNullable(this Type type)
		{
			// http://stackoverflow.com/a/1770232
			return type.IsReferenceType() || Nullable.GetUnderlyingType(type) != null;
		}

		public static bool IsReferenceType(this Type type)
		{
			return !type.IsValueType;
		}

		public static bool IsStruct(this Type type)
		{
			return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
		}

		public static bool IsDelegate(this Type type)
		{
			return type != typeof(Delegate) && typeof(Delegate).IsAssignableFrom(type);
		}

		public static bool IsGenericDelegate(this Type type)
		{
			return type.Namespace == "System" && (type.Name == "Action" || type.Name == "Func") && type.IsDelegate();
		}

		public static bool SupportsMembers(this Type type)
		{
			return !type.IsEnum && !type.IsDelegate();
		}

		public static bool IsInstanceOfTypeNullable(this Type type, object value)
		{
			if (value == null)
			{
				return type.IsNullable();
			}
			else
			{
				return type.IsInstanceOfType(value);
			}
		}

		public static bool CanMakeGenericTypeVia(this Type openConstructedType, Type closedConstructedType)
		{
			using (ProfilingUtility.SampleBlock(nameof(CanMakeGenericTypeVia)))
			{
				Ensure.That(nameof(openConstructedType)).IsNotNull(openConstructedType);
				Ensure.That(nameof(closedConstructedType)).IsNotNull(closedConstructedType);

				if (openConstructedType == closedConstructedType)
				{
					return true;
				}

				if (openConstructedType.IsGenericParameter) // e.g.: T
				{
					// The open-constructed type is a generic parameter. 

					// First, check if all special attribute constraints are respected.

					var constraintAttributes = openConstructedType.GenericParameterAttributes;

					if (constraintAttributes != GenericParameterAttributes.None)
					{
						// e.g.: where T : struct
						if (constraintAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) &&
						    !closedConstructedType.IsValueType)
						{
							return false;
						}

						// e.g.: where T : class
						if (constraintAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) &&
						    closedConstructedType.IsValueType)
						{
							return false;
						}

						// e.g.: where T : new()
						if (constraintAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint) &&
						    closedConstructedType.GetConstructor(Type.EmptyTypes) == null)
						{
							return false;
						}
					}

					// Then, check if all type constraints are respected.

					// e.g.: where T : BaseType, IInterface1, IInterface2
					foreach (var constraint in openConstructedType.GetGenericParameterConstraints())
					{
						if (!constraint.IsAssignableFrom(closedConstructedType))
						{
							return false;
						}
					}

					return true;
				}
				else if (openConstructedType.ContainsGenericParameters)
				{
					// The open-constructed type is not a generic parameter but contains generic parameters.
					// It could be either a generic type or an array.

					if (openConstructedType.IsGenericType) // e.g. Generic<T1, int, T2>
					{
						// The open-constructed type is a generic type.

						var openConstructedGenericDefinition = openConstructedType.GetGenericTypeDefinition(); // e.g.: Generic<,,>
						var openConstructedGenericArguments = openConstructedType.GetGenericArguments(); // e.g.: { T1, int, T2 }

						// Check a list of possible candidate closed-constructed types:
						//	- the closed-constructed type itself
						//  - its base type, if any (i.e.: if the closed-constructed type is not object)
						//  - its implemented interfaces

						foreach (var inheritedClosedConstructedType in closedConstructedType.AndBaseTypeAndInterfaces())
						{
							if (inheritedClosedConstructedType.IsGenericType &&
							    inheritedClosedConstructedType.GetGenericTypeDefinition() == openConstructedGenericDefinition)
							{
								// The inherited closed-constructed type and the open-constructed type share the same generic definition.

								var inheritedClosedConstructedGenericArguments = inheritedClosedConstructedType.GetGenericArguments(); // e.g.: { float, int, string }

								// For each open-constructed generic argument, recursively check if it
								// can be made into a closed-constructed type via the closed-constructed generic argument.

								for (var i = 0; i < openConstructedGenericArguments.Length; i++)
								{
									if (!openConstructedGenericArguments[i].CanMakeGenericTypeVia(inheritedClosedConstructedGenericArguments[i])) // !T1.IsAssignableFromGeneric(float)
									{
										return false;
									}
								}

								// The inherited closed-constructed type matches the generic definition of 
								// the open-constructed type and each of its type arguments are assignable to each equivalent type
								// argument of the constraint.

								return true;
							}
						}

						// The open-constructed type contains generic parameters, but no
						// inherited closed-constructed type has a matching generic definition.

						return false;
					}
					else if (openConstructedType.IsArray) // e.g. T[]
					{
						// The open-constructed type is an array.

						if (!closedConstructedType.IsArray ||
						    closedConstructedType.GetArrayRank() != openConstructedType.GetArrayRank())
						{
							// Fail if the closed-constructed type isn't an array of the same rank.
							return false;
						}

						var openConstructedElementType = openConstructedType.GetElementType();
						var closedConstructedElementType = closedConstructedType.GetElementType();

						return openConstructedElementType.CanMakeGenericTypeVia(closedConstructedElementType);
					}
					else if (openConstructedType.IsByRef) // e.g. T&
					{
						// The open-constructed type is by ref.

						if (!closedConstructedType.IsByRef)
						{
							// Fail if the closed-constructed type isn't also by ref.
							return false;
						}

						var openConstructedElementType = openConstructedType.GetElementType();
						var closedConstructedElementType = closedConstructedType.GetElementType();

						return openConstructedElementType.CanMakeGenericTypeVia(closedConstructedElementType);
					}
					else
					{
						throw new NotImplementedException();
					}
				}
				else
				{
					// The open-constructed type does not contain generic parameters,
					// we can proceed to a regular closed-type check.

					return openConstructedType.IsAssignableFrom(closedConstructedType);
				}
			}
		}

		public static Type MakeGenericTypeVia(this Type openConstructedType, Type closedConstructedType, Dictionary<Type, Type> resolvedGenericParameters, bool safe = true)
		{
			Ensure.That(nameof(openConstructedType)).IsNotNull(openConstructedType);
			Ensure.That(nameof(closedConstructedType)).IsNotNull(closedConstructedType);
			Ensure.That(nameof(resolvedGenericParameters)).IsNotNull(resolvedGenericParameters);

			if (safe && !openConstructedType.CanMakeGenericTypeVia(closedConstructedType))
			{
				throw new GenericClosingException(openConstructedType, closedConstructedType);
			}

			if (openConstructedType == closedConstructedType)
			{
				return openConstructedType;
			}

			if (openConstructedType.IsGenericParameter) // e.g.: T
			{
				// The open-constructed type is a generic parameter.
				// We can directly map it to the closed-constructed type.

				// Because this is the lowest possible level of type resolution,
				// we will add this entry to our list of resolved generic parameters
				// in case we need it later (e.g. for resolving generic methods).

				// Note that we allow an open-constructed type to "make" another
				// open-constructed type, as long as the former respects all of 
				// the latter's constraints. Therefore, we will only add the resolved 
				// parameter to our dictionary if it actually is resolved.

				if (!closedConstructedType.ContainsGenericParameters)
				{
					if (resolvedGenericParameters.ContainsKey(openConstructedType))
					{
						if (resolvedGenericParameters[openConstructedType] != closedConstructedType)
						{
							throw new InvalidOperationException("Nested generic parameters resolve to different values.");
						}
					}
					else
					{
						resolvedGenericParameters.Add(openConstructedType, closedConstructedType);
					}
				}

				return closedConstructedType;
			}
			else if (openConstructedType.ContainsGenericParameters) // e.g.: Generic<T1, int, T2>
			{
				// The open-constructed type is not a generic parameter but contains generic parameters.
				// It could be either a generic type or an array.

				if (openConstructedType.IsGenericType) // e.g. Generic<T1, int, T2>
				{
					// The open-constructed type is a generic type.

					var openConstructedGenericDefinition = openConstructedType.GetGenericTypeDefinition(); // e.g.: Generic<,,>
					var openConstructedGenericArguments = openConstructedType.GetGenericArguments(); // e.g.: { T1, int, T2 }

					// Check a list of possible candidate closed-constructed types:
					//	- the closed-constructed type itself
					//  - its base type, if any (i.e.: if the closed-constructed type is not object)
					//  - its implemented interfaces

					foreach (var inheritedCloseConstructedType in closedConstructedType.AndBaseTypeAndInterfaces())
					{
						if (inheritedCloseConstructedType.IsGenericType &&
						    inheritedCloseConstructedType.GetGenericTypeDefinition() == openConstructedGenericDefinition)
						{
							// The inherited closed-constructed type and the open-constructed type share the same generic definition.

							var inheritedClosedConstructedGenericArguments = inheritedCloseConstructedType.GetGenericArguments(); // e.g.: { float, int, string }

							// For each inherited open-constructed type generic argument, recursively resolve it
							// via the equivalent closed-constructed type generic argument.

							var closedConstructedGenericArguments = new Type[openConstructedGenericArguments.Length];

							for (var j = 0; j < openConstructedGenericArguments.Length; j++)
							{
								closedConstructedGenericArguments[j] = MakeGenericTypeVia
								(
									openConstructedGenericArguments[j],
									inheritedClosedConstructedGenericArguments[j],
									resolvedGenericParameters,
									safe: false // We recursively checked before, no need to do it again
								);

								// e.g.: Resolve(T1, float)
							}

							// Construct the final closed-constructed type from the resolved arguments

							return openConstructedGenericDefinition.MakeGenericType(closedConstructedGenericArguments);
						}
					}

					// The open-constructed type contains generic parameters, but no 
					// inherited closed-constructed type has a matching generic definition.
					// This cannot happen in safe mode, but could in unsafe mode.

					throw new GenericClosingException(openConstructedType, closedConstructedType);
				}
				else if (openConstructedType.IsArray) // e.g. T[]
				{
					var arrayRank = openConstructedType.GetArrayRank();

					// The open-constructed type is an array.

					if (!closedConstructedType.IsArray ||
					    closedConstructedType.GetArrayRank() != arrayRank)
					{
						// Fail if the closed-constructed type isn't an array of the same rank.
						// This cannot happen in safe mode, but could in unsafe mode.
						throw new GenericClosingException(openConstructedType, closedConstructedType);
					}

					var openConstructedElementType = openConstructedType.GetElementType();
					var closedConstructedElementType = closedConstructedType.GetElementType();

					return openConstructedElementType.MakeGenericTypeVia
					(
						closedConstructedElementType,
						resolvedGenericParameters,
						safe: false
					).MakeArrayType(arrayRank);
				}
				else if (openConstructedType.IsByRef) // e.g. T&
				{
					// The open-constructed type is by ref.

					if (!closedConstructedType.IsByRef)
					{
						// Fail if the closed-constructed type isn't also by ref.
						// This cannot happen in safe mode, but could in unsafe mode.
						throw new GenericClosingException(openConstructedType, closedConstructedType);
					}

					var openConstructedElementType = openConstructedType.GetElementType();
					var closedConstructedElementType = closedConstructedType.GetElementType();

					return openConstructedElementType.MakeGenericTypeVia
					(
						closedConstructedElementType,
						resolvedGenericParameters,
						safe: false
					).MakeByRefType();
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				// The open-constructed type does not contain generic parameters,
				// it is by definition already resolved.

				return openConstructedType;
			}
		}

		public static string ToShortString(this object o, int maxLength = 20)
		{
			var type = o?.GetType();

			if (type == null || o.IsUnityNull())
			{
				return "Null";
			}
			else if (type == typeof(float))
			{
				return ((float)o).ToString("0.##");
			}
			else if (type == typeof(double))
			{
				return ((double)o).ToString("0.##");
			}
			else if (type == typeof(decimal))
			{
				return ((decimal)o).ToString("0.##");
			}
			else if (type.IsBasic() || typesWithShortStrings.Contains(type))
			{
				return o.ToString().Truncate(maxLength);
			}
			else if (typeof(UnityObject).IsAssignableFrom(type))
			{
				return ((UnityObject)o).name.Truncate(maxLength);
			}
			else if (typeof(Guid).IsAssignableFrom(type))
			{
				return ((Guid)o).ToShortString();
			}
			else
			{
				return null;
			}
		}

		public static IEnumerable<Type> GetTypesSafely(this Assembly assembly)
		{
			Type[] types;

			try
			{
				types = assembly.GetTypes();
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"Failed to load types in assembly '{assembly}'.\n{ex}");

				yield break;
			}

			foreach (var type in types)
			{
				// Apparently void can be returned somehow:
				// http://support.ludiq.io/topics/483
				if (type == typeof(void))
				{
					continue;
				}

				yield return type;
			}
		}

		public static string NameWithoutGenericArity(this Type type)
		{
			var name = type.Name;
			var index = name.IndexOf('`');
			return index >= 0
				? name.Substring(0, index)
				: name;
		}

		public static bool HasAnyComparisonOperator(this Type type)
		{
			if (type.IsPrimitive || type.IsEnum || type.IsClass)
			{
				return true;
			}

			return
				BinaryOperator.Equality.Handler().Supports(type, type) ||
				BinaryOperator.Inequality.Handler().Supports(type, type) ||
				BinaryOperator.GreaterThan.Handler().Supports(type, type) ||
				BinaryOperator.LessThan.Handler().Supports(type, type) ||
				BinaryOperator.GreaterThanOrEqual.Handler().Supports(type, type) ||
				BinaryOperator.LessThanOrEqual.Handler().Supports(type, type);
		}

		public static bool IsFlagsEnum(this Type type)
		{
			return type.IsEnum && type.HasAttribute<FlagsAttribute>(false);
		}

		public static bool IsPseudoFlagsEnum(this Type type)
		{
			return type.IsFlagsEnum()
			       || type == typeof(RigidbodyConstraints)
			       || type == typeof(CollisionFlags);
		}

		public static Type ToType(this TypeCode code)
		{
			switch (code)
			{
				case TypeCode.Boolean: return typeof(bool);
				case TypeCode.Byte: return typeof(byte);
				case TypeCode.SByte: return typeof(sbyte);
				case TypeCode.Int16: return typeof(short);
				case TypeCode.Int32: return typeof(int);
				case TypeCode.Int64: return typeof(long);
				case TypeCode.UInt16: return typeof(ushort);
				case TypeCode.UInt32: return typeof(uint);
				case TypeCode.UInt64: return typeof(ulong);
				case TypeCode.Single: return typeof(float);
				case TypeCode.Double: return typeof(double);
				case TypeCode.Decimal: return typeof(decimal);
				case TypeCode.Object: return typeof(object);
				case TypeCode.Char: return typeof(char);
				case TypeCode.String: return typeof(string);
				case TypeCode.DateTime: return typeof(DateTime);
				case TypeCode.DBNull: return typeof(DBNull);
				case TypeCode.Empty: return null;
				default: throw new UnexpectedEnumValueException<TypeCode>(code);
			}
		}

		public static T CastTo<T>(this object o)
		{
			try
			{
				return (T)o;
			}
			catch (InvalidCastException)
			{
				throw new InvalidCastException($"Specified cast is not valid.\nExpected {typeof(T)}, found {o?.GetType().ToString() ?? "null"}.");
			}
		}
	}
}