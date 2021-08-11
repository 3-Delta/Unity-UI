using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ludiq.PeekCore
{
	public class TypeSignature
	{
		public string Namespace;

		public string TypeName;
		
		public int ArrayRank;
		
		public TypeSignature Element;

		public bool IsArray => ArrayRank > 0 || IsUnboundArray;

		public bool IsUnboundArray => ArrayRank == -1;

		public List<TypeSignature> FlattenedGenericArguments;

		public int FlattenedGenericRank => FlattenedGenericArguments?.Count ?? 0;
		
		public int ParentGenericRank => Parent?.FlattenedGenericRank ?? 0;

		public int DefinedGenericRank => FlattenedGenericRank - ParentGenericRank;

		public bool HasGenericArguments => FlattenedGenericRank > 0;

		public bool DefinesGenericArguments => DefinedGenericRank > 0;
		
		public bool IsConstructedGeneric;

		public bool IsGenericParameter;

		public bool IsPointer;

		public bool IsByRef;

		public string AssemblyShortName;

		public string AssemblyVersion;

		public string AssemblyCulture;

		public string AssemblyPublicKeyToken;

		public TypeSignature Parent;

		public TypeSignature Nested;

		public TypeSignature Root
		{
			get
			{
				var type = this;

				while (type.Parent != null)
				{
					type = type.Parent;
				}

				return type;
			}
		}

		public string ToAssemblyQualifiedName()
		{
			return ToDotNetName(true, true);
		}

		public string ToFullNameWithAssemblyQualifiedGenerics()
		{
			return ToDotNetName(false, true);
		}

		public string ToFullName()
		{
			return ToDotNetName(false, false);
		}

		private string ToDotNetName(bool qualify, bool qualifyGenerics)
		{
			var sb = new StringBuilder();
			
			if (IsArray)
			{
				sb.Append(Element.ToDotNetName(false, qualifyGenerics));
				sb.Append('[');

				if (IsUnboundArray)
				{
					sb.Append('*');
				}
				else
				{
					sb.Append(',', ArrayRank - 1);
				}

				sb.Append(']');
			}
			else if (IsByRef)
			{
				sb.Append(Element.ToDotNetName(false, qualifyGenerics));
				sb.Append('&');
			}
			else if (IsPointer)
			{
				sb.Append(Element.ToDotNetName(false, qualifyGenerics));
				sb.Append('*');
			}
			else if (IsGenericParameter)
			{
				sb.Append(TypeName);
			}
			else
			{
				var root = Root;

				if (root.Namespace != null)
				{
					sb.Append(root.Namespace);
					sb.Append('.');
				}

				var current = root;

				while (current != null)
				{
					sb.Append(current.TypeName);

					if (current.DefinesGenericArguments)
					{
						sb.Append('`');
						sb.Append(current.DefinedGenericRank);
					}

					if (current.HasGenericArguments && (current.IsConstructedGeneric || (!qualifyGenerics && current.Nested == null)))
					{
						sb.Append('[');

						var first = true;

						foreach (var genericArgument in current.FlattenedGenericArguments)
						{
							if (!first)
							{
								sb.Append(',');
							}
							else
							{
								first = false;
							}
						
							if (qualifyGenerics)
							{
								sb.Append('[');
							}

							sb.Append(genericArgument.ToDotNetName(qualifyGenerics, qualifyGenerics));
						
							if (qualifyGenerics)
							{
								sb.Append(']');
							}
						}

						sb.Append(']');
					}

					current = current.Nested;

					if (current != null)
					{
						sb.Append('+');
					}
				}
			}

			if (qualify)
			{
				if (AssemblyShortName != null || 
				    AssemblyVersion != null ||
				    AssemblyCulture != null ||
				    AssemblyPublicKeyToken != null)
				{
					sb.Append(", ");
				}

				if (AssemblyShortName != null)
				{
					sb.Append(AssemblyShortName);

					if (AssemblyVersion != null ||
					    AssemblyCulture != null ||
					    AssemblyPublicKeyToken != null)
					{
						sb.Append(", ");
					}
				}

				if (AssemblyVersion != null)
				{
					sb.Append("Version=");
					sb.Append(AssemblyVersion);

					if (AssemblyCulture != null ||
					    AssemblyPublicKeyToken != null)
					{
						sb.Append(", ");
					}
				}

				if (AssemblyCulture != null)
				{
					sb.Append("Culture=");
					sb.Append(AssemblyCulture);

					if (AssemblyPublicKeyToken != null)
					{
						sb.Append(", ");
					}
				}

				if (AssemblyPublicKeyToken != null)
				{
					sb.Append("PublicKeyToken=");
					sb.Append(AssemblyPublicKeyToken);
				}
			}

			return sb.ToString();
		}

		public string ToCSharpReference()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return ToFullName();
		}

		public Type ToType(Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase)
		{
			if (IsGenericParameter)
			{
				throw new NotSupportedException();
			}
			
			if (IsArray)
			{
				var elementType = Element.ToType();

				// https://stackoverflow.com/questions/7057951
				if (IsUnboundArray)
				{
					return elementType.MakeArrayType(1);
				}
				else if (ArrayRank == 1)
				{
					return elementType.MakeArrayType();
				}
				else
				{
					return elementType.MakeArrayType(ArrayRank);
				}
			}
			else if (IsByRef)
			{
				return Element.ToType(assemblyResolver, typeResolver, throwOnError, ignoreCase).MakeByRefType();
			}
			else if (IsPointer)
			{
				return Element.ToType(assemblyResolver, typeResolver, throwOnError, ignoreCase).MakePointerType();
			}

			var fullNameSb = new StringBuilder();

			var root = Root;

			if (root.Namespace != null)
			{
				fullNameSb.Append(root.Namespace);
				fullNameSb.Append('.');
			}

			var current = root;

			while (current != null)
			{
				fullNameSb.Append(current.TypeName);

				if (current.DefinesGenericArguments)
				{
					fullNameSb.Append('`');
					fullNameSb.Append(current.DefinedGenericRank);
				}

				current = current.Nested;

				if (current != null)
				{
					fullNameSb.Append('+');
				}
			}

			var type = Type.GetType(fullNameSb.ToString(), assemblyResolver, typeResolver, throwOnError, ignoreCase);

			if (type == null)
			{
				return null;
			}

			if (IsConstructedGeneric)
			{
				var genericArguments = new Type[FlattenedGenericRank];

				for (int i = 0; i < FlattenedGenericRank; i++)
				{
					var genericArgumentSignature = FlattenedGenericArguments[i];
					genericArguments[i] = genericArgumentSignature.ToType();
				}

				type = type.MakeGenericType(genericArguments);
			}

			return type;
		}

		public Type ToType(Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError)
		{
			return ToType(assemblyResolver, typeResolver, throwOnError, false);
		}
		
		public Type ToType(Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver)
		{
			return ToType(assemblyResolver, typeResolver, false, false);
		}
		
		public Type ToType(bool throwOnError, bool ignoreCase)
		{
			return ToType(null, null, throwOnError, ignoreCase);
		}
		
		public Type ToType(bool throwOnError)
		{
			return ToType(null, null, throwOnError, false);
		}

		public Type ToType()
		{
			return ToType(null, null, false, false);
		}
	}

	public static class TypeSignatureUtility
	{
		public static void Main()
		{
			var testTypes = new Type[]
			{
				typeof(int),
				typeof(int[]),
				typeof(int[,]),
				typeof(int[][]),
				typeof(int[,][,,][,,,]),
				typeof(int*[]),
				typeof(int**),
				typeof(List<>),
				typeof(List<int>),
				typeof(List<int>[]),
				typeof(List<>.Enumerator),
				typeof(List<int>.Enumerator),
				typeof(List<int>.Enumerator[]),
				typeof(Dictionary<,>),
				typeof(Dictionary<int, string>),
				typeof(Dictionary<int, string>.Enumerator),
				typeof(Dictionary<,>.Enumerator),
				typeof(Func<>),
			};

			foreach (var testType in testTypes)
			{
				var signature = testType.ToSignature();
				var reconstructedType = signature.ToType();
				
				Console.WriteLine($"Reconstruction:                  {testType == reconstructedType}");
				Console.WriteLine();
				Console.WriteLine($"Full Name (Type):                {testType.ToString()}");
				Console.WriteLine($"Full Name (Signature):           {signature.ToFullName()}");
				//Console.WriteLine($"Full Name (Parsing):             {TypeSignature.ParseDotNetName(testType.ToString()).ToFullName()}");
				Console.WriteLine();
				Console.WriteLine($"Qualified Generics (Type):       {testType.FullName}");
				Console.WriteLine($"Qualified Generics (Signature):  {signature.ToFullNameWithAssemblyQualifiedGenerics()}");
				//Console.WriteLine($"Qualified Generics (Parsing):    {TypeSignature.ParseDotNetName(testType.ToString()).ToFullNameWithAssemblyQualifiedGenerics()}");
				Console.WriteLine();
				Console.WriteLine($"Qualified Name (Type):           {testType.AssemblyQualifiedName}");
				Console.WriteLine($"Qualified Name (Signature):      {signature.ToAssemblyQualifiedName()}");
				//Console.WriteLine($"Qualified Name (Parsing):        {TypeSignature.ParseDotNetName(testType.ToString()).ToAssemblyQualifiedName()}");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine();
			}
		}

		public static TypeSignature ToSignature(this Type type)
		{
			var signature = new TypeSignature();

			var assemblyName = type.Assembly.GetName();
			signature.AssemblyShortName = assemblyName.Name;
			signature.AssemblyCulture = string.IsNullOrEmpty(assemblyName.CultureName) ? "neutral" : assemblyName.CultureName;
			signature.AssemblyVersion = assemblyName.Version.ToString();
			signature.AssemblyPublicKeyToken = BitConverter.ToString(assemblyName.GetPublicKeyToken()).Replace("-", "").ToLower();

			if (type.IsArray)
			{
				signature.ArrayRank = type.GetArrayRank();
				signature.Element = type.GetElementType().ToSignature();
			}
			else if (type.IsByRef)
			{
				signature.IsByRef = true;
				signature.Element = type.GetElementType().ToSignature();
			}
			else if (type.IsPointer)
			{
				signature.IsPointer = true;
				signature.Element = type.GetElementType().ToSignature();
			}
			else if (type.IsGenericParameter)
			{
				signature.TypeName = type.Name;
				signature.IsGenericParameter = true;
			}
			else
			{
				if (type.IsGenericType)
				{
					signature.TypeName = type.Name.PartBeforeTemp('`');
					signature.FlattenedGenericArguments = type.GetGenericArguments().Select(ToSignature).ToList();
					signature.IsConstructedGeneric = type.IsConstructedGenericType;
				}
				else
				{
					signature.TypeName = type.Name;
				}

				if (type.IsNested)
				{
					signature.Parent = type.DeclaringType.ToSignature();
					signature.Parent.Nested = signature;
				}
				else
				{
					signature.Namespace = type.Namespace;
				}
			}

			return signature;
		}
	}

	public static class StringExTemp
	{
		public static string PartBeforeTemp(this string s, char c)
		{
			//Ensure.That(nameof(s)).IsNotNull(s);

			var index = s.IndexOf(c);

			if (index > 0)
			{
				return s.Substring(0, index);
			}
			else
			{
				return s;
			}
		}
	}
}
