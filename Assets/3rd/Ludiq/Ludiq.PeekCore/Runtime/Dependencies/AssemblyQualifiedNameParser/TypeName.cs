using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ludiq.PeekCore
{
	public class TypeName
	{
		public string AssemblyDescription { get; }
		
		public string AssemblyName { get; }

		public string AssemblyVersion { get; }

		public string AssemblyCulture { get; }

		public string AssemblyPublicKeyToken { get; }

		public List<TypeName> GenericParameters { get; } = new List<TypeName>();
		
		private readonly List<string> names = new List<string>();

		private readonly List<int> genericarities = new List<int>();

		public string Name { get; private set; }

		public string LastName => names[names.Count - 1];

		public static TypeName Parse(string s)
		{
			var index = 0;
			return new TypeName(s, ref index);
		}

		private enum State
		{
			Name,
			Array,
			Generics,
			Assembly
		}

		private TypeName(string s, ref int index)
		{
			try
			{
				var startIndex = index;

				var nameStartIndex = startIndex;
				var nameEndIndex = (int?)null;

				var assemblyDescriptionStartIndex = (int?)null;
				var assemblyDescriptionEndIndex = (int?)null;

				var hasGroupingBracket = false;

				var state = State.Name;

				for (; index < s.Length; ++index)
				{
					var currentCharacter = s[index];
					var nextCharacter = index + 1 < s.Length ? s[index + 1] : (char?)null;

					if (state == State.Name)
					{
						if (currentCharacter == '[')
						{
							if (index == startIndex)
							{
								// Skip type grouping bracket
								hasGroupingBracket = true;
								nameStartIndex++;
							}
							else if (nextCharacter == ']' || nextCharacter == ',')
							{
								// Square bracket delimits an array
								state = State.Array;
							}
							else
							{
								// Square bracket delimits the generic argument list
								nameEndIndex = index;
								state = State.Generics;
							}
						}
						else if (currentCharacter == ']')
						{
							if (hasGroupingBracket)
							{
								// We finished the current grouping, break out
								break;
							}
						}
						else if (currentCharacter == ',')
						{
							// We're entering assembly description

							state = State.Assembly;

							assemblyDescriptionStartIndex = index + 1;

							if (nameEndIndex == null)
							{
								nameEndIndex = index;
							}
						}
					}
					else if (state == State.Array)
					{
						if (currentCharacter == ']')
						{
							state = State.Name;
						}
					}
					else if (state == State.Generics)
					{
						if (currentCharacter == ']')
						{
							state = State.Name;
						}
						else if (currentCharacter == ',' || currentCharacter == ' ')
						{
							// Generic delimiters
						}
						else
						{
							GenericParameters.Add(new TypeName(s, ref index));
						}
					}
					else if (state == State.Assembly)
					{
						if (currentCharacter == ']')
						{
							if (hasGroupingBracket)
							{
								// We finished the current grouping, break out
								assemblyDescriptionEndIndex = index;
								break;
							}
						}
					}
				}

				if (nameEndIndex == null)
				{
					nameEndIndex = s.Length;
				}

				if (assemblyDescriptionEndIndex == null)
				{
					assemblyDescriptionEndIndex = s.Length;
				}

				Name = s.Substring(nameStartIndex, nameEndIndex.Value - nameStartIndex);

				if (Name.Contains('+'))
				{
					var nestedNames = Name.Split('+');

					foreach (var nestedName in nestedNames)
					{
						nestedName.PartsAround('`', out var name, out var genericarity);
						
						names.Add(name);

						if (genericarity != null)
						{
							genericarities.Add(int.Parse(genericarity));
						}
						else
						{
							genericarities.Add(0);
						}
					}
				}
				else
				{
					Name.PartsAround('`', out var name, out var genericarity);
						
					names.Add(name);

					if (genericarity != null)
					{
						genericarities.Add(int.Parse(genericarity));
					}
					else
					{
						genericarities.Add(0);
					}
				}

				if (assemblyDescriptionStartIndex != null)
				{
					AssemblyDescription = s.Substring(assemblyDescriptionStartIndex.Value, assemblyDescriptionEndIndex.Value - assemblyDescriptionStartIndex.Value);

					var parts = AssemblyDescription.Split(',')
						.Select(x => x.Trim())
						.ToList();

					AssemblyVersion = LookForPairThenRemove(parts, "Version");
					AssemblyCulture = LookForPairThenRemove(parts, "Culture");
					AssemblyPublicKeyToken = LookForPairThenRemove(parts, "PublicKeyToken");

					if (parts.Count > 0)
					{
						AssemblyName = parts[0];
					}
				}
			}
			catch (Exception ex)
			{
				throw new FormatException($"Failed to parse type name: {s}", ex);
			}
		}

		private static string LookForPairThenRemove(List<string> strings, string Name)
		{
			for (var istr = 0; istr < strings.Count; istr++)
			{
				var s = strings[istr];
				var i = s.IndexOf(Name);

				if (i == 0)
				{
					var i2 = s.IndexOf('=');

					if (i2 > 0)
					{
						var ret = s.Substring(i2 + 1);
						strings.RemoveAt(istr);

						return ret;
					}
				}
			}

			return null;
		}

		public void ReplaceNamespace(string oldNamespace, string newNamespace)
		{
			if (names[0].StartsWith(oldNamespace + "."))
			{
				names[0] = newNamespace + "." + names[0].TrimStart(oldNamespace + ".");
			}

			foreach (var genericParameter in GenericParameters)
			{
				genericParameter.ReplaceNamespace(oldNamespace, newNamespace);
			}

			UpdateName();
		}

		public void Replace(string oldTypeName, string newTypeName)
		{
			for (int i = 0; i < names.Count; i++)
			{
				if (names[i] == oldTypeName)
				{
					names[i] = newTypeName;
				}
			}

			foreach (var genericParameter in GenericParameters)
			{
				genericParameter.Replace(oldTypeName, newTypeName);
			}

			UpdateName();
		}

		private void UpdateName()
		{
			var sb = new StringBuilder();

			for (int i = 0; i < names.Count; i++)
			{
				if (i != 0)
				{
					sb.Append('+');
				}

				sb.Append(names[i]);

				if (genericarities[i] > 0)
				{
					sb.Append('`');
					sb.Append(genericarities[i]);
				}
			}

			Name = sb.ToString();
		}

		public string ToString(TypeNameDetail specification, TypeNameDetail genericsSpecification)
		{
			var sb = new StringBuilder();

			sb.Append(Name);

			if (GenericParameters.Count > 0)
			{
				sb.Append("[");

				var isFirstParameter = true;

				foreach (var genericParameter in GenericParameters)
				{
					if (!isFirstParameter)
					{
						sb.Append(",");
					}

					if (genericsSpecification != TypeNameDetail.Name)
					{
						sb.Append("[");
					}

					sb.Append(genericParameter.ToString(genericsSpecification, genericsSpecification));

					if (genericsSpecification != TypeNameDetail.Name)
					{
						sb.Append("]");
					}

					isFirstParameter = false;
				}

				sb.Append("]");
			}

			if (specification == TypeNameDetail.Full)
			{
				if (!string.IsNullOrEmpty(AssemblyDescription))
				{
					sb.Append(", ");
					sb.Append(AssemblyDescription);
				}
			}
			else if (specification == TypeNameDetail.NameAndAssembly)
			{
				if (!string.IsNullOrEmpty(AssemblyName))
				{
					sb.Append(", ");
					sb.Append(AssemblyName);
				}
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return ToString(TypeNameDetail.Name, TypeNameDetail.Full);
		}

		public string ToLooseString()
		{
			// Removes the Culture, Token and VersionNumber
			return ToString(TypeNameDetail.NameAndAssembly, TypeNameDetail.NameAndAssembly);
		}

		public static string Simplify(string typeName)
		{
			return Parse(typeName).ToLooseString();
		}

		public static string SimplifyFast(string typeName)
		{
			// This assumes type strings are written with ', Version=' first, which
			// is standard for Type.AssemblyQualifiedName but not technically spec guaranteed.
			// It is however incredibly faster than parsing the type name and re-outputting it.

			while (true)
			{
				var startIndex = typeName.IndexOf(", Version=", StringComparison.Ordinal);

				if (startIndex >= 0)
				{
					var endIndex = typeName.IndexOf(']', startIndex);

					if (endIndex >= 0)
					{
						typeName = typeName.Remove(startIndex, endIndex - startIndex);
					}
					else
					{
						typeName = typeName.Substring(0, startIndex);
						break;
					}
				}
				else
				{
					break;
				}
			}

			return typeName;
		}
	}
}