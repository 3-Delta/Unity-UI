using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public sealed class Member : ISerializationCallbackReceiver
	{
		public enum Source
		{
			Unknown,
			Field,
			Property,
			Method,
			Constructor,
			Indexer,
			Event
		}

		[Obsolete(Serialization.ConstructorWarning)]
		public Member() { }

		public Member(Type targetType, string name, Type[] parameterTypes = null, string[] parameterOpenTypeNames = null, Type[] genericMethodTypeArguments = null)
		{
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(name)).IsNotNull(name);

			if (targetType.ContainsGenericParameters)
			{
				targetType = targetType.GetGenericTypeDefinition();
			}

			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentNullException(nameof(parameterTypes) + $"[{i}]");
					}
				}
			}

			_targetType = targetType;
			_name = name;
			this.parameterTypes = parameterTypes;
			this.parameterOpenTypeNames = parameterOpenTypeNames;
			this._genericMethodTypeArguments = genericMethodTypeArguments;
		}

		public Member(Type targetType, FieldInfo fieldInfo)
		{
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);
			 
			_source = Source.Field;

			if (targetType.ContainsGenericParameters && !targetType.IsGenericTypeDefinition)
			{
				targetType = targetType.GetGenericTypeDefinition();
				fieldInfo = targetType.GetMember(fieldInfo.Name, SupportedBindingFlags).OfType<FieldInfo>().Single(fi => fi.MetadataToken == fieldInfo.MetadataToken);
			}

			_fieldInfo = fieldInfo;
			_targetType = targetType;
			_name = _fieldInfo.Name;
			parameterTypes = null;
			reflectedType = _fieldInfo.ReflectedType;
			_isReflected = true;
		}

		public Member(Type targetType, PropertyInfo propertyInfo)
		{
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);

			_source = Source.Property;

			if (targetType.ContainsGenericParameters && !targetType.IsGenericTypeDefinition)
			{
				targetType = targetType.GetGenericTypeDefinition();
				propertyInfo = targetType.GetMember(propertyInfo.Name, SupportedBindingFlags).OfType<PropertyInfo>().Single(pi => pi.MetadataToken == propertyInfo.MetadataToken);
			}

			if (propertyInfo.IsIndexer())
			{
				_source = Source.Indexer;
			}

			_propertyInfo = propertyInfo;
			_targetType = targetType;
			_name = _propertyInfo.Name;
			parameterTypes = null;
			reflectedType = _propertyInfo.ReflectedType;
			parameterTypes = _propertyInfo.GetIndexParameters().Select(pi => pi.ParameterType).ToArray();
			parameterOpenTypeNames = _propertyInfo.GetIndexParameters().Select(pi => pi.ParameterType.ToString()).ToArray();
			_isReflected = true;
		}

		public Member(Type targetType, MethodInfo methodInfo)
		{
			// TODO / BUG: Here, if we pass the DeclaringType as the targetType, 
			// like we could we're not careful after a GetMembers call, we don't get
			// a valid Member object that can properly reflect after deserialization.

			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);

			_source = Source.Method;

			if (targetType.ContainsGenericParameters && !targetType.IsGenericTypeDefinition)
			{
				var oldTargetType = targetType;
				targetType = targetType.GetGenericTypeDefinition();

				if (methodInfo.IsExtension())
				{
					methodInfo = targetType.GetExtensionMethods().Single(mi => mi.ReflectedType == methodInfo.ReflectedType && mi.MetadataToken == methodInfo.MetadataToken);
				}
				else
				{
					methodInfo = targetType.GetMember(methodInfo.Name, SupportedBindingFlags).OfType<MethodInfo>().Single(mi => mi.MetadataToken == methodInfo.MetadataToken);
				}
			}

			_targetType = targetType;
			_methodInfo = methodInfo;

			_name = _methodInfo.Name;
			parameterTypes = _methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
			parameterOpenTypeNames = (_methodInfo.IsGenericMethod ? _methodInfo.GetGenericMethodDefinition() : _methodInfo).GetParameters().Select(pi => pi.ParameterType.ToString()).ToArray();
			_genericMethodTypeArguments = _methodInfo.IsGenericMethod ? _methodInfo.GetGenericArguments() : null;
			reflectedType = _methodInfo.ReflectedType;
			_isExtension = _methodInfo.IsExtension();
			_isReflected = true;
		}

		public Member(Type targetType, ConstructorInfo constructorInfo)
		{
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(constructorInfo)).IsNotNull(constructorInfo);

			_source = Source.Constructor;

			if (targetType.ContainsGenericParameters && !targetType.IsGenericTypeDefinition)
			{
				targetType = targetType.GetGenericTypeDefinition();
				constructorInfo = targetType.GetMember(constructorInfo.Name, SupportedBindingFlags & ~BindingFlags.Static).OfType<ConstructorInfo>().Single(ci => ci.MetadataToken == constructorInfo.MetadataToken);
			}

			_constructorInfo = constructorInfo;
			_targetType = targetType;
			_name = _constructorInfo.Name;
			parameterTypes = _constructorInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
			parameterOpenTypeNames = parameterTypes.Select(t => t.ToString()).ToArray();
			reflectedType = _constructorInfo.ReflectedType;
			_isReflected = true;
		}

		public Member(Type targetType, EventInfo eventInfo)
		{
			Ensure.That(nameof(targetType)).IsNotNull(targetType);
			Ensure.That(nameof(fieldInfo)).IsNotNull(eventInfo);
			 
			_source = Source.Event;

			if (targetType.ContainsGenericParameters && !targetType.IsGenericTypeDefinition)
			{
				targetType = targetType.GetGenericTypeDefinition();
				eventInfo = targetType.GetMember(eventInfo.Name, SupportedBindingFlags).OfType<EventInfo>().Single(fi => fi.MetadataToken == fieldInfo.MetadataToken);
			}

			_eventInfo = eventInfo;
			_targetType = targetType;
			_name = _eventInfo.Name;
			parameterTypes = null;
			reflectedType = _eventInfo.ReflectedType;
			_isReflected = true;
		}

		public Member(MemberData value)
		{
			_source = value.source;
			reflectedTypeName = value.reflectedType.ToName();
			_targetTypeName = value.targetType.ToName();
			_name = value.name;
			_isExtension = value.isExtension;
			parameterTypeNames = value.parameterTypeNames;
			parameterOpenTypeNames = value.parameterOpenTypeNames;
			genericMethodTypeArgumentNames = value.genericMethodTypeArgumentNames;
			((ISerializationCallbackReceiver)this).OnAfterDeserialize();
		}

		[SerializeAs(nameof(name))]
		private string _name;

		[DoNotSerialize]
		private bool _isReflected;

		[DoNotSerialize]
		private bool hasLegacyParameters;

		[SerializeAs(nameof(parameterTypes))]
		private Type[] legacySerializedParameterTypes;

		[DoNotSerialize]
		private Type[] parameterTypes;

		[SerializeAs(nameof(parameterTypeNames))]
		private string[] parameterTypeNames;

		[SerializeAs(nameof(parameterOpenTypeNames))]
		private string[] parameterOpenTypeNames;

		[SerializeAs(nameof(genericMethodTypeArgumentNames))]
		private string[] genericMethodTypeArgumentNames;

		[DoNotSerialize]
		private Type[] _genericMethodTypeArguments;

		[DoNotSerialize]
		private Type _targetType;

		[SerializeAs(nameof(targetTypeName))]
		private string _targetTypeName;

		[DoNotSerialize]
		private Type reflectedType;

		[SerializeAs(nameof(reflectedTypeName))]
		private string reflectedTypeName;

		[SerializeAs(nameof(source))]
		private Source _source;

		[DoNotSerialize]
		private FieldInfo _fieldInfo;

		[DoNotSerialize]
		private PropertyInfo _propertyInfo;

		[DoNotSerialize]
		private MethodInfo _methodInfo;

		[DoNotSerialize]
		private ConstructorInfo _constructorInfo;

		[DoNotSerialize]
		private EventInfo _eventInfo;

		[SerializeAs(nameof(isExtension))]
		private bool _isExtension;

		[DoNotSerialize]
		private IOptimizedAccessor fieldAccessor;

		[DoNotSerialize]
		private IOptimizedAccessor propertyAccessor;

		[DoNotSerialize]
		private IOptimizedInvoker methodInvoker;

		[DoNotSerialize]
		private IOptimizedInvoker indexerGetInvoker;

		[DoNotSerialize]
		private IOptimizedInvoker indexerSetInvoker;

		[DoNotSerialize]
		public Type targetType => _targetType;

		[DoNotSerialize]
		public string targetTypeName => _targetTypeName;

		[DoNotSerialize]
		public IEnumerable<Type> genericMethodTypeArguments => _genericMethodTypeArguments;

		[DoNotSerialize]
		public string name => _name;

		[DoNotSerialize]
		public bool isReflected => _isReflected;

		[DoNotSerialize]
		public Source source
		{
			get
			{
				EnsureReflected();
				return _source;
			}
		}

		[DoNotSerialize]
		public FieldInfo fieldInfo
		{
			get
			{
				EnsureReflected();
				return _fieldInfo;
			}
		}

		[DoNotSerialize]
		public PropertyInfo propertyInfo
		{
			get
			{
				EnsureReflected();
				return _propertyInfo;
			}
		}

		[DoNotSerialize]
		public MethodInfo methodInfo
		{
			get
			{
				EnsureReflected();
				return _methodInfo;
			}
		}

		[DoNotSerialize]
		public ConstructorInfo constructorInfo
		{
			get
			{
				EnsureReflected();
				return _constructorInfo;
			}
		}

		[DoNotSerialize]
		public EventInfo eventInfo
		{
			get
			{
				EnsureReflected();
				return _eventInfo;
			}
		}

		[DoNotSerialize]
		public bool isExtension
		{
			get
			{
				EnsureReflected();
				return _isExtension;
			}
		}

		public MethodBase methodBase
		{
			get
			{
				switch (source)
				{
					case Source.Method:
						return methodInfo;
					case Source.Constructor:
						return constructorInfo;
					default:
						return null;
				}
			}
		}
		
		private MemberInfo _info
		{
			get
			{
				switch (source)
				{
					case Source.Field:
						return _fieldInfo;
					case Source.Property:
					case Source.Indexer:
						return _propertyInfo;
					case Source.Method:
						return _methodInfo;
					case Source.Constructor:
						return _constructorInfo;
					case Source.Event:
						return _eventInfo;
					default:
						throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public MemberInfo info
		{
			get
			{
				switch (source)
				{
					case Source.Field:
						return fieldInfo;
					case Source.Property:
					case Source.Indexer:
						return propertyInfo;
					case Source.Method:
						return methodInfo;
					case Source.Constructor:
						return constructorInfo;
					case Source.Event:
						return eventInfo;
					default:
						throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public Type type
		{
			get
			{
				switch (source)
				{
					case Source.Field:
						return fieldInfo.FieldType;
					case Source.Property:
					case Source.Indexer:
						return propertyInfo.PropertyType;
					case Source.Method:
						return methodInfo.ReturnType;
					case Source.Constructor:
						return constructorInfo.DeclaringType;
					case Source.Event:
						return eventInfo.EventHandlerType;
					default:
						throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public bool isGettable => IsGettable(true);
		public bool isPubliclyGettable => IsGettable(false);

		public bool isSettable => IsSettable(true);
		public bool isPubliclySettable => IsSettable(false);

		public bool isInvocable => IsInvocable(true);
		public bool isPubliclyInvocable => IsInvocable(false);
		
		public bool isHookable => IsHookable(true);
		public bool isPubliclyHookable => IsHookable(false);

		public bool isAccessor
		{
			get
			{
				switch (source)
				{
					case Source.Field: return true;
					case Source.Property: return true;
					case Source.Method: return false;
					case Source.Constructor: return false;
					case Source.Indexer: return false;
					case Source.Event: return false;
					default: throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public bool isField => source == Source.Field;

		public bool isProperty => source == Source.Property;

		public bool isMethod => source == Source.Method;

		public bool isConstructor => source == Source.Constructor;

		public bool isIndexer => source == Source.Indexer;

		public bool isEvent => source == Source.Event;

		public bool requiresTarget
		{
			get
			{
				switch (source)
				{
					case Source.Field:
						return !fieldInfo.IsStatic;
					case Source.Property:
					case Source.Indexer:
						return !propertyInfo.IsStatic();
					case Source.Method:
						return !methodInfo.IsStatic || isExtension;
					case Source.Constructor:
						return false;
					case Source.Event:
						return !
							eventInfo.IsStatic();
					default:
						throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public bool isVirtual
		{
			get
			{
				switch (source)
				{
					case Source.Field:
						return false;
					case Source.Property:
					case Source.Indexer:
						return propertyInfo.IsVirtual();
					case Source.Method:
						return methodInfo.IsVirtual;
					case Source.Constructor:
						return false;
					case Source.Event:
						return eventInfo.IsVirtual();
					default:
						throw new UnexpectedEnumValueException<Source>(source);
				}
			}
		}

		public bool isOperator => isMethod && methodInfo.IsOperator();

		public bool isGenericMethod => isMethod && methodInfo.IsGenericMethod;

		public OperatorCategory operatorCategory => isMethod ? methodInfo.GetOperatorCategory() : OperatorCategory.None;

		public bool isConversion => isMethod && methodInfo.IsUserDefinedConversion();

		public int order => info.MetadataToken;

		public Type declaringType => info.DeclaringOrExtendedType();

		public bool isInherited => targetType != declaringType;

		public Type pseudoDeclaringType
		{
			get
			{
				// For Unity objects, we'll consider parent types to be only root types,
				// to allow common objects like BoxCollider to show Collider members as self-defined.
				// We'll also consider them as absolute roots, and therefore none of their members
				// should display as inherited.
				
				var declaringType = this.declaringType;

				if (typeof(UnityObject).IsAssignableFrom(targetType))
				{
					if (targetType == typeof(GameObject) ||
						targetType == typeof(Component) ||
						targetType == typeof(ScriptableObject))
					{
						return targetType;
					}
					else
					{
						if (declaringType != typeof(UnityObject) &&
							declaringType != typeof(GameObject) &&
							declaringType != typeof(Component) &&
							declaringType != typeof(MonoBehaviour) &&
							declaringType != typeof(ScriptableObject) &&
							declaringType != typeof(object))
						{
							return targetType;
						}
					}
				}

				// Some implicitly-defined struct methods have a declaring type of System.ValueType,
				// rather than object, but we don't include ValueType in the extracted types.
				// Return object instead so there are less greyed out inherited members in the option tree.

				if (declaringType == typeof(ValueType))
				{
					return typeof(object);
				}

				return declaringType;
			}
		}

		public bool isPseudoInherited => targetType != pseudoDeclaringType || (isMethod && methodInfo.IsGenericExtension());

		public bool isPredictable => isField || info.HasAttribute<PredictableAttribute>();

		public bool allowsNull => isSettable && ((type.IsReferenceType() && info.HasAttribute<AllowsNullAttribute>()) || Nullable.GetUnderlyingType(type) != null);

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			// Only save member serialization data if reflection succeeds
			// Otherwise, preserve the old data in case a user restores it after an assembly reload
			try
			{
				EnsureReflected();
			}
			catch (Exception e)
			{
				Debug.LogWarning("OnBeforeSerialize: Failed to ensure reflected.\n" + e.ToString());
				return;
			}

			reflectedTypeName = RuntimeCodebase.SerializeType(reflectedType);
			_targetTypeName = RuntimeCodebase.SerializeType(targetType);
			legacySerializedParameterTypes = null;
			parameterTypeNames = null;
			genericMethodTypeArgumentNames = null;

			// Serialize parameter types as strings, but only if they're all not open-constructed types.
			if (parameterTypes != null && parameterTypes.All(t => !t.ContainsGenericParameters))
			{ 
				parameterTypeNames = new string[parameterTypes.Length];

				for (int i = 0; i < parameterTypeNames.Length; i++)
				{
					var parameterTypeName = RuntimeCodebase.SerializeType(parameterTypes[i]);
					if (parameterTypeName == null)
					{
						Debug.LogWarning("OnBeforeSerialize: SerializeType returned null for parameter type " + i + " of member '" + _targetTypeName + "." + _name + "'");
					}
					parameterTypeNames[i] = parameterTypeName;
				}
			}

			// Serialize generic method argument types
			if (_genericMethodTypeArguments != null)
			{
				genericMethodTypeArgumentNames = new string[_genericMethodTypeArguments.Length];

				// Only serialize the generic method argument types if they're all close-constructed.
				// Otherwise, this list will be null-filled to indicate an open-constructed method with specific method arity.
				if (_genericMethodTypeArguments.All(t => t != null && !t.ContainsGenericParameters))
				{
					for (int i = 0; i < genericMethodTypeArgumentNames.Length; i++)
					{
						genericMethodTypeArgumentNames[i] = RuntimeCodebase.SerializeType(_genericMethodTypeArguments[i]);
					}
				}
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			// Only overwrite the target type name if it successfully loaded
			// If this fails, the target type will be set to null, but the serialized target type name will be preserved (because we use the private field)
			// If it succeeds, the target type will be set to the deserialized type, and the valid serialized target type name will stay unchanged (as it should!)

			if (reflectedTypeName != null && !RuntimeCodebase.TryDeserializeType(reflectedTypeName, out reflectedType))
			{
				Debug.LogWarning($"Failed to deserialize member reflected type:\n{reflectedTypeName}");
			}

			if (!RuntimeCodebase.TryDeserializeType(_targetTypeName, out _targetType))
			{
				Debug.LogWarning($"Failed to deserialize member target type:\n{targetTypeName}");
				return;
			}

			if (genericMethodTypeArgumentNames != null)
			{
				bool failedGenericMethodTypeArguments = false;

				_genericMethodTypeArguments = new Type[genericMethodTypeArgumentNames.Length];

				if (genericMethodTypeArgumentNames.All(t => t != null))
				{
					for (int i = 0; i < _genericMethodTypeArguments.Length; i++)
					{
						var genericMethodTypeArgumentName = genericMethodTypeArgumentNames[i];

						if (!RuntimeCodebase.TryDeserializeType(genericMethodTypeArgumentName, out _genericMethodTypeArguments[i]))
						{
							failedGenericMethodTypeArguments = true;
							Debug.LogWarning($"Failed to deserialize generic method type argument:\n{genericMethodTypeArgumentName}");
						}
					}
				}

				if (failedGenericMethodTypeArguments)
				{
					return;
				}
			}
			
			// Parameter type names will not be included if the method is not closed-constructed.
			if (parameterTypeNames != null)
			{
				bool failedParameterTypes = false;

				// Older versions of 2.0 don't include the 'this' parameter for extension methods.
				// Source is never unknown for new serialized versions, so we can use this to detect whether to add the missing 'this' parameter.
				if (_source == Source.Unknown)
				{
					hasLegacyParameters = true;
				}

				parameterTypes = new Type[parameterTypeNames.Length];

				for (int i = 0; i < parameterTypes.Length; i++)
				{
					var parameterTypeName = parameterTypeNames[i];

					if (!RuntimeCodebase.TryDeserializeType(parameterTypeName, out parameterTypes[i]))
					{
						failedParameterTypes = true;
						Debug.LogWarning($"Failed to deserialize member parameter type:\n{parameterTypeName}");
					}
				}

				if (failedParameterTypes)
				{
					return;
				}
			}
			else if (legacySerializedParameterTypes != null)
			{
				// We're running on legacy data here, make sure to move the data to parameterTypeNames.
				parameterTypes = legacySerializedParameterTypes;
				parameterTypeNames = new string[legacySerializedParameterTypes.Length];
				hasLegacyParameters = true;

				for (int i = 0; i < parameterTypeNames.Length; i++)
				{
					parameterTypeNames[i] = RuntimeCodebase.SerializeType(legacySerializedParameterTypes[i]);
				}

				legacySerializedParameterTypes = null;
			}
		}

		public bool IsGettable(bool nonPublic)
		{
			switch (source)
			{
				case Source.Field:
					return nonPublic || fieldInfo.IsPublic;
				case Source.Property:
				case Source.Indexer:
					return propertyInfo.CanRead && (nonPublic || propertyInfo.GetGetMethod(false) != null);
				case Source.Method:
					return methodInfo.ReturnType != typeof(void) && (nonPublic || methodInfo.IsPublic);
				case Source.Constructor:
					return nonPublic || constructorInfo.IsPublic;
				case Source.Event:
					return false;
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		public bool IsSettable(bool nonPublic)
		{
			switch (source)
			{
				case Source.Field:
					return !(fieldInfo.IsLiteral || fieldInfo.IsInitOnly) && (nonPublic || fieldInfo.IsPublic);
				case Source.Property:
				case Source.Indexer:
					return propertyInfo.CanWrite && (nonPublic || propertyInfo.GetSetMethod(false) != null);
				case Source.Method:
					return false;
				case Source.Constructor:
					return false;
				case Source.Event:
					return false;
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		public bool IsInvocable(bool nonPublic)
		{
			switch (source)
			{
				case Source.Field:
					return false;
				case Source.Property:
					return false;
				case Source.Method:
					return nonPublic || methodInfo.IsPublic;
				case Source.Constructor:
					return nonPublic || constructorInfo.IsPublic;
				case Source.Indexer:
					return false;
				case Source.Event:
					return false;
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		public bool IsHookable(bool nonPublic)
		{
			return isEvent && (nonPublic || eventInfo.IsPublic());
		}

		private void EnsureExplicitParameterTypes()
		{
			if (parameterTypes == null && parameterOpenTypeNames == null)
			{
				throw new InvalidOperationException("Missing parameter types or open-constructed parameter type names.");
			}
		}

		public void Reflect()
		{
			using (ProfilingUtility.SampleBlock("Reflect Member"))
			{
				// Cannot happen from the constructor, but will occur
				// if the type doesn't exist and fails to be deserialized
				if (targetType == null)
				{
					if (targetTypeName != null)
					{
						throw new MissingMemberException(targetTypeName, name);
					}
					else
					{
						throw new MissingMemberException("Target type not found.");
					}
				}

				var oldSource = _source;
				_source = Source.Unknown;

				_fieldInfo = null;
				_propertyInfo = null;
				_methodInfo = null;
				_constructorInfo = null;
				_eventInfo = null;

				fieldAccessor = null;
				propertyAccessor = null;
				methodInvoker = null;
				indexerGetInvoker = null;
				indexerSetInvoker = null;

				// Optimizing member flags depending on whether or not we have parameter types specified
				// Anyhow, ReflectMethod and ReflectConstructor would fail without parameter types
				MemberTypes supportedMemberTypes;

				if (parameterTypes == null && parameterOpenTypeNames == null)
				{
					supportedMemberTypes = MemberTypes.Field | MemberTypes.Property;
				}
				else
				{
					supportedMemberTypes = MemberTypes.Property | MemberTypes.Constructor | MemberTypes.Method;
				}

				var withoutThis = false;
				var candidates = ListPool<MemberInfo>.New();

				try
				{
					if (!hasLegacyParameters && reflectedType != null)
					{
						switch (oldSource)
						{
							case Source.Field:
								candidates.Add(reflectedType.GetField(name, SupportedBindingFlags));
								break;

							case Source.Property:
								candidates.Add(reflectedType.GetPropertyUnambiguous(name, SupportedBindingFlags));
								break;

							case Source.Indexer:
								if (parameterTypes != null)
								{
									candidates.Add(reflectedType.GetPropertyUnambiguous(name, SupportedBindingFlags, parameterTypes));
								}
								else
								{
									foreach (var candidate in reflectedType.GetMember(name, MemberTypes.Property, SupportedBindingFlags))
									{
										candidates.Add(candidate);
									}
								}

								break;

							case Source.Constructor:
								if (parameterTypes != null)
								{
									candidates.Add(reflectedType.GetConstructor(SupportedBindingFlags & ~BindingFlags.Static, null, parameterTypes, null));
								}
								else
								{
									foreach (var candidate in reflectedType.GetMember(name, MemberTypes.Constructor, SupportedBindingFlags & ~BindingFlags.Static))
									{
										candidates.Add(candidate);
									}
								}

								break;

							case Source.Method:
								if (parameterTypes != null && _genericMethodTypeArguments == null)
								{
									try
									{
										candidates.Add(reflectedType.GetMethod(name, SupportedBindingFlags, null, parameterTypes, null));
									}
									catch (AmbiguousMatchException) { }
								}

								if (candidates.Count == 0)
								{
									foreach (var candidate in reflectedType.GetMember(name, MemberTypes.Method, SupportedBindingFlags))
									{
										candidates.Add(candidate);
									}
								}

								break;

							case Source.Event:
								candidates.Add(reflectedType.GetEvent(name, SupportedBindingFlags));
								break;
						}
					}

					if (candidates.Count == 0)
					{
						withoutThis = true;
						
						foreach (var candidate in targetType.GetExtendedMember(name, supportedMemberTypes, SupportedBindingFlags))
						{
							candidates.Add(candidate);
						}
					}

					if (candidates.Count == 0) // Not found, check if it might have been renamed
					{
						withoutThis = true;
						var renamedMembers = RuntimeCodebase.RenamedMembers(targetType);

						string newName;

						if (renamedMembers.TryGetValue(name, out newName))
						{
							_name = newName;
							
							foreach (var candidate in targetType.GetExtendedMember(name, supportedMemberTypes, SupportedBindingFlags))
							{
								candidates.Add(candidate);
							}
						}
					}

					if (candidates.Count == 0) // Nope, not even, abort
					{
						throw new MissingMemberException($"No matching member found: '{targetType.Name}.{name}'");
					}

					MemberTypes? memberType = null;

					foreach (var candidate in candidates)
					{
						if (memberType == null)
						{
							memberType = candidate.MemberType;
						}
						else if (candidate.MemberType != memberType && !candidate.IsExtensionMethod())
						{
							// This theoretically shouldn't happen according to the .NET specification, I believe
							Debug.LogWarning($"Multiple members with the same name are of a different type: '{targetType.Name}.{name}'");
							break;
						}
					}

					switch (memberType)
					{
						case MemberTypes.Field:
							ReflectField(candidates);
							break;

						case MemberTypes.Property:
							ReflectProperty(candidates);
							break;

						case MemberTypes.Method:
							ReflectMethod(candidates, withoutThis);
							break;

						case MemberTypes.Constructor:
							ReflectConstructor(candidates, withoutThis);
							break;

						case MemberTypes.Event:
							ReflectEvent(candidates);
							break;

						default:
							throw new UnexpectedEnumValueException<MemberTypes>(memberType.Value);
					}

					_isReflected = true;
				}
				finally
				{
					candidates.Free();
				}
			}
		}

		private void ReflectField(List<MemberInfo> candidates)
		{
			_fieldInfo = candidates.OfType<FieldInfo>().Disambiguate(targetType);

			if (_fieldInfo == null)
			{
				throw new MissingMemberException($"No matching field found: '{targetType.Name}.{name}'");
			}

			_source = Source.Field;
			reflectedType = _fieldInfo.ReflectedType;
		}

		private void ReflectProperty(List<MemberInfo> candidates)
		{
			if (parameterTypes != null)
			{
				_propertyInfo = candidates.OfType<PropertyInfo>().Disambiguate(targetType, parameterTypes);
			}
			else if (parameterOpenTypeNames != null)
			{
				_propertyInfo = candidates.OfType<PropertyInfo>().Disambiguate(targetType, parameterOpenTypeNames);
			}
			else
			{
				_propertyInfo = candidates.OfType<PropertyInfo>().Disambiguate(targetType);
			}

			if (_propertyInfo == null)
			{
				if ((parameterTypes != null && parameterTypes.Length > 0) || (parameterOpenTypeNames != null && parameterOpenTypeNames.Length > 0))
				{
					throw new MissingMemberException($"No matching indexer found: '{targetType.Name} [{(parameterOpenTypeNames != null ? parameterOpenTypeNames : parameterTypes.Select(t => t.Name)).ToCommaSeparatedString()}]'");
				}
				else
				{
					throw new MissingMemberException($"No matching property found: '{targetType.Name}.{name}'");
				}
			}

			_source = _propertyInfo.IsIndexer() ? Source.Indexer : Source.Property;
			reflectedType = _propertyInfo.ReflectedType;
		}

		private void ReflectConstructor(List<MemberInfo> candidates, bool withoutThis)
		{
			EnsureExplicitParameterTypes();
			
			// Exclude static constructors (type initializers) because calling them
			// is always a violation of types expecting it to be called only once.
			// http://stackoverflow.com/a/2524938

			if (parameterTypes != null)
			{
				_constructorInfo = candidates.OfType<ConstructorInfo>().Where(c => !c.IsStatic).Disambiguate(targetType, parameterTypes, withoutThis, hasLegacyParameters);
			}
			else
			{
				_constructorInfo = candidates.OfType<ConstructorInfo>().Where(c => !c.IsStatic).Disambiguate(targetType, parameterOpenTypeNames, withoutThis);
			}

			if (_constructorInfo == null)
			{
				throw new MissingMemberException($"No matching constructor found: '{targetType.Name} ({(parameterOpenTypeNames != null ? parameterOpenTypeNames : parameterTypes.Select(t => t.Name)).ToCommaSeparatedString()})'");
			}

			_source = Source.Constructor;
			reflectedType = _constructorInfo.ReflectedType;

			if (parameterTypes == null || hasLegacyParameters)
			{
				parameterTypes = _constructorInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
				hasLegacyParameters = false;
			}

			if (parameterOpenTypeNames == null)
			{
				parameterOpenTypeNames = _constructorInfo.GetParameters().Select(pi => pi.ParameterType.ToString()).ToArray();
			}
		}

		private void ReflectMethod(List<MemberInfo> candidates, bool withoutThis)
		{
			EnsureExplicitParameterTypes();

			if (parameterTypes != null && _genericMethodTypeArguments == null)
			{
				_methodInfo = candidates.OfType<MethodInfo>().Disambiguate(targetType, parameterTypes, withoutThis, hasLegacyParameters);
			}
			else
			{
				_methodInfo = candidates.OfType<MethodInfo>().Disambiguate(targetType, parameterOpenTypeNames, _genericMethodTypeArguments?.Length ?? 0, withoutThis);
			}
			
			if (_methodInfo == null)
			{
				throw new MissingMemberException($"No matching method found: '{targetType.Name}.{name} ({(parameterOpenTypeNames ?? parameterTypes.Select(t => t.Name)).ToCommaSeparatedString()}))'");
			}

			_source = Source.Method;
			_isExtension = _methodInfo.IsExtension();
			reflectedType = _methodInfo.ReflectedType;

			if (parameterTypes == null || hasLegacyParameters)
			{
				parameterTypes = _methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
				hasLegacyParameters = false;
			}

			if (_methodInfo.IsGenericMethod)
			{
				var genericMethodDefinition = _methodInfo.GetGenericMethodDefinition();

				if (_genericMethodTypeArguments != null && _genericMethodTypeArguments.All(t => t != null && !t.ContainsGenericParameters))
				{
					_methodInfo = genericMethodDefinition.MakeGenericMethod(_genericMethodTypeArguments);
				}

				if (_genericMethodTypeArguments == null)
				{
					_genericMethodTypeArguments = new Type[genericMethodDefinition.GetGenericArguments().Length];
				}
			}

			if (parameterOpenTypeNames == null)
			{
				parameterOpenTypeNames = (_methodInfo.IsGenericMethod ? _methodInfo.GetGenericMethodDefinition() : _methodInfo).GetParameters().Select(pi => pi.ParameterType.ToString()).ToArray();
			}
		}

		private void ReflectEvent(List<MemberInfo> candidates)
		{
			_eventInfo = candidates.OfType<EventInfo>().Disambiguate(targetType);

			if (_eventInfo == null)
			{
				throw new MissingMemberException($"No matching event found: '{targetType.Name}.{name}'");
			}

			_source = Source.Event;
			reflectedType = _eventInfo.ReflectedType;
		}

		public void Prewarm()
		{
			fieldAccessor = fieldInfo?.Prewarm();
			propertyAccessor = propertyInfo?.Prewarm();
			methodInvoker = methodInfo?.Prewarm();
			indexerGetInvoker = propertyInfo?.GetGetMethod(true)?.Prewarm();
			indexerSetInvoker = propertyInfo?.GetSetMethod(true)?.Prewarm();
		}

		public void EnsureReflected()
		{
			if (!isReflected)
			{
				Reflect();
			}
		}

		public void EnsureReady(object target)
		{
			EnsureReflected();

			if (target == null && requiresTarget)
			{
				throw new InvalidOperationException($"Missing target object for '{targetType}.{name}'.");
			}
			else if (target != null && !requiresTarget)
			{
				throw new InvalidOperationException($"Superfluous target object for '{targetType}.{name}'.");
			}
		}

		public object Get(object target)
		{
			EnsureReady(target);

			switch (source)
			{
				case Source.Field:
					if (fieldAccessor == null)
					{
						fieldAccessor = fieldInfo.Prewarm();
					}

					return fieldAccessor.GetValue(target);

				case Source.Property:
					if (propertyAccessor == null)
					{
						propertyAccessor = propertyInfo.Prewarm();
					}

					return propertyAccessor.GetValue(target);				

				case Source.Method:
					throw new NotSupportedException("Member is a method. Consider using 'Invoke' instead.");
				case Source.Constructor:
					throw new NotSupportedException("Member is a constructor. Consider using 'Invoke' instead.");
				case Source.Indexer:
					throw new NotSupportedException("Member is an indexer. Consider using 'GetItem' instead.");
				case Source.Event:
					throw new NotSupportedException("Member is an event.");
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		public T Get<T>(object target)
		{
			return (T)Get(target);
		}

		public object Set(object target, object value)
		{
			EnsureReady(target);

			// When setting, we return the assigned value, not the updated field or property.
			// This is consistent with C# language behaviour: https://msdn.microsoft.com/en-us/library/sbkb459w.aspx
			// "The assignment operator (=) [...] returns the value as its result"
			// See confirmation here: https://dotnetfiddle.net/n4RZcW

			switch (source)
			{
				case Source.Field:
					if (fieldAccessor == null)
					{
						fieldAccessor = fieldInfo.Prewarm();
					}

					fieldAccessor.SetValue(target, value);
					return value;

				case Source.Property:
					if (propertyAccessor == null)
					{
						propertyAccessor = propertyInfo.Prewarm();
					}

					propertyAccessor.SetValue(target, value);
					return value;

				case Source.Method:
					throw new NotSupportedException("Member is a method.");
				case Source.Constructor:
					throw new NotSupportedException("Member is a constructor.");
				case Source.Indexer:
					throw new NotSupportedException("Member is an indexer. Consider using 'SetItem' instead.");
				case Source.Event:
					throw new NotSupportedException("Member is an event.");
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		private void EnsureInvocable(object target)
		{
			EnsureReady(target);

			switch (source)
			{
				case Source.Field:
					throw new NotSupportedException("Member is a field.");
				case Source.Property:
					throw new NotSupportedException("Member is a property.");
				case Source.Method:
					{
						if (methodInfo.ContainsGenericParameters)
						{
							throw new NotSupportedException($"Trying to invoke an open-constructed generic method: '{methodInfo}'.");
						}

						if (methodInvoker == null)
						{
							methodInvoker = methodInfo.Prewarm();
						}
						break;
					}
				case Source.Constructor:
				{
					if (constructorInfo.ContainsGenericParameters)
					{
						throw new NotSupportedException($"Trying to invoke an open-constructed generic constructor: '{constructorInfo}'.");
					}
					break;
				}
				case Source.Indexer:
					throw new NotSupportedException("Member is an indexer.");
				case Source.Event:
					throw new NotSupportedException("Member is an event.");
				default:
					throw new UnexpectedEnumValueException<Source>(source);
			}
		}

		public object Invoke(object target)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target);
				}
				else
				{
					return methodInvoker.Invoke(target);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(EmptyObjects);
			}
		}

		public object Invoke(object target, object arg0)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target, arg0);
				}
				else
				{
					return methodInvoker.Invoke(target, arg0);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(new[] { arg0 });
			}
		}

		public object Invoke(object target, object arg0, object arg1)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target, arg0, arg1);
				}
				else
				{
					return methodInvoker.Invoke(target, arg0, arg1);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(new[] { arg0, arg1 });
			}
		}

		public object Invoke(object target, object arg0, object arg1, object arg2)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target, arg0, arg1, arg2);
				}
				else
				{
					return methodInvoker.Invoke(target, arg0, arg1, arg2);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(new[] { arg0, arg1, arg2 });
			}
		}

		public object Invoke(object target, object arg0, object arg1, object arg2, object arg3)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target, arg0, arg1, arg2, arg3);
				}
				else
				{
					return methodInvoker.Invoke(target, arg0, arg1, arg2, arg3);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(new[] { arg0, arg1, arg2, arg3 });
			}
		}

		public object Invoke(object target, object arg0, object arg1, object arg2, object arg3, object arg4)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					return methodInvoker.Invoke(null, target, arg0, arg1, arg2, arg3, arg4);
				}
				else
				{
					return methodInvoker.Invoke(target, arg0, arg1, arg2, arg3, arg4);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(new[] { arg0, arg1, arg2, arg3, arg4 });
			}
		}

		public object Invoke(object target, params object[] arguments)
		{
			EnsureInvocable(target);

			if (source == Source.Method)
			{
				if (isExtension)
				{
					var argumentsWithThis = new object[arguments.Length + 1];
					argumentsWithThis[0] = target;
					Array.Copy(arguments, 0, argumentsWithThis, 1, arguments.Length);
					return methodInvoker.Invoke(null, argumentsWithThis);
				}
				else
				{
					return methodInvoker.Invoke(target, arguments);
				}
			}
			else // if (source == Source.Constructor)
			{
				return constructorInfo.Invoke(arguments);
			}
		}

		public T Invoke<T>(object target)
		{
			return (T)Invoke(target);
		}

		public T Invoke<T>(object target, object arg0)
		{
			return (T)Invoke(target, arg0);
		}

		public T Invoke<T>(object target, object arg0, object arg1)
		{
			return (T)Invoke(target, arg0, arg1);
		}

		public T Invoke<T>(object target, object arg0, object arg1, object arg2)
		{
			return (T)Invoke(target, arg0, arg1, arg2);
		}

		public T Invoke<T>(object target, object arg0, object arg1, object arg2, object arg3)
		{
			return (T)Invoke(target, arg0, arg1, arg2, arg3);
		}

		public T Invoke<T>(object target, object arg0, object arg1, object arg2, object arg3, object arg4)
		{
			return (T)Invoke(target, arg0, arg1, arg2, arg3, arg4);
		}

		public T Invoke<T>(object target, params object[] arguments)
		{
			return (T)Invoke(target, arguments);
		}

        private void EnsureIndexerGetReady(object target)
        {
			EnsureReady(target);

            switch (source)
            {
                case Source.Field:
                    throw new NotSupportedException("Member is a field. Consider using 'Get' instead");
                case Source.Property:
                    throw new NotSupportedException("Member is a property. Consider using 'Get' instead");
                case Source.Method:
                    throw new NotSupportedException("Member is a method.");
                case Source.Constructor:
                    throw new NotSupportedException("Member is a constructor.");
                case Source.Indexer:
                {
                    if (indexerGetInvoker == null)
                    {
                        indexerGetInvoker = propertyInfo.GetGetMethod(true).Prewarm();
                    }

                    break;
                }
				case Source.Event:
					throw new NotSupportedException("Member is an event.");
                default:
                    throw new UnexpectedEnumValueException<Source>(source);
            }
        }

        public object GetItem(object target, object arg0)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arg0);
        }

        public object GetItem(object target, object arg0, object arg1)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arg0, arg1);
        }

        public object GetItem(object target, object arg0, object arg1, object arg2)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arg0, arg1, arg2);
        }

        public object GetItem(object target, object arg0, object arg1, object arg2, object arg3)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arg0, arg1, arg2, arg3);
        }

        public object GetItem(object target, object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arg0, arg1, arg2, arg3, arg4);
        }

        public object GetItem(object target, params object[] arguments)
        {
            EnsureIndexerGetReady(target);
            return indexerGetInvoker.Invoke(target, arguments);
        }

        public T GetItem<T>(object target, object arg0)
        {
            return (T)GetItem(target, arg0);
        }

        public T GetItem<T>(object target, object arg0, object arg1)
        {
            return (T)GetItem(target, arg0, arg1);
        }

        public T GetItem<T>(object target, object arg0, object arg1, object arg2)
        {
            return (T)GetItem(target, arg0, arg1, arg2);
        }

        public T GetItem<T>(object target, object arg0, object arg1, object arg2, object arg3)
        {
            return (T)GetItem(target, arg0, arg1, arg2, arg3);
        }

        public T GetItem<T>(object target, object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return (T)GetItem(target, arg0, arg1, arg2, arg3, arg4);
        }

        public T GetItem<T>(object target, params object[] arguments)
        {
            return (T)GetItem(target, arguments);
        }

        private void EnsureIndexerSetReady(object target)
        {
			EnsureReady(target);

            switch (source)
            {
                case Source.Field:
                    throw new NotSupportedException("Member is a field. Consider using 'Set' instead");
                case Source.Property:
                    throw new NotSupportedException("Member is a property. Consider using 'Set' instead");
                case Source.Method:
                    throw new NotSupportedException("Member is a method.");
                case Source.Constructor:
                    throw new NotSupportedException("Member is a constructor.");
                case Source.Indexer:
                {
                    if (indexerSetInvoker == null)
                    {
                        indexerSetInvoker = propertyInfo.GetSetMethod(true).Prewarm();
                    }

                    break;
                }
				case Source.Event:
					throw new NotSupportedException("Member is an event.");
                default:
                    throw new UnexpectedEnumValueException<Source>(source);
            }
        }

        public object SetItem(object target, object arg0, object value)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arg0, value);
        }

        public object SetItem(object target, object arg0, object arg1, object value)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arg0, arg1, value);
        }

        public object SetItem(object target, object arg0, object arg1, object arg2, object value)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arg0, arg1, arg2, value);
        }

        public object SetItem(object target, object arg0, object arg1, object arg2, object arg3, object value)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arg0, arg1, arg2, arg3, value);
        }

        public object SetItem(object target, object arg0, object arg1, object arg2, object arg3, object arg4, object value)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arg0, arg1, arg2, arg3, arg4, value);
        }

        public object SetItem(object target, params object[] arguments)
        {
            EnsureIndexerSetReady(target);
            return indexerSetInvoker.Invoke(target, arguments);
        }

        public T SetItem<T>(object target, object arg0, object value)
        {
            return (T)SetItem(target, arg0, value);
        }

        public T SetItem<T>(object target, object arg0, object arg1, object value)
        {
            return (T)SetItem(target, arg0, arg1, value);
        }

        public T SetItem<T>(object target, object arg0, object arg1, object arg2, object value)
        {
            return (T)SetItem(target, arg0, arg1, arg2, value);
        }

        public T SetItem<T>(object target, object arg0, object arg1, object arg2, object arg3, object value)
        {
            return (T)SetItem(target, arg0, arg1, arg2, arg3, value);
        }

        public T SetItem<T>(object target, object arg0, object arg1, object arg2, object arg3, object arg4, object value)
        {
            return (T)SetItem(target, arg0, arg1, arg2, arg3, arg4, value);
        }

        public T SetItem<T>(object target, params object[] arguments)
        {
            return (T)SetItem(target, arguments);
        }

        private void EnsureHookable(object target)
        {
	        EnsureReady(target);

	        if (!isEvent)
	        {
		        throw new NotSupportedException("Member must be an event.");
	        }
        }

        public void AddEventHandler(object target, Delegate handler)
        {
	        EnsureHookable(target);
	        eventInfo.AddEventHandler(target, handler);
        }

        public void RemoveEventHandler(object target, Delegate handler)
        {
	        EnsureHookable(target);
	        eventInfo.RemoveEventHandler(target, handler);
        }

		public override bool Equals(object obj)
		{
			var other = obj as Member;

			var equals = other != null &&
						 targetType == other.targetType &&
						 name == other.name;

			if (!equals)
			{
				return false;
			}
			
			var selfHasParameters = parameterTypes != null;
			var otherHasParameters = other.parameterTypes != null;

			if (selfHasParameters != otherHasParameters)
			{
				return false;
			}

			if (selfHasParameters /* && otherHasParameters */)
			{
				var selfCount = parameterTypes.Length;
				var otherCount = other.parameterTypes.Length;

				if (selfCount != otherCount)
				{
					return false;
				}

				for (var i = 0; i < selfCount; i++)
				{
					if (parameterTypes[i] != other.parameterTypes[i])
					{
						return false;
					}
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 17;

				hash = hash * 23 + (targetType?.GetHashCode() ?? 0);
				hash = hash * 23 + (name?.GetHashCode() ?? 0);

				if (parameterTypes != null)
				{
					foreach (var parameterType in parameterTypes)
					{
						hash = hash * 23 + parameterType.GetHashCode();
					}
				}
				else
				{
					hash = hash * 23 + 0;
				}

				return hash;
			}
		}

		public static bool operator ==(Member a, Member b)
		{
			if (ReferenceEquals(a, b))
			{
				return true;
			}

			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
			{
				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Member a, Member b)
		{
			return !(a == b);
		}

		public string ToUniqueString()
		{
			var s = targetType.FullName + "." + this.name;
			
			if (parameterTypes != null)
			{
				s += "(";

				foreach (var parameterType in parameterTypes)
				{
					s += parameterType.FullName;
				}

				s += ")";
			}
			else if (parameterOpenTypeNames != null)
			{
				s += "(";

				foreach (var parameterOpenTypeName in parameterOpenTypeNames)
				{
					s += parameterOpenTypeName;
				}

				s += ")";
			}

			return s;
		}

		public override string ToString()
		{
			return $"{targetType.CSharpName()}.{name}";
		}

		public Member ToDeclarer()
		{
			return WithTargetType(declaringType);
		}

		public Member ToPseudoDeclarer()
		{
			return WithTargetType(pseudoDeclaringType);
		}

		public Member WithTargetType(Type targetType)
		{
			if (targetType == this.targetType)
			{
				return this;
			}

			if (isExtension)
			{
				return targetType.GetExtensionMethods().Single(mi => mi.ReflectedType == info.ReflectedType && mi.MetadataToken == info.MetadataToken).ToMember(targetType);
			}
			else
			{
				if (isVirtual && (targetType.IsSubclassOf(this.targetType) || this.targetType.IsSubclassOf(targetType)))
				{
					switch (_source)
					{
						case Source.Method:
						{
							var baseDefinition = _methodInfo.GetBaseDefinition();
							return targetType.GetMember(name, SupportedBindingFlags).OfType<MethodInfo>().Single(mi => mi.GetBaseDefinition() == baseDefinition).ToMember(targetType);
						}
						case Source.Property:
						case Source.Indexer:
						{
							var baseDefinition = _propertyInfo.GetIndicativeMethod().GetBaseDefinition();
							return targetType.GetMember(name, SupportedBindingFlags).OfType<PropertyInfo>().Single(pi => pi.GetIndicativeMethod().GetBaseDefinition() == baseDefinition).ToMember(targetType);
						}
						case Source.Event:
						{
							var baseDefinition = _eventInfo.GetIndicativeMethod().GetBaseDefinition();
							return targetType.GetMember(name, SupportedBindingFlags).OfType<EventInfo>().Single(mi => mi.GetIndicativeMethod().GetBaseDefinition() == baseDefinition).ToMember(targetType);
						}
						default: throw new UnexpectedEnumValueException<Source>(source);
					}
				}
				else
				{
					return targetType.GetMember(name, SupportedBindingFlags).Single(mi => mi.MetadataToken == info.MetadataToken).ToMember(targetType);
				}
			}
		}

		public Member WithGenericMethodTypeArguments(Type[] genericMethodTypeArguments)
		{
			if (!isGenericMethod)
			{
				throw new InvalidOperationException($"WithGenericMethodTypeArguments: member '{targetType}.{name}' is not a generic method so it cannot be passed generic argument.");
			}

			if (genericMethodTypeArguments.All(t => t != null && !t.ContainsGenericParameters))
			{
				return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(genericMethodTypeArguments).ToMember(targetType);
			}
			else
			{
				return methodInfo.GetGenericMethodDefinition().ToMember(targetType);
			}
		}
		
		public MemberData ToData()
		{
			((ISerializationCallbackReceiver)this).OnBeforeSerialize();
			return new MemberData() {
				source = _source,
				reflectedType = RuntimeCodebase.SerializeTypeData(reflectedType, reflectedTypeName),
				targetType = RuntimeCodebase.SerializeTypeData(targetType, _targetTypeName),
				name = _name,
				isExtension = _isExtension,
				parameterTypeNames = parameterTypeNames,
				parameterOpenTypeNames = parameterOpenTypeNames,
				genericMethodTypeArgumentNames = genericMethodTypeArgumentNames
			};
		}
		
		public const MemberTypes SupportedMemberTypes = MemberTypes.Property | MemberTypes.Field | MemberTypes.Method | MemberTypes.Constructor | MemberTypes.Event;

		public const BindingFlags SupportedBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

		private static readonly object[] EmptyObjects = new object[0];
	}
}