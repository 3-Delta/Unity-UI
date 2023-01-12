using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ludiq.PeekCore
{
	public static class CSharpNameUtility
	{
		private static readonly Dictionary<Type, string> primitives = new Dictionary<Type, string>
		{
			{ typeof(byte), "byte" },
			{ typeof(sbyte), "sbyte" },
			{ typeof(short), "short" },
			{ typeof(ushort), "ushort" },
			{ typeof(int), "int" },
			{ typeof(uint), "uint" },
			{ typeof(long), "long" },
			{ typeof(ulong), "ulong" },
			{ typeof(float), "float" },
			{ typeof(double), "double" },
			{ typeof(decimal), "decimal" },
			{ typeof(string), "string" },
			{ typeof(char), "char" },
			{ typeof(bool), "bool" },
			{ typeof(void), "void" },
			{ typeof(object), "object" },
		};

		private static readonly HashSet<char> illegalTypeFileNameCharacters = new HashSet<char>()
		{
			'<',
			'>',
			'?',
			' ',
			',',
			':',
		};

		public static string CSharpName(this MemberInfo member, MemberAction action)
		{
			if (member is MethodInfo && ((MethodInfo)member).IsOperator())
			{
				return OperatorUtility.operatorAlternativeNames[member.Name];
			}

			if (member is ConstructorInfo)
			{
				return "new " + member.DeclaringType.CSharpName();
			}

			var memberName = member.Name;

			if (member is PropertyInfo propertyInfo && propertyInfo.IsIndexer())
			{
				memberName = "[]";
			}

			if ((member is FieldInfo || member is PropertyInfo) && action != MemberAction.None)
			{
				return $"{memberName} ({action.ToString().ToLower()})";
			}

			return memberName;
		}

		public static string CSharpName(this Type type, bool includeGenericParameters = true)
		{
			return type.CSharpName(TypeQualifier.Name, includeGenericParameters);
		}

		public static string CSharpFullName(this Type type, bool includeGenericParameters = true)
		{
			return type.CSharpName(TypeQualifier.Namespace, includeGenericParameters);
		}

		public static string CSharpUniqueName(this Type type, bool includeGenericParameters = true)
		{
			return type.CSharpName(TypeQualifier.GlobalNamespace, includeGenericParameters);
		}

		public static string CSharpFileName(this Type type, bool includeNamespace, bool includeGenericParameters = false)
		{
			var fileName = type.CSharpName(includeNamespace ? TypeQualifier.Namespace : TypeQualifier.Name, includeGenericParameters);

			if (!includeGenericParameters && type.IsGenericType && fileName.Contains('<'))
			{
				fileName = fileName.Substring(0, fileName.IndexOf('<'));
			}

			fileName = fileName.ReplaceMultiple(illegalTypeFileNameCharacters, '_')
							   .Trim('_')
							   .RemoveConsecutiveCharacters('_');

			return fileName;
		}

		private static string CSharpName(this Type type, TypeQualifier qualifier, bool includeGenericParameters = true)
		{
			if (primitives.ContainsKey(type))
			{
				return primitives[type];
			}
			else if (type.IsGenericParameter)
			{
				return includeGenericParameters ? type.Name : "";
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var nonNullable = Nullable.GetUnderlyingType(type);

				if (nonNullable == null)
				{
					nonNullable = type.GetGenericArguments()[0];
				}

				var underlyingName = nonNullable.CSharpName(qualifier, includeGenericParameters);

				return underlyingName + "?";
			}
			else
			{
				var name = type.Name;

				if (type.IsGenericType && name.Contains('`'))
				{
					name = name.Substring(0, name.IndexOf('`'));
				}

				var genericArguments = type.GetGenericArguments();
				var genericStartOffset = 0;

				if (type.IsNested)
				{
					name = type.DeclaringType.CSharpName(qualifier, includeGenericParameters) + "." + name;

					if (type.DeclaringType.IsGenericType)
					{
						genericStartOffset += type.DeclaringType.GetGenericArguments().Length;
					}
				}

				if (!type.IsNested)
				{
					if ((qualifier == TypeQualifier.Namespace || qualifier == TypeQualifier.GlobalNamespace) && type.Namespace != null)
					{
						name = type.Namespace + "." + name;
					}

					if (qualifier == TypeQualifier.GlobalNamespace)
					{
						name = "global::" + name;
					}
				}

				if (genericStartOffset < genericArguments.Length)
				{
					name += "<";
					name += string.Join(includeGenericParameters ? ", " : ",", genericArguments.Skip(genericStartOffset).Select(t => t.CSharpName(qualifier, includeGenericParameters)).ToArray());
					name += ">";
				}

				return name;
			}
		}

		public static string CSharpName(this Namespace @namespace, bool full = true)
		{
			return @namespace.IsGlobal ? "(global)" : (full ? @namespace.FullName : @namespace.Name);
		}
	}
}