using System.Collections.Generic;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
	public static class CodeGeneratorUtility
	{
		public static void Generate(this IEnumerable<CodeDirective> directives, CodeGenerator generator)
		{
            foreach (var directive in directives)
            {
				directive.Generate(generator);
            }
		}

		public static void ReserveLocals(this IEnumerable<CodeStatement> statements, CodeGenerator generator, CodeStatementEmitOptions options)
		{
            foreach (var statement in statements)
            {
                statement.ReserveLocals(generator, options);
            }
		}

        public static void Generate(this IEnumerable<CodeStatement> statements, CodeGenerator generator, CodeStatementEmitOptions options)
        {
            foreach (var statement in statements)
            {
                statement.Generate(generator, options);
            }
        }

        public static void Generate(this IEnumerable<CodeNamespace> namespaces, CodeGenerator generator)
        {
            foreach (var ns in namespaces)
            {
                ns.Generate(generator);
            }
        }

        public static void Generate(this IEnumerable<CodeComment> comments, CodeGenerator generator)
        {
            foreach (var comment in comments)
            {
                comment.Generate(generator);
				generator.WriteLine();
            }
        }

        public static void Generate(this IEnumerable<CodeUsingImport> usingImports, CodeGenerator generator)
        {
			var sortedUniqueUsingImports = new List<CodeUsingImport>(new HashSet<CodeUsingImport>(usingImports));
			sortedUniqueUsingImports.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

            foreach (var usingImport in sortedUniqueUsingImports)
            {
                usingImport.Generate(generator);
            }

            if (usingImports.Any())
            {
                generator.WriteLine();
            }
        }

		public static void GenerateCommaSeparated(this IEnumerable<CodeExpression> expressions, CodeGenerator generator, bool newlineBetweenItems = false)
		{
            bool first = true;

            //generator.Indent++;

            foreach (var expression in expressions)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    if (newlineBetweenItems)
					{
                        generator.WriteLine(TokenType.Punctuation, ',');
					}
                    else
					{
						generator.Write(TokenType.Punctuation, ',');
						generator.Write(TokenType.Space, ' ');
					}
                }

                expression.Generate(generator);
            }

            //generator.Indent--;
		}
		
        public static void GenerateCommaSeparated(this IEnumerable<CodeParameterDeclaration> parameters, CodeGenerator generator, bool newlineBetweenItems = false)
        {
            bool first = true;

            foreach (var parameter in parameters)
            {
				if (first)
				{
					first = false;
				}
				else
				{
					if (newlineBetweenItems)
					{
                        generator.WriteLine(TokenType.Punctuation, ',');
					}
					else
					{
						generator.Write(TokenType.Punctuation, ',');
						generator.Write(TokenType.Space, ' ');
					}
				}

                parameter.Generate(generator);
            }
        }

        public static void Generate(this CodeParameterDirection dir, CodeGenerator generator)
        {
            switch (dir)
            {
                case CodeParameterDirection.Default: break;
                case CodeParameterDirection.Out: generator.Write(TokenType.Keyword, "out"); generator.Write(TokenType.Space, ' '); break;
                case CodeParameterDirection.Ref: generator.Write(TokenType.Keyword, "ref"); generator.Write(TokenType.Space, ' '); break;
            }
        }

		public static void Generate(this CodeMemberModifiers modifiers, CodeGenerator generator)
		{
            switch (modifiers & CodeMemberModifiers.NewMask)
            {
                case CodeMemberModifiers.New: generator.Write(TokenType.Keyword, "new"); generator.Write(TokenType.Space, ' '); break;
            }

            switch (modifiers & CodeMemberModifiers.AccessMask)
            {
                case CodeMemberModifiers.Internal: generator.Write(TokenType.Keyword, "internal"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Protected: generator.Write(TokenType.Keyword, "protected"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.ProtectedInternal: generator.Write(TokenType.Keyword, "protected internal"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Private: generator.Write(TokenType.Keyword, "private"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Public: generator.Write(TokenType.Keyword, "public"); generator.Write(TokenType.Space, ' '); break;
            }

            switch (modifiers & CodeMemberModifiers.ScopeMask)
            {
                case CodeMemberModifiers.Abstract: generator.Write(TokenType.Keyword, "abstract"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Virtual: generator.Write(TokenType.Keyword, "virtual"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Static: generator.Write(TokenType.Keyword, "static"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Override: generator.Write(TokenType.Keyword, "override"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Sealed: generator.Write(TokenType.Keyword, "sealed"); generator.Write(TokenType.Space, ' '); break;
                case CodeMemberModifiers.Const: generator.Write(TokenType.Keyword, "const"); generator.Write(TokenType.Space, ' '); break;
            }

            switch (modifiers & CodeMemberModifiers.ReadOnlyMask)
            {
                case CodeMemberModifiers.ReadOnly: generator.Write(TokenType.Keyword, "readonly"); generator.Write(TokenType.Space, ' '); break;
            }
		}

        public static readonly HashSet<string> Keywords = new HashSet<string> {
            "as",
            "do",
            "if",
            "in",
            "is",
            "for",
            "int",
            "new",
            "out",
            "ref",
            "try",
            "base",
            "bool",
            "byte",
            "case",
            "char",
            "else",
            "enum",
            "goto",
            "lock",
            "long",
            "null",
            "this",
            "true",
            "uint",
            "void",
			"async",
			"await",
            "break",
            "catch",
            "class",
            "const",
            "event",
            "false",
            "fixed",
            "float",
            "sbyte",
            "short",
            "throw",
            "ulong",
            "using",
            "while",
            "yield",
            "double",
            "extern",
            "object",
            "params",
            "public",
            "return",
            "sealed",
            "sizeof",
            "static",
            "string",
            "struct",
            "switch",
            "typeof",
            "unsafe",
            "ushort",
            "checked",
            "decimal",
            "default",
            "finally",
            "foreach",
            "private",
            "virtual",
            "abstract",
            "continue",
            "delegate",
            "explicit",
            "implicit",
            "internal",
            "operator",
            "override",
            "readonly",
            "volatile",
            "__arglist",
            "__makeref",
            "__reftype",
            "interface",
            "namespace",
            "protected",
            "unchecked",
            "__refvalue",
            "stackalloc",
        };

        public static readonly HashSet<string> TypeKeywords = new HashSet<string> {
			"int",
			"bool",
			"byte",
			"char",
			"long",
			"uint",
			"float",
			"sbyte",
			"short",
			"ulong",
			"double",
			"object",
			"string",
			"ushort",
			"decimal",
		};

        public static string EscapeIdentifier(this string name)
        {
	        var first = name[0];

	        if (first != '_' && !char.IsLetter(first))
	        {
		        name = '_' + name;
	        }

	        if (Keywords.Contains(name) || name.StartsWith("__"))
	        {
		        name = '@' + name;
	        }

	        return name;
        }

		public static string EscapeUnqualifiedTypeIdentifier(this string name)
		{
			return TypeKeywords.Contains(name)
				? name
				: EscapeIdentifier(name);
		}

		// See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/
		public enum BinaryOperatorPrecedence
		{
			Multiplicative,
			Additive,
			BitwiseShift,
			Relational,
			Equality,
			BitwiseAnd,
			BitwiseXor,
			BitwiseOr,
			LogicalAnd,
			LogicalOr,
			NullCoalescing,
		}

		public static readonly Dictionary<CodeBinaryOperatorType, BinaryOperatorPrecedence> BinaryOperatorPrecedences = new Dictionary<CodeBinaryOperatorType, BinaryOperatorPrecedence>() {
			// Multiplicative operators
			{ CodeBinaryOperatorType.Multiply, BinaryOperatorPrecedence.Multiplicative },
			{ CodeBinaryOperatorType.Divide, BinaryOperatorPrecedence.Multiplicative },
			{ CodeBinaryOperatorType.Modulo, BinaryOperatorPrecedence.Multiplicative },
			// Additive operators
			{ CodeBinaryOperatorType.Add, BinaryOperatorPrecedence.Additive },
			{ CodeBinaryOperatorType.Subtract, BinaryOperatorPrecedence.Additive },
			// Bitwise shift operators
			{ CodeBinaryOperatorType.BitwiseShiftLeft, BinaryOperatorPrecedence.BitwiseShift },
			{ CodeBinaryOperatorType.BitwiseShiftRight, BinaryOperatorPrecedence.BitwiseShift },
			// Relational operators
			{ CodeBinaryOperatorType.LessThan, BinaryOperatorPrecedence.Relational },
			{ CodeBinaryOperatorType.GreaterThan, BinaryOperatorPrecedence.Relational },
			{ CodeBinaryOperatorType.LessThanOrEqual, BinaryOperatorPrecedence.Relational },
			{ CodeBinaryOperatorType.GreaterThanOrEqual, BinaryOperatorPrecedence.Relational },
			{ CodeBinaryOperatorType.Is, BinaryOperatorPrecedence.Relational },
			{ CodeBinaryOperatorType.As, BinaryOperatorPrecedence.Relational },
			// Equality operators
			{ CodeBinaryOperatorType.Equality, BinaryOperatorPrecedence.Equality },
			{ CodeBinaryOperatorType.Inequality, BinaryOperatorPrecedence.Equality },
			// Bitwise and
			{ CodeBinaryOperatorType.BitwiseAnd, BinaryOperatorPrecedence.BitwiseAnd },
			// Bitwise xor
			{ CodeBinaryOperatorType.BitwiseXor, BinaryOperatorPrecedence.BitwiseXor },
			// Bitwise or
			{ CodeBinaryOperatorType.BitwiseOr, BinaryOperatorPrecedence.BitwiseOr },
			// Logical and
			{ CodeBinaryOperatorType.LogicalAnd, BinaryOperatorPrecedence.LogicalAnd },
			// Logical or
			{ CodeBinaryOperatorType.LogicalOr, BinaryOperatorPrecedence.LogicalOr },
			// Null coalescing
			{ CodeBinaryOperatorType.NullCoalesce, BinaryOperatorPrecedence.NullCoalescing },
		};

		public static bool IsLogicalPrecedence(this BinaryOperatorPrecedence prec)
		{
			return prec == BinaryOperatorPrecedence.LogicalOr || prec == BinaryOperatorPrecedence.LogicalAnd;
		}

		public static bool IsBitwisePrecedence(this BinaryOperatorPrecedence prec)
		{
			return prec == BinaryOperatorPrecedence.BitwiseXor || prec == BinaryOperatorPrecedence.BitwiseAnd || prec == BinaryOperatorPrecedence.BitwiseOr || prec == BinaryOperatorPrecedence.BitwiseShift;
		}

		public static bool ShouldParenthesizeWhenMixed(this BinaryOperatorPrecedence prec)
		{
			return prec.IsBitwisePrecedence() || prec.IsLogicalPrecedence();
		}

		public static bool IsValidAssignmentTerm(this CodeExpression expression)
		{
			return expression is CodeVariableReferenceExpression || expression is CodeFieldReferenceExpression || expression is CodeIndexExpression;
		}
	}
}
