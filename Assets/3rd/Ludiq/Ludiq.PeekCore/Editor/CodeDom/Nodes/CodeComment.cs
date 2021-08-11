// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodeComment : CodeElement
    {
		public enum CommentStyle
		{
			Line,
			Block,
			Documentation
		}

        public CodeComment(string text, CommentStyle commentStyle = CommentStyle.Line)
        {
            Text = text;
            Style = commentStyle;
        }

        public string Text { get; }
        public CommentStyle Style { get; }

		public void Generate(CodeGenerator generator)
		{
			generator.EnterElement(this);

			switch (Style)
			{            
				case CommentStyle.Line: generator.Write(TokenType.Comment, "// "); break;
				case CommentStyle.Block: generator.Write(TokenType.Comment, "/* "); break;
				case CommentStyle.Documentation: generator.Write(TokenType.Comment, "/// "); break;
				default: throw new UnexpectedEnumValueException<CommentStyle>(Style);
			}

            string value = Text;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '\u0000')
                {
                    continue;
                }

                generator.Write(TokenType.Comment, value[i]);

				bool writeNewLine = false;

                if (value[i] == '\r')
                {
                    if (i < value.Length - 1 && value[i + 1] == '\n')
                    {
                        i++;
                    }

					writeNewLine = true;
                }
                else if (value[i] == '\n' || value[i] == '\u2028' || value[i] == '\u2029' || value[i] == '\u0085')
                {
					writeNewLine = true;
				}

				if (writeNewLine)
				{
					generator.WriteLine();

					switch (Style)
					{            
						case CommentStyle.Line: generator.Write(TokenType.Comment, "// "); break;
						case CommentStyle.Block: generator.Write(TokenType.Comment, " * "); break;
						case CommentStyle.Documentation: generator.Write(TokenType.Comment, "/// "); break;
						default: throw new UnexpectedEnumValueException<CommentStyle>(Style);
					}
                }
            }

			switch (Style)
			{            
				case CommentStyle.Line: break;
				case CommentStyle.Block:
				{
					generator.Write(TokenType.Comment, " */");					
					break;
				}
				case CommentStyle.Documentation: break;
				default: throw new UnexpectedEnumValueException<CommentStyle>(Style);
			}

			generator.ExitElement();
		}
	}
}
