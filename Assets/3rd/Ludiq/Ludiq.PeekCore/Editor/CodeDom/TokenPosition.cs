using System;

namespace Ludiq.PeekCore.CodeDom
{
	public struct TokenPosition : IEquatable<TokenPosition>, IComparable, IComparable<TokenPosition>
	{
		public TokenPosition(int lineIndex, int columnIndex)
		{
			LineIndex = lineIndex;
			ColumnIndex = columnIndex;
		}

		public int LineIndex;
		public int ColumnIndex;

		public override bool Equals(object other)
		{
			if (other is TokenPosition otherPosition)
			{
				return Equals(otherPosition);
			}
			return false;
		}

		public bool Equals(TokenPosition other)
		{
			return LineIndex == other.LineIndex
				&& ColumnIndex == other.ColumnIndex;
		}

		public override int GetHashCode()
		{
			return LineIndex.GetHashCode() * 23 + ColumnIndex.GetHashCode();
		}

		public int CompareTo(object other)
		{
			if (other is TokenPosition otherPosition)
			{
				return CompareTo(otherPosition);
			}

			throw new ArgumentException();
		}

		public int CompareTo(TokenPosition other)
		{
			var comparison = LineIndex.CompareTo(other.LineIndex);

			if (comparison == 0)
			{
				comparison = ColumnIndex.CompareTo(other.ColumnIndex);
			}

			return comparison;
		}

		public static bool operator ==(TokenPosition left, TokenPosition right) => left.Equals(right);
		public static bool operator !=(TokenPosition left, TokenPosition right) => !left.Equals(right);
		public static bool operator <(TokenPosition left, TokenPosition right) => left.CompareTo(right) < 0;
		public static bool operator <=(TokenPosition left, TokenPosition right) => left.CompareTo(right) <= 0;
		public static bool operator >(TokenPosition left, TokenPosition right) => left.CompareTo(right) > 0;
		public static bool operator >=(TokenPosition left, TokenPosition right) => left.CompareTo(right) >= 0;
	}
}
