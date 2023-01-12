// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeGenerator
    {	
        private ICodeWriter writer;
        private int indent;
        private bool tabsPending;

        private CodeGenerator(ICodeWriter writer, CodeGeneratorOptions options)
		{
			this.writer = writer;
			Options = options;
		}

        public CodeGeneratorOptions Options { get; }
		public List<HashSet<CodeUsingImport>> UsingSets { get; } = new List<HashSet<CodeUsingImport>>();
		public List<HashSet<string>> LocalScopes { get; } = new List<HashSet<string>>();
		public bool JustWroteOpeningBrace { get; set; }
		public bool JustWroteClosingBrace { get; set; }
		public bool JustWroteVariableDeclaration { get; set; }
		public bool IsInSetterProperty { get; set; }

        public int Indent
        {
            get
			{
				return indent;
			}
            set
			{
				indent = Math.Max(value, 0);
				writer.Indent = indent;
			}
        }

        private void MaybeWritePendingTabs()
        {
            if (tabsPending)
            {
                WriteTabs();
                ClearPendingTabs();
            }
        }

		private void ClearPendingTabs()
		{
            tabsPending = false;
		}

        private void WriteTabs()
        {
            for (int i = 0; i < indent; i++)
            {
                writer.Write(TokenType.Indentation, Options.IndentString);
            }
        }

		private void ClearWriteState()
		{
			JustWroteOpeningBrace = false;
			JustWroteClosingBrace = false;
			JustWroteVariableDeclaration = false;
		}

		public void EnterElement(CodeElement element)
		{
			writer.EnterElement(element);
		}

		public void ExitElement()
		{
			writer.ExitElement();
		}

        public void Write(TokenType type, string value)
        {
            MaybeWritePendingTabs();
            writer.Write(type, value);

			ClearWriteState();
        }

        public void Write(TokenType type, char value)
        {
            MaybeWritePendingTabs();
            writer.Write(type, value);

			ClearWriteState();
        }

        public void WriteLine()
        {
            MaybeWritePendingTabs();
            writer.WriteLine();

            tabsPending = true;
			ClearWriteState();
		}

        public void WriteLine(TokenType type, char value)
        {
            MaybeWritePendingTabs();
            writer.WriteLine(type, value);

            tabsPending = true;
			ClearWriteState();
        }

        public void WriteLine(TokenType type, string value)
        {
            MaybeWritePendingTabs();
            writer.WriteLine(type, value);

            tabsPending = true;
			ClearWriteState();
        }

		public void WriteBlankLineBeforeEnteringBlock()
		{
			if (JustWroteClosingBrace || !JustWroteOpeningBrace)
			{
                WriteLine();
			}
		}

		public void WriteBlankLineIfJustExitedBlock()
		{
			if (JustWroteClosingBrace)
			{
				WriteLine();
			}
		}

        public void WriteEmptyBlock(bool forceSameLine = false)
		{
			Write(TokenType.Space, ' ');

			if (forceSameLine)
			{
				Write(TokenType.Punctuation, "{ }");
			}
			else
			{
				WriteLine(TokenType.Punctuation, "{ }");
			}
		}

        public void WriteOpeningBrace()
        {
            if (Options.BracingStyle == CodeGeneratorOptions.BracingStyleOption.AddNewLine)
            {
                WriteLine();
            }
            else
            {
				Write(TokenType.Space, ' ');
            }

            WriteLine(TokenType.Punctuation, '{');
			JustWroteOpeningBrace = true;
        }

        public void WriteMiddleClosingBrace(bool forceSameLine = false)
		{
            Write(TokenType.Punctuation, '}');
            if (Options.MiddleClosingBraceOnSameLine || forceSameLine)
            {
                Write(TokenType.Space, ' ');
            }
            else
            {
                WriteLine();
            }

			JustWroteClosingBrace = true;
		}

        public void WriteClosingBrace(bool sameLine = false)
		{
			Write(TokenType.Punctuation, '}');

			if (!sameLine)
			{
				WriteLine();
			}

			JustWroteClosingBrace = true;
		}

		public void PushUsingSet(IEnumerable<CodeUsingImport> usings)
		{
			UsingSets.Add(new HashSet<CodeUsingImport>(usings));
		}

		public void PopUsingSet()
		{
			UsingSets.RemoveAt(UsingSets.Count - 1);
		}

		public void EnterLocalScope()
		{
			LocalScopes.Add(new HashSet<string>());
		}

		public void ExitLocalScope()
		{
			LocalScopes.RemoveAt(LocalScopes.Count - 1);
		}

		public void ReserveLocal(string name)
		{
			if (ContainsLocalByName(name))
			{
				throw new InvalidOperationException($"A local named \"{name}\" already exists in the current scope or an enclosing scope");
			}

			LocalScopes.LastOrDefault().Add(name);
		}

		public bool ContainsLocalByName(string name)
		{
			foreach (var localScope in LocalScopes)
			{
				if (localScope.Contains(name))
				{
					return true;
				}
			}

			return false;
		}

        public void OutputIdentifier(TokenType type, string identifier)
		{
			Write(TokenType.Identifier, identifier.EscapeIdentifier());
		}

		public void OutputQualifiedName(TokenType identifierType, string qualifiedName, int startIndex = 0, int endIndex = -1)
		{
			endIndex = endIndex >= 0 ? endIndex : qualifiedName.Length;

			int lastIdentifier = startIndex;

			for (int i = startIndex; i < endIndex; i++)
			{
				char c = qualifiedName[i];

				if (c == '.')
				{
					Write(identifierType, qualifiedName.Substring(lastIdentifier, i - lastIdentifier));
					Write(TokenType.Punctuation, '.');
					lastIdentifier = i + 1;
				}
			}

			if (lastIdentifier < endIndex)
			{
				Write(identifierType, qualifiedName.Substring(lastIdentifier, endIndex - lastIdentifier));
			}
		}

		public void OutputQualifiedName(TokenType identifierType, StringBuilder stringBuilder, int startIndex = 0, int endIndex = -1)
		{
			endIndex = endIndex >= 0 ? endIndex : stringBuilder.Length;

			int lastIdentifier = startIndex;

			for (int i = startIndex; i < endIndex; i++)
			{
				char c = stringBuilder[i];

				if (c == '.')
				{
					Write(identifierType, stringBuilder.ToString(lastIdentifier, i - lastIdentifier));
					Write(TokenType.Punctuation, '.');
					lastIdentifier = i + 1;
				}
			}

			if (lastIdentifier < endIndex)
			{
				Write(identifierType, stringBuilder.ToString(lastIdentifier, endIndex - lastIdentifier));
			}
		}

        public void OutputStringLiteral(string value)
        {
            Write(TokenType.StringLiteral, '\"');
			for (int i = 0; i < value.Length; i++)
			{
				OutputEscapedChar(TokenType.StringLiteral, value[i]);
			}
            Write(TokenType.StringLiteral, '\"');
        }

        public void OutputCharLiteral(char value)
        {
            Write(TokenType.CharLiteral, '\'');
			OutputEscapedChar(TokenType.CharLiteral, value);
            Write(TokenType.CharLiteral,'\'');
        }

        private void OutputEscapedChar(TokenType type, char c)
        {
			switch (c)
			{
				case '\r': Write(type, "\\r"); break;
                case '\t': Write(type, "\\t"); break;
                case '\"': Write(type, "\\\""); break;
                case '\'': Write(type, "\\\'"); break;
                case '\\': Write(type, "\\\\"); break;
                case '\0': Write(type, "\\0"); break;
                case '\n': Write(type, "\\n"); break;
                case '\u2028':
				case '\u2029':
				case '\u0084':
				case '\u0085':
				{
					OutputUnicodeEscape(type, c);
					break;
				}
				default:
				{
                    if (char.IsSurrogate(c))
					{
						OutputUnicodeEscape(type, c);
					}
					else
					{
						Write(type, c);
					}
					break;
				}
			}
        }

		private void OutputUnicodeEscape(TokenType type, char c)
		{
			Write(type, "\\u");
			Write(type, ((int) c).ToString("X4", CultureInfo.InvariantCulture));
		}

		public void WriteStatementEnd(CodeStatementEmitOptions options)
		{
			if ((options & CodeStatementEmitOptions.OmitSemiColon) == 0)
			{
				WriteLine(TokenType.Punctuation, ';');
			}
		}

        public void OutputTypeParameters(List<CodeTypeParameter> typeParameters)
        {
            if (typeParameters.Count > 0)
            {
				Write(TokenType.Punctuation, '<');

				bool first = true;
				foreach (var typeParameter in typeParameters)
				{
					if (first)
					{
						first = false;
					}
					else
					{
						Write(TokenType.Punctuation, ',');
						Write(TokenType.Space, ' ');
					}
					typeParameter.GenerateDeclaration(this);
				}

				Write(TokenType.Punctuation, '>');
            }
        }

        public void GenerateAttributes(List<CodeAttributeDeclaration> attributes, string prefix = null, bool sameLine = false)
        {
            if (attributes.Count == 0)
			{
				return;
			}

            bool paramArray = false;
            foreach (CodeAttributeDeclaration current in attributes)
            {
                // we need to convert paramArrayAttribute to params keyword to 
                // make csharp compiler happy. In addition, params keyword needs to be after 
                // other attributes.

                if (current.AttributeType.RawName.Equals("system.paramarrayattribute", StringComparison.OrdinalIgnoreCase))
                {
                    paramArray = true;
                    continue;
                }

                Write(TokenType.Punctuation, '[');
                if (prefix != null)
                {
                    Write(TokenType.Identifier, prefix);
                    Write(TokenType.Punctuation, ':');
                    Write(TokenType.Space, ' ');
                }

                if (current.AttributeType != null)
                {
                    current.AttributeType.Generate(this);
                }
                Write(TokenType.Punctuation, '(');

                bool firstArg = true;
                foreach (CodeAttributeArgument arg in current.Arguments)
                {
                    if (firstArg)
                    {
                        firstArg = false;
                    }
                    else
                    {
                        Write(TokenType.Punctuation, ',');
                        Write(TokenType.Space, ' ');
                    }

                    arg.Generate(this);
                }

                Write(TokenType.Punctuation, ')');
				Write(TokenType.Punctuation, ']');

                if (sameLine)
                {
                    Write(TokenType.Space, ' ');
                }
                else
                {
                    WriteLine();
                }
            }

            if (paramArray)
            {
                if (prefix != null)
                {
                    Write(TokenType.Identifier, prefix);
                    Write(TokenType.Punctuation, ':');
                    Write(TokenType.Space, ' ');
                }
                Write(TokenType.Keyword, "params");

                if (sameLine)
                {
                    Write(TokenType.Space, ' ');
                }
                else
                {
                    WriteLine();
                }
            }
        }

		public static void GenerateCodeFromCompositeTypeMember(CodeCompositeTypeMember member, CodeCompositeTypeDeclaration enclosingType, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
            member.Generate(generator, enclosingType);
        }

        public static void GenerateCodeFromType(CodeBasicTypeDeclaration decl, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
            decl.Generate(generator);
        }

        public static void GenerateCodeFromExpression(CodeExpression expr, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
            expr.Generate(generator);
        }

        public static void GenerateCodeFromCompileUnit(CodeCompileUnit unit, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
            unit.Generate(generator);
        }

        public static void GenerateCodeFromNamespace(CodeNamespace ns, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
            ns.Generate(generator);
        }

        public static void GenerateCodeFromStatement(CodeStatement stmt, ICodeWriter writer, CodeGeneratorOptions options)
        {
			var generator = new CodeGenerator(writer, options);
			stmt.Generate(generator, default(CodeStatementEmitOptions));
        }
	}
}
