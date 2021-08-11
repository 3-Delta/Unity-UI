// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace Ludiq.PeekCore.CodeDom
{
    public sealed class CodePragmaChecksumDirective : CodeDirective
    {
        public CodePragmaChecksumDirective(string fileName, Guid checksumAlgorithmId, byte[] checksumData)
        {
            FileName = fileName;
            ChecksumAlgorithmId = checksumAlgorithmId;
            ChecksumData = checksumData;
        }

        public string FileName { get; }
        public Guid ChecksumAlgorithmId { get; }
        public byte[] ChecksumData { get; }

		protected override void GenerateInner(CodeGenerator generator)
		{
            generator.Write(TokenType.Directive, "#pragma checksum ");
			generator.Write(TokenType.StringLiteral, '\"');
            generator.Write(TokenType.StringLiteral, FileName);
            generator.Write(TokenType.StringLiteral, '\"');
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.StringLiteral, '\"');
            generator.Write(TokenType.StringLiteral, ChecksumAlgorithmId.ToString("B", CultureInfo.InvariantCulture));
            generator.Write(TokenType.StringLiteral, '\"');
            generator.Write(TokenType.Space, ' ');
            generator.Write(TokenType.StringLiteral, '\"');
            if (ChecksumData != null)
            {
                foreach (byte b in ChecksumData)
                {
                    generator.Write(TokenType.StringLiteral, b.ToString("X2", CultureInfo.InvariantCulture));
                }
            }
            generator.Write(TokenType.StringLiteral, '\"');
		}
	}
}
