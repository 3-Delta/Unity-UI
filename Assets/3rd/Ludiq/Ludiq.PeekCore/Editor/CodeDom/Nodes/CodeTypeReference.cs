// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class CodeTypeReference : CodeElement
	{
		static CodeTypeReference()
		{
			TypesByNames = new Dictionary<string, List<CodePredeclaredType>>();

			foreach (var type in Codebase.types)
			{
				var predeclaredType = new CodePredeclaredType(type);

				if (!TypesByNames.TryGetValue(predeclaredType.TypeName, out var typesByName))
				{
					typesByName = new List<CodePredeclaredType>();
					TypesByNames[predeclaredType.TypeName] = typesByName;
				}

				typesByName.Add(predeclaredType);
			}
		}

		private static Dictionary<string, List<CodePredeclaredType>> TypesByNames { get; }

		private static readonly Dictionary<string, string> SimplifiedBuiltinTypeNames = new Dictionary<string, string>
		{
			{"System.SByte", "sbyte"},
			{"System.Int16", "short"},
			{"System.Int32", "int"},
			{"System.Int64", "long"},
			{"System.String", "string"},
			{"System.Object", "object"},
			{"System.Boolean", "bool"},
			{"System.Void", "void"},
			{"System.Char", "char"},
			{"System.Byte", "byte"},
			{"System.UInt16", "ushort"},
			{"System.UInt32", "uint"},
			{"System.UInt64", "ulong"},
			{"System.Single", "float"},
			{"System.Double", "double"},
			{"System.Decimal", "decimal"},
		};

		private static readonly Dictionary<string, string> VerboseBuiltinTypeNames = SimplifiedBuiltinTypeNames.ToDictionary(x => x.Value, x => x.Key);

		public CodeTypeReference(Type type, bool global = false)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsArray)
			{
				ArrayRank = type.GetArrayRank();
				ArrayElementType = new CodeTypeReference(type.GetElementType());
			}
			else
			{
				RawName = type.Name;

				if (!type.IsGenericParameter)
				{
					var currentType = type;
					while (currentType.IsNested)
					{
						currentType = currentType.DeclaringType;
						RawName = currentType.Name + "+" + RawName;
					}

					if (!string.IsNullOrEmpty(type.Namespace))
					{
						RawName = type.Namespace + "." + RawName;
					}
				}

				if (type.IsGenericType)
				{
					var genericArgs = type.GetGenericArguments();

					if (type.ContainsGenericParameters)
					{
						TypeArguments = new List<CodeTypeReference>();

						for (var i = 0; i < genericArgs.Length; i++)
						{
							TypeArguments.Add(null);
						}
					}
					else
					{
						TypeArguments = new List<CodeTypeReference>();

						for (var i = 0; i < genericArgs.Length; i++)
						{
							TypeArguments.Add(new CodeTypeReference(genericArgs[i]));
						}
					}
				}
			}
		}

		public CodeTypeReference(string name, bool global = false)
		{
			RawName = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException("Must be a non-null/empty string", nameof(name));
			Global = global;
		}

		public CodeTypeReference(string name, IEnumerable<CodeTypeReference> typeArguments, bool global = false)
		{
			RawName = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException("Must be a non-null/empty string", nameof(name));
			Global = global;
			TypeArguments = new List<CodeTypeReference>(typeArguments);
		}

		public CodeTypeReference(CodeTypeReference arrayElementType, int rank, bool global = false)
		{
			Global = global;
			ArrayElementType = arrayElementType ?? throw new ArgumentNullException(nameof(arrayElementType));
			ArrayRank = rank > 0 ? rank : throw new ArgumentOutOfRangeException(nameof(rank));
		}

		public bool Global { get; }

		public CodeTypeReference ArrayElementType { get; }

		public int ArrayRank { get; }

		public string RawName { get; }

		public List<CodeTypeReference> TypeArguments { get; }

		public string ArraySuffix => '[' + new string(',', ArrayRank - 1) + ']';

		public string CompleteArraySuffix => ArrayElementType != null ? ArrayElementType.CompleteArraySuffix + ArraySuffix : "";

		public CodeTypeReference NestedElementType => ArrayElementType != null ? ArrayElementType.NestedElementType : this;

		public override IEnumerable<CodeElement> Children
		{
			get
			{
				foreach (var child in base.Children)
				{
					yield return child;
				}

				if (ArrayElementType != null)
				{
					yield return ArrayElementType;
				}

				if (TypeArguments != null)
				{
					foreach (var child in TypeArguments)
					{
						if (child != null)
						{
							yield return child;
						}
					}
				}
			}
		}

		public string GetExpandedTypeName()
		{
			var elementType = NestedElementType;
			var elementTypeArguments = elementType.TypeArguments;
			var elementTypeName = elementType.RawName;

			if (VerboseBuiltinTypeNames.TryGetValue(elementTypeName, out var verboseElementName))
			{
				elementTypeName = verboseElementName;
			}

			var result = new StringBuilder(elementTypeName);

			if (elementTypeArguments != null && elementTypeArguments.Count > 0)
			{
				result.Append('[');
				foreach (var elementTypeArgument in elementTypeArguments)
				{
					result.Append('[');
					result.Append(elementTypeArgument.GetExpandedTypeName());
					result.Append(']');
				}

				result.Append(']');
			}

			result.Append(CompleteArraySuffix);
			return result.ToString();
		}

		public Type ResolveExpandedType()
		{
			if (RuntimeCodebase.TryDeserializeType(GetExpandedTypeName(), out var type))
			{
				return type;
			}

			return null;
		}

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

			if (ArrayElementType != null)
			{
				ArrayElementType.Generate(generator);
				generator.Write(TokenType.Punctuation, ArraySuffix);
				generator.ExitElement();
				return;
			}

			var name = RawName;

			if (SimplifiedBuiltinTypeNames.TryGetValue(name, out var simplifiedBuiltinTypeName))
			{
				generator.Write(TokenType.Keyword, simplifiedBuiltinTypeName);
				generator.ExitElement();
				return;
			}

			if (name == "var")
			{
				generator.Write(TokenType.Keyword, name);
				generator.ExitElement();
				return;
			}

			if (Global)
			{
				generator.Write(TokenType.Keyword, "global");
				generator.Write(TokenType.Punctuation, "::");
			}

			var nameBuilder = new StringBuilder();
			var namespaceNameLength = 0;

			{
				var previousIndex = 0;

				for (var i = 0; i < name.Length; i++)
				{
					var c = name[i];
					if (c == '.')
					{
						nameBuilder.Append(name.Substring(previousIndex, i - previousIndex).EscapeIdentifier());
						namespaceNameLength = nameBuilder.Length;
						nameBuilder.Append('.');
						previousIndex = i + 1;
					}
					else if (c == '+')
					{
						nameBuilder.Append(name.Substring(previousIndex, i - previousIndex).EscapeIdentifier()).Append('.');
						previousIndex = i + 1;
					}
				}

				nameBuilder.Append(previousIndex > 0 ? name.Substring(previousIndex).EscapeIdentifier() : name.EscapeUnqualifiedTypeIdentifier());
			}

			if (namespaceNameLength > 0)
			{
				var namespaceRef = new CodeUsingImport(nameBuilder.ToString(0, namespaceNameLength));

				var usingSets = generator.UsingSets;

				foreach (var usingSet in usingSets)
				{
					if (usingSet.Contains(namespaceRef))
					{
						var ambiguityDetected = false;
						var unqualifiedTypeName = nameBuilder.ToString(namespaceNameLength + 1, nameBuilder.Length - namespaceNameLength - 1);
						var unqualifiedTypeNameWithArity = unqualifiedTypeName + (TypeArguments != null && TypeArguments.Count != 0 ? "`" + TypeArguments.Count : "");

						if (TypesByNames.TryGetValue(unqualifiedTypeNameWithArity, out var types))
						{
							foreach (var type in types)
							{
								var otherNamespaceRef = new CodeUsingImport(type.NamespaceName);

								if (namespaceRef != otherNamespaceRef)
								{
									foreach (var otherUsingSet in usingSets)
									{
										if (otherUsingSet.Contains(otherNamespaceRef))
										{
											ambiguityDetected = true;
											break;
										}
									}
								}

								if (ambiguityDetected)
								{
									break;
								}
							}
						}

						ambiguityDetected = ambiguityDetected || generator.Options.PredeclaredTypes.Where(type => type.NamespaceName != namespaceRef.Name && type.TypeName == unqualifiedTypeNameWithArity).Any();

						if (!ambiguityDetected)
						{
							nameBuilder.Length = 0;
							nameBuilder.Append(unqualifiedTypeName);
						}

						break;
					}
				}
			}

			{
				var previousIndex = 0;

				var typeArgumentStart = 0;

				for (var characterIndex = 0; characterIndex < nameBuilder.Length;)
				{
					if (nameBuilder[characterIndex] == '`')
					{
						generator.OutputQualifiedName(TokenType.TypeIdentifier, nameBuilder, previousIndex, characterIndex);

						characterIndex++;

						var arity = 0;

						while (characterIndex < nameBuilder.Length && '0' <= nameBuilder[characterIndex] && nameBuilder[characterIndex] <= '9')
						{
							arity = arity * 10 + (nameBuilder[characterIndex] - '0');
							characterIndex++;
						}

						previousIndex = characterIndex;

						generator.Write(TokenType.Punctuation, '<');

						var first = true;

						for (var typeArgumentIndex = typeArgumentStart; typeArgumentIndex < typeArgumentStart + arity; typeArgumentIndex++)
						{
							var typeArgument = TypeArguments[typeArgumentIndex];

							if (first)
							{
								first = false;
							}
							else
							{
								generator.Write(TokenType.Punctuation, ',');
								if (typeArgument != null)
								{
									generator.Write(TokenType.Space, ' ');
								}
							}

							if (typeArgument != null)
							{
								typeArgument.Generate(generator);
							}
						}

						typeArgumentStart += arity;

						generator.Write(TokenType.Punctuation, '>');
					}
					else
					{
						characterIndex++;
					}
				}

				if (previousIndex < nameBuilder.Length)
				{
					generator.OutputQualifiedName(TokenType.TypeIdentifier, nameBuilder, previousIndex);
				}
			}

			generator.ExitElement();
		}
	}
}