namespace Ludiq.PeekCore.CodeDom
{
	public class SyntaxHighlightingTokenEntry
	{
		public SyntaxHighlightingTokenEntry(int rawStartOffset, int rawEndOffset, TokenPosition startPosition, TokenPosition endPosition)
		{
			RawStartOffset = rawStartOffset;
			RawEndOffset = rawEndOffset;
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		public int RawStartOffset;
		public int RawEndOffset;
		public TokenPosition StartPosition;
		public TokenPosition EndPosition;
	}
}
