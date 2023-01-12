using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludiq.PeekCore
{
	public static class StringUtility
	{
		public static string Colored(this string s, Color color)
		{
			return $"<color=#{color.ToHexString()}>{s}</color>";
		}

		public static bool IsNullOrWhiteSpace(string s)
		{
			return s == null || s.Trim() == string.Empty;
		}

		public static string FallbackEmpty(string s, string fallback)
		{
			if (string.IsNullOrEmpty(s))
			{
				s = fallback;
			}

			return s;
		}

		public static string FallbackWhitespace(string s, string fallback)
		{
			if (IsNullOrWhiteSpace(s))
			{
				s = fallback;
			}

			return s;
		}

		public static void AppendLineFormat(this StringBuilder sb, string format, params object[] args)
		{
			sb.AppendFormat(format, args);
			sb.AppendLine();
		}

		public static string ToSeparatedString(this IEnumerable enumerable, string separator)
		{
			return string.Join(separator, enumerable.Cast<object>().Select(o => o?.ToString() ?? "(null)").ToArray());
		}

		public static string ToCommaSeparatedString(this IEnumerable enumerable)
		{
			return ToSeparatedString(enumerable, ", ");
		}

		public static string ToLineSeparatedString(this IEnumerable enumerable)
		{
			return ToSeparatedString(enumerable, Environment.NewLine);
		}

		public static bool ContainsInsensitive(this string haystack, string needle)
		{
			return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static IEnumerable<int> AllIndexesOf(this string haystack, string needle)
		{
			if (string.IsNullOrEmpty(needle))
			{
				yield break;
			}

			for (var index = 0;; index += needle.Length)
			{
				index = haystack.IndexOf(needle, index, StringComparison.OrdinalIgnoreCase);

				if (index == -1)
				{
					break;
				}

				yield return index;
			}
		}

		public static string Filter(this string s, bool letters = true, bool numbers = true, bool whitespace = true, bool symbols = true, bool punctuation = true)
		{
			var sb = new StringBuilder();

			foreach (var c in s)
			{
				if ((!letters && char.IsLetter(c)) ||
				    (!numbers && char.IsNumber(c)) ||
				    (!whitespace && char.IsWhiteSpace(c)) ||
				    (!symbols && char.IsSymbol(c)) ||
				    (!punctuation && char.IsPunctuation(c)))
				{
					continue;
				}

				sb.Append(c);
			}

			return sb.ToString();
		}

		public static string FilterReplace(this string s, char replacement, bool merge, bool letters = true, bool numbers = true, bool whitespace = true, bool symbols = true, bool punctuation = true)
		{
			var sb = new StringBuilder();

			var wasFiltered = true;

			foreach (var c in s)
			{
				if ((!letters && char.IsLetter(c)) ||
				    (!numbers && char.IsNumber(c)) ||
				    (!whitespace && char.IsWhiteSpace(c)) ||
				    (!symbols && char.IsSymbol(c)) ||
				    (!punctuation && char.IsPunctuation(c)))
				{
					if (!merge || !wasFiltered)
					{
						sb.Append(replacement);
					}

					wasFiltered = true;
				}
				else
				{
					sb.Append(c);

					wasFiltered = false;
				}
			}

			return sb.ToString();
		}

		public static string RemoveNonAlphanumeric(this string s)
		{
			return s.Filter(symbols: false, whitespace: false, punctuation: false);
		}

		public static string Prettify(this string s)
		{
			return s.FirstCharacterToUpper().SplitWords(' ');
		}

		public static bool IsWordDelimiter(char c)
		{
			return char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c);
		}

		public static bool IsWordBeginning(char? previous, char current, char? next)
		{
			var isFirst = previous == null;
			var isLast = next == null;

			var isLetter = char.IsLetter(current);
			var wasLetter = previous != null && char.IsLetter(previous.Value);

			var isNumber = char.IsNumber(current);
			var wasNumber = previous != null && char.IsNumber(previous.Value);

			var isUpper = char.IsUpper(current);
			var wasUpper = previous != null && char.IsUpper(previous.Value);

			var isDelimiter = IsWordDelimiter(current);
			var wasDelimiter = previous != null && IsWordDelimiter(previous.Value);

			var willBeLower = next != null && char.IsLower(next.Value);

			return
				(!isDelimiter && isFirst) ||
				(!isDelimiter && wasDelimiter) ||
				(isLetter && wasLetter && isUpper && !wasUpper) || // camelCase => camel_Case
				(isLetter && wasLetter && isUpper && wasUpper && !isLast && willBeLower) || // => ABBRWord => ABBR_Word
				(isNumber && wasLetter) || // Vector3 => Vector_3
				(isLetter && wasNumber && isUpper && willBeLower); // Word1Word => Word_1_Word, Word1word => Word_1word
		}

		public static bool IsWordBeginning(string s, int index)
		{
			Ensure.That(nameof(index)).IsGte(index, 0);
			Ensure.That(nameof(index)).IsLt(index, s.Length);

			var previous = index > 0 ? s[index - 1] : (char?)null;
			var current = s[index];
			var next = index < s.Length - 1 ? s[index + 1] : (char?)null;

			return IsWordBeginning(previous, current, next);
		}

		public static string SplitWords(this string s, char separator)
		{
			var sb = new StringBuilder();

			for (var i = 0; i < s.Length; i++)
			{
				var c = s[i];

				if (i > 0 && IsWordBeginning(s, i))
				{
					sb.Append(separator);
				}

				if (!IsWordDelimiter(c))
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}

		public static string RemoveConsecutiveCharacters(this string s, char c)
		{
			var sb = new StringBuilder();

			var previous = '\0';

			foreach (var current in s)
			{
				if (current != c || current != previous)
				{
					sb.Append(current);
					previous = current;
				}
			}

			return sb.ToString();
		}

		public static string ReplaceMultiple(this string s, HashSet<char> haystacks, char replacement)
		{
			Ensure.That(nameof(haystacks)).IsNotNull(haystacks);

			var sb = new StringBuilder();

			foreach (var current in s)
			{
				if (haystacks.Contains(current))
				{
					sb.Append(replacement);
				}
				else
				{
					sb.Append(current);
				}
			}

			return sb.ToString();
		}

		public static string Truncate(this string value, int maxLength, string suffix = "...")
		{
			return value.Length <= maxLength ? value : value.Substring(0, maxLength) + suffix;
		}

		public static string TrimEnd(this string source, string value)
		{
			if (!source.EndsWith(value))
			{
				return source;
			}

			return source.Remove(source.LastIndexOf(value));
		}

		public static string TrimStart(this string source, string value)
		{
			if (!source.StartsWith(value))
			{
				return source;
			}

			return source.Substring(value.Length);
		}

		public static string FirstCharacterToLower(this string s)
		{
			if (string.IsNullOrEmpty(s) || char.IsLower(s, 0))
			{
				return s;
			}

			return char.ToLowerInvariant(s[0]) + s.Substring(1);
		}

		public static string FirstCharacterToUpper(this string s)
		{
			if (string.IsNullOrEmpty(s) || char.IsUpper(s, 0))
			{
				return s;
			}

			return char.ToUpperInvariant(s[0]) + s.Substring(1);
		}

		public static string PartBefore(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

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

		public static string PartBeforeLast(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			var index = s.LastIndexOf(c);

			if (index > 0)
			{
				return s.Substring(0, index);
			}
			else
			{
				return s;
			}
		}

		public static string PartAfter(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			var index = s.IndexOf(c);

			if (index > 0)
			{
				return s.Substring(index + 1);
			}
			else
			{
				return s;
			}
		}

		public static string PartAfterLast(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			var index = s.LastIndexOf(c);

			if (index > 0)
			{
				return s.Substring(index + 1);
			}
			else
			{
				return s;
			}
		}

		public static void PartsAround(this string s, char c, out string before, out string after)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			var index = s.IndexOf(c);

			if (index > 0)
			{
				before = s.Substring(0, index);
				after = s.Substring(index + 1);
			}
			else
			{
				before = s;
				after = null;
			}
		}

		public static int FirstIndexOf(this string s, params char[] cs)
		{
			var index = int.MaxValue;

			foreach (var c in cs)
			{
				index = Math.Min(index, s.FirstIndexOf(c));
			}

			return index;
		}

		public static int LastIndexOf(this string s, params char[] cs)
		{
			var index = -1;

			foreach (var c in cs)
			{
				index = Math.Max(index, s.LastIndexOf(c));
			}

			return index;
		}

		// Faster equivalents for chars

		public static bool EndsWith(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			if (s.Length == 0)
			{
				return false;
			}

			return s[s.Length - 1] == c;
		}

		public static bool StartsWith(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			if (s.Length == 0)
			{
				return false;
			}

			return s[0] == c;
		}

		public static bool Contains(this string s, char c)
		{
			Ensure.That(nameof(s)).IsNotNull(s);

			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == c)
				{
					return true;
				}
			}

			return false;
		}

		public static string NullIfEmpty(this string s)
		{
			if (s == string.Empty)
			{
				return null;
			}
			else
			{
				return s;
			}
		}

		public static string ToBinaryString(this int value)
		{
			return Convert.ToString(value, 2).PadLeft(8, '0');
		}

		public static string ToBinaryString(this long value)
		{
			return Convert.ToString(value, 2).PadLeft(16, '0');
		}

		public static string ToBinaryString(this Enum value)
		{
			return Convert.ToString(Convert.ToInt64(value), 2).PadLeft(16, '0');
		}

		public static int CountIndices(this string s, char c)
		{
			var count = 0;

			foreach (var _c in s)
			{
				if (c == _c)
				{
					count++;
				}
			}

			return count;
		}

		public static bool IsGuid(string value)
		{
			return guidRegex.IsMatch(value);
		}

		private static readonly Regex guidRegex = new Regex(@"[a-fA-F0-9]{8}(\-[a-fA-F0-9]{4}){3}\-[a-fA-F0-9]{12}");

		public static string PathEllipsis(string s, int maxLength)
		{
			var ellipsis = "...";

			if (s.Length < maxLength)
			{
				return s;
			}

			var fileName = Path.GetFileName(s);
			var directory = Path.GetDirectoryName(s);

			var maxDirectoryLength = maxLength - fileName.Length - ellipsis.Length;

			if (maxDirectoryLength > 0)
			{
				return directory.Substring(0, maxDirectoryLength) + ellipsis + Path.DirectorySeparatorChar + fileName;
			}
			else
			{
				return ellipsis + Path.DirectorySeparatorChar + fileName;
			}
		}
		
		public static string UniqueName(string originalName, IEnumerable<string> existingNames)
		{
			var collection = existingNames.ToHashSetPooled();
			var result = UniqueName(originalName, collection);
			collection.Free();
			return result;
		}

		public static string UniqueName(string originalName, ICollection<string> existingNames)
		{
			var uniqueName = originalName;

			if (existingNames.Contains(uniqueName))
			{
				int suffix = 2;

				do
				{
					uniqueName = originalName + " " + suffix;
					suffix++;
				} while (existingNames.Contains(uniqueName));
			}

			return uniqueName;
		}

		// https://stackoverflow.com/a/3961365/154502
		public static string WordWrap(string s, int width)
		{
			int pos, next;
			var sb = new StringBuilder();
			var _newline = Environment.NewLine;

			// Lucidity check
			if (width < 1)
			{
				return s;
			}
			
			// Parse each line of text
			for (pos = 0; pos < s.Length; pos = next)
			{
				// Find end of line
				int eol = s.IndexOf(_newline, pos);

				if (eol == -1)
					next = eol = s.Length;
				else
					next = eol + _newline.Length;

				// Copy this line of text, breaking into smaller lines as needed
				if (eol > pos)
				{
					do
					{
						int len = eol - pos;

						if (len > width)
							len = BreakLine(s, pos, width);

						sb.Append(s, pos, len);
						sb.Append(_newline);

						// Trim whitespace following break
						pos += len;

						while (pos < eol && char.IsWhiteSpace(s[pos]))
							pos++;
					} while (eol > pos);
				}
				else sb.Append(_newline); // Empty line
			}

			return sb.ToString();
		}
		
		public static int BreakLine(string text, int pos, int max)
		{
			// Find last whitespace in line
			int i = max - 1;
			while (i >= 0 && !char.IsWhiteSpace(text[pos + i]))
				i--;
			if (i < 0)
				return max; // No whitespace found; break at maximum length
			// Find start of whitespace
			while (i >= 0 && char.IsWhiteSpace(text[pos + i]))
				i--;
			// Return length of text before whitespace
			return i + 1;
		}
	}
}