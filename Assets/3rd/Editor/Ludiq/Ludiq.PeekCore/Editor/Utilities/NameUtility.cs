using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityObject = UnityEngine.Object;

namespace Ludiq.PeekCore
{
	public static class NameUtility
	{
		private static readonly Dictionary<Type, string> humanPrimitiveNames = new Dictionary<Type, string>
		{
			{ typeof(byte), "Byte" },
			{ typeof(sbyte), "Signed Byte" },
			{ typeof(short), "Short" },
			{ typeof(ushort), "Unsigned Short" },
			{ typeof(int), "Integer" },
			{ typeof(uint), "Unsigned Integer" },
			{ typeof(long), "Long" },
			{ typeof(ulong), "Unsigned Long" },
			{ typeof(float), "Float" },
			{ typeof(double), "Double" },
			{ typeof(decimal), "Decimal" },
			{ typeof(string), "String" },
			{ typeof(char), "Character" },
			{ typeof(bool), "Boolean" },
			{ typeof(void), "Void" },
			{ typeof(object), "Object" },
		};

		private static readonly HashSet<string> booleanVerbs = new HashSet<string>
		{
			"Is",
			"Can",
			"Has",
			"Are",
			"Will",
			"Was",
			"Had",
			"Were"
		};

		public static string SelectedName(this Type type, bool human)
		{
			return human ? type.HumanName() : type.CSharpName();
		}

		public static string SelectedName(this MemberInfo member, bool human, MemberAction action = MemberAction.None, bool expectingBoolean = false)
		{
			return human ? member.HumanName(action) : member.CSharpName(action);
		}

		public static string SelectedName(this ParameterInfo parameter, bool human)
		{
			return human ? parameter.HumanName() : parameter.Name;
		}

		public static string SelectedName(this Exception exception, bool human)
		{
			return human ? exception.HumanName() : exception.GetType().CSharpName(false);
		}

		public static string SelectedName(this Enum @enum, bool human)
		{
			return human ? HumanName(@enum) : @enum.ToString();
		}

		public static string SelectedName(this Namespace @namespace, bool human, bool full = true)
		{
			return human ? @namespace.HumanName(full) : @namespace.CSharpName(full);
		}

		public static string SelectedParameterString(this MethodBase methodBase, bool human)
		{
			return string.Join(", ", methodBase.GetParametersWithoutThis().Select(p => p.SelectedName(human)).ToArray());
		}

		public static string SelectedParameterString(this PropertyInfo propertyInfo, bool human)
		{
			return string.Join(", ", propertyInfo.GetIndexParameters().Select(p => p.SelectedName(human)).ToArray());
		}

		public static string DisplayName(this Type type)
		{
			return SelectedName(type, LudiqCore.Configuration.humanNaming);
		}

		public static string DisplayName(this MemberInfo member, MemberAction action = MemberAction.None, bool expectingBoolean = false)
		{
			return SelectedName(member, LudiqCore.Configuration.humanNaming, action, expectingBoolean);
		}

		public static string DisplayName(this ParameterInfo parameter)
		{
			return SelectedName(parameter, LudiqCore.Configuration.humanNaming);
		}

		public static string DisplayName(this Exception exception)
		{
			return SelectedName(exception, LudiqCore.Configuration.humanNaming);
		}

		public static string DisplayName(this Enum @enum)
		{
			return SelectedName(@enum, LudiqCore.Configuration.humanNaming);
		}

		public static string DisplayName(this Namespace @namespace, bool full = true)
		{
			return SelectedName(@namespace, LudiqCore.Configuration.humanNaming, full);
		}

		public static string DisplayParameterString(this MethodBase methodBase)
		{
			return SelectedParameterString(methodBase, LudiqCore.Configuration.humanNaming);
		}

		public static string DisplayParameterString(this PropertyInfo propertyInfo)
		{
			return SelectedParameterString(propertyInfo, LudiqCore.Configuration.humanNaming);
		}

		public static string HumanName(this Type type)
		{
			if (type == typeof(UnityObject))
			{
				return "Unity Object";
			}

			if (humanPrimitiveNames.ContainsKey(type))
			{
				return humanPrimitiveNames[type];
			}
			else if (type.IsGenericParameter)
			{
				var genericParameterName = type.Name.Prettify();

				if (genericParameterName == "T")
				{
					return "Generic";
				}
				else if (genericParameterName.StartsWith("T "))
				{
					return genericParameterName.Substring(2).Prettify();
				}
				else
				{
					return genericParameterName.Prettify();
				}
			}
			else if (type.IsGenericType && !type.ContainsGenericParameters && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var nonNullable = Nullable.GetUnderlyingType(type);

				var underlyingName = nonNullable.HumanName();

				return "Nullable " + underlyingName;
			}
			else
			{
				var name = type.Name.Prettify();

				if (type.IsGenericType && name.Contains('`'))
				{
					name = name.Substring(0, name.IndexOf('`'));
				}

				if (type.IsInterface && name.StartsWith("I "))
				{
					name = name.Substring(2) + " Interface";
				}

				if (type.IsArray && name.Contains("[]"))
				{
					name = name.Replace("[]", " Array");
				}

				if (!type.ContainsGenericParameters)
				{
					var genericArguments = type.GetGenericArguments();
					var genericStartOffset = 0;

					if (type.IsNested)
					{
						if (type.DeclaringType.IsGenericType)
						{
							genericStartOffset += type.DeclaringType.GetGenericArguments().Length;
						}
					}

					if (genericStartOffset < genericArguments.Length)
					{
						name += " <"
							+ string.Join(", ", genericArguments.Skip(genericStartOffset).Select(t => t.HumanName()).ToArray())
							+ ">";
					}

					if (type.IsNested)
					{
						name += " in " + type.DeclaringType.HumanName();
					}
				}

				return name;
			}
		}

		public static string Verb(this MemberAction action)
		{
			return action.ToString();
		}

		public static string HumanName(this MemberInfo member, MemberAction action = MemberAction.None, bool expectingBoolean = false)
		{
			var words = member.Name.Prettify();

			if (member is MethodInfo methodInfo)
			{
				if (methodInfo.IsOperator())
				{
					return OperatorUtility.operatorHumanNames[methodInfo.Name];
				}
				else
				{
					return words;
				}
			}
			else if (member is FieldInfo || member is PropertyInfo)
			{
				if (action == MemberAction.None)
				{
					return words;
				}

				var type = member is FieldInfo ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType;

				// Fix for Unity's object-to-boolean implicit null-check operators
				if (action == MemberAction.Get && typeof(UnityObject).IsAssignableFrom(type) && expectingBoolean)
				{
					return words + " Is Not Null";
				}

				var verb = Verb(action);

				if (type == typeof(bool))
				{
					// Check for boolean verbs like IsReady, HasChildren, etc.
					if (words.Contains(' ') && booleanVerbs.Contains(words.Split(' ')[0]))
					{
						// Return them as-is for gets
						if (action == MemberAction.Get)
						{
							return words;
						}
						// Skip them for sets
						else if (action == MemberAction.Set)
						{
							return verb + " " + words.Substring(words.IndexOf(' ') + 1);
						}
						else
						{
							throw new UnexpectedEnumValueException<MemberAction>(action);
						}
					}
					else
					{
						return verb + " " + words;
					}
				}
				else if (action == MemberAction.Get && member.IsStatic() && !member.IsExtensionMethod())
				{
					return words;
				}
				// Otherwise, add get/set the verb prefix
				else
				{
					return verb + " " + words;
				}
			}
			else if (member is ConstructorInfo)
			{
				return "Create " + member.DeclaringType.HumanName();
			}
			else if (member is EventInfo)
			{
				return words;
			}
			else
			{
				throw new UnexpectedEnumValueException<MemberAction>(action);
			}
		}

		public static string HumanName(this ParameterInfo parameter)
		{
			return parameter.Name.Prettify();
		}

		public static string HumanName(this Exception exception)
		{
			return exception.GetType().CSharpName(false).Prettify().Replace(" Exception", "");
		}

		public static string HumanName(this Enum @enum)
		{
			return @enum.ToString().Prettify();
		}

		public static string HumanName(this Namespace @namespace, bool full = true)
		{
			return @namespace.IsGlobal ? "(Global Namespace)" : (full ? string.Join("/", @namespace.Parts.Select(p => p.Prettify()).ToArray()) : @namespace.Name.Prettify());
		}

		public static string ToSummaryString(this Exception ex)
		{
			return $"{ex.GetType().DisplayName()}: {ex.Message}";
		}
	}
}