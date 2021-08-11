// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodePrimitiveExpression : CodeExpression
    {
        public CodePrimitiveExpression(object value)
        {
            Value = value;
        }

        public object Value { get; }

		public override PrecedenceGroup Precedence => PrecedenceGroup.Primary;

		protected override void GenerateInner(CodeGenerator generator)
		{
			switch (Value)
			{
				case null: generator.Write(TokenType.NullLiteral, "null"); break;
				case char v: generator.OutputCharLiteral(v); break;
				case sbyte v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); break;
				case ushort v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); break;
				case uint v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); generator.Write(TokenType.IntLiteral, 'u'); break;
				case ulong v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); generator.Write(TokenType.IntLiteral, "ul"); break;
				case string v: generator.OutputStringLiteral(v); break;
				case byte v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); break;
				case short v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); break;
				case int v: generator.Write(TokenType.IntLiteral, v.ToString(CultureInfo.InvariantCulture)); break;
				case long v: generator.Write(TokenType.IntLiteral,  v.ToString(CultureInfo.InvariantCulture)); generator.Write(TokenType.IntLiteral, 'L'); break;
				case float v:
				{
					if (float.IsNaN(v))
					{
						generator.Write(TokenType.Keyword, "float");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "NaN");
					}
					else if (float.IsNegativeInfinity(v))
					{
						generator.Write(TokenType.Keyword, "float");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "NegativeInfinity");
					}
					else if (float.IsPositiveInfinity(v))
					{
						generator.Write(TokenType.Keyword, "float");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "PositiveInfinity");
					}
					else
					{
						generator.Write(TokenType.FloatLiteral, v.ToString(CultureInfo.InvariantCulture));
						generator.Write(TokenType.FloatLiteral, 'f');
					}
					break;
				}
				case double v:
				{
					if (double.IsNaN(v))
					{
						generator.Write(TokenType.Keyword, "double");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "NaN");
					}
					else if (double.IsNegativeInfinity(v))
					{
						generator.Write(TokenType.Keyword, "double");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "NegativeInfinity");
					}
					else if (double.IsPositiveInfinity(v))
					{
						generator.Write(TokenType.Keyword, "double");
						generator.Write(TokenType.Punctuation, '.');
						generator.Write(TokenType.Identifier, "PositiveInfinity");
					}
					else
					{
						var str = v.ToString("R", CultureInfo.InvariantCulture);
						generator.Write(TokenType.FloatLiteral, str.All(char.IsDigit) ? (str + ".0") : str);
					}
					break;
				}
				case decimal v: generator.Write(TokenType.FloatLiteral, v.ToString(CultureInfo.InvariantCulture)); generator.Write(TokenType.FloatLiteral, 'm'); break;
				case bool v: generator.Write(TokenType.BoolLiteral, v ? "true" : "false"); break;
				default: throw new InvalidOperationException(string.Format("Invalid Primitive Type: {0}. Consider using CodeObjectCreateExpression instead.", Value.GetType().ToString()));
			}
		}
	}
}
