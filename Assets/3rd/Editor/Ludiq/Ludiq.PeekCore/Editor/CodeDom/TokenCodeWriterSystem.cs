using System.Collections.Generic;

namespace Ludiq.PeekCore.CodeDom
{
	public sealed class TokenCodeWriterSystem : CodeWriterSystem<TokenCodeWriter>
	{
		private readonly Dictionary<string, TokenCodeWriter> writers = new Dictionary<string, TokenCodeWriter>();

		public override TokenCodeWriter OpenWriter(string className)
		{
			var writer = new TokenCodeWriter();
			writers.Add(className, writer);
			return writer;
		}

		public List<Token> GetTokens(string className)
		{
			return writers[className].tokens;
		}

		public Dictionary<CodeElement, List<Token>> GetTokensByElements(string className)
		{
			return writers[className].tokensByElements;
		}
	}
}
