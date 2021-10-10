
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unity.DebugWatch.WatchContext
{
    [UnityEditor.InitializeOnLoad]
    public static class ParserUtils
    {

        public static void TrimHead(this ref RangeInt r, int length)
        {
            r.start += length;
            r.length -= length;
        }
        public static void TrimTail(this ref RangeInt r, int length)
        {
            r.length -= length;
        }
        public static string Substring(this string str, RangeInt range)
        {
            if (range.length == str.Length) return str;
            return str.Substring(range.start, range.length);
        }
        public static RangeInt Range(this string str)
        {
            return new RangeInt(0, str.Length);
        }
        public static bool StartsWithAt(this string str, int index, string value)
        {
            if (index == 0) return str.StartsWith(value);
            if (index > str.Length) return false;
            if (index + value.Length > str.Length) return false;
            // TODO Should find another way that does not involve creating a new string
            return str.Substring(index, value.Length).StartsWith(value);
        }
        // index is past-the-end
        public static bool EndsWithAt(this string str, int index, string value)
        {
            if (index == str.Length) return str.EndsWith(value);
            if (index > str.Length) return false;
            if (index < value.Length) return false;
            // TODO Should find another way that does not involve creating a new string
            return str.Substring(index - value.Length, value.Length).EndsWith(value);
        }
        public static string JoinPath(string path0, string path1)
        {
            return path0 + MakePathRelative(path1);
        }
        public static string MakePathRelative(string path)
        {
            if (IsIndexerAt(path, 0) || IsScopeReferenceAt(path, 0)) return path;
            return "." + path;

        }
        //public static bool StringEqualAt(string str, int cursor, string token)
        //{
        //    string strLocal;
        //    if (cursor != 0)
        //    {
        //        if (cursor > str.Length) return false;
        //        // TODO Should find another way that does not involve creating a new string
        //        strLocal = str.Substring(cursor);
        //    }
        //    else
        //    {
        //        strLocal = str;
        //    }
        //    return strLocal.StartsWith(token);
        //}
        public static bool TryParseAt(string str, ref RangeInt cursor, string token)
        {
            if (str.StartsWithAt(cursor.start, token))
            {
                cursor.TrimHead(token.Length);
                return true;
            }
            return false;
        }
        public static bool TryParseAtEnd(string str, ref RangeInt cursor, string token)
        {
            if (str.EndsWithAt(cursor.end, token))
            {
                cursor.TrimTail(token.Length);
                return true;
            }
            return false;
        }
        public static bool TryParseIntAt(string str, ref RangeInt cursor, out int value)
        {
            int i = 0;
            switch (str[cursor.start])
            {
                case '-':
                case '+':
                    ++i;
                    break;
            }
            while (cursor.start + i < str.Length
                && i < cursor.length
                && Char.IsDigit(str[cursor.start + i]))
            {
                ++i;
            }
            if (i == 0)
            {
                value = default;
                return false;
            }
            var ss = str.Substring(cursor.start, i);
            if (int.TryParse(ss, out value))
            {
                cursor.TrimHead(i);
                return true;
            }
            return false;
        }
        public static bool IsSeparator(string str, int cursor)
        {
            if (cursor >= str.Length) return true;
            if (IsIdentifierCharAt(str, cursor)) return false;
            return true;
        }
        public static bool IsIndexerAt(string str, int cursor)
        {
            return str[cursor] == '[';
        }
        public static bool IsScopeReferenceAt(string str, int cursor)
        {
            return str[cursor] == '.';
        }
        public static bool IsIdentifierFirstCharAt(string str, int cursor)
        {
            return Char.IsLetter(str[cursor]) || str[cursor] == '_';
        }
        public static bool IsIdentifierCharAt(string str, int cursor)
        {
            return Char.IsLetterOrDigit(str[cursor]) || str[cursor] == '_';
        }
        // str at cursor must be == token followed by a separator
        public static bool TryParseToken(string str, ref RangeInt cursor, string token)
        {
            if (str.StartsWithAt(cursor.start, token))
            {
                int c = cursor.start + token.Length;
                if (IsSeparator(str, c))
                {
                    cursor.TrimHead(token.Length);
                    return true;
                }
            }
            return false;
        }

        // parse string until hits a separator
        public static bool TryParseIdentifier(string str, ref RangeInt cursor, out string identifier)
        {
            if (TryParseIdentifierRange(str, ref cursor, out var range))
            {
                identifier = str.Substring(range.start, range.length);
                return true;
            }
            identifier = default;
            return false;
        }
        public static bool TryParseIdentifierRange(string str, ref RangeInt cursor, out RangeInt identifierRange)
        {
            int consumed = 0;
            while (consumed < cursor.length
                && !IsSeparator(str, cursor.start + consumed))
            {
                ++consumed;
            }
            if (consumed > 0)
            {
                identifierRange = new RangeInt(cursor.start, consumed);
                cursor = new RangeInt(cursor.start + consumed, cursor.length - consumed);
                return true;
            }
            identifierRange = default;
            return false;
        }
        public static bool TryParseIdentifierRangeEnd(string str, ref RangeInt cursor, out RangeInt identifierRange)
        {
            int consumed = 0;
            while (consumed < cursor.length
                && !IsSeparator(str, cursor.end - 1 - consumed))
            {
                ++consumed;
            }
            if (consumed > 0)
            {
                identifierRange = new RangeInt(cursor.end - 1 - consumed, consumed);
                cursor = new RangeInt(cursor.start, cursor.length - consumed);
                return true;
            }
            identifierRange = default;
            return false;
        }
        public static bool TryParseSequence(string source, ref RangeInt cursor, char endCharacter, out string sequence, bool canEscape)
        {
            if (TryParseSequenceRange(source, ref cursor, endCharacter, out var sequenceRange, canEscape))
            {
                sequence = source.Substring(sequenceRange.start, sequenceRange.length);
                return true;
            }
            sequence = default;
            return false;
        }
        public static bool TryParseSequenceRange(string source, ref RangeInt cursor, char endCharacter, out RangeInt sequenceRange, bool canEscape)
        {
            int consumed = 0;
            while (consumed < cursor.length)
            {
                if (source[cursor.start + consumed] == endCharacter)
                {
                    sequenceRange = new RangeInt(cursor.start, consumed);
                    ++consumed;
                    cursor.TrimHead(consumed);
                    return true;
                }
                else if (canEscape && source[cursor.start + consumed] == '\\')
                {
                    ++consumed;
                }
                ++consumed;
            }
            sequenceRange = default;
            return false;
        }
        public class ScopeDelimiter
        {
            public string Begin;
            public string End;
            public bool CanEscape;
            public List<ScopeDelimiter> SubDelemiters = new List<ScopeDelimiter>();
        }
        public static ScopeDelimiter DelemiterQuote = new ScopeDelimiter() { Begin = "\"", End = "\"", CanEscape = true };
        public static ScopeDelimiter DelemiterSquareBracket = new ScopeDelimiter() { Begin = "[", End = "]", CanEscape = false };
        public static List<ScopeDelimiter> ScopeDelimiters = new List<ScopeDelimiter>()
        {
            DelemiterQuote,
            DelemiterSquareBracket
        };


        static ParserUtils()
        {
            DelemiterSquareBracket.SubDelemiters = ScopeDelimiters;
        }
        public static string Escape(string str)
        {
            var sb = new StringBuilder(str.Length);
            foreach (var c in str)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append(@"\\"); break;
                    case '\a': sb.Append(@"\a"); break;
                    case '\b': sb.Append(@"\b"); break;
                    case '\f': sb.Append(@"\f"); break;
                    case '\n': sb.Append(@"\n"); break;
                    case '\r': sb.Append(@"\r"); break;
                    case '\t': sb.Append(@"\t"); break;
                    case '\v': sb.Append(@"\v"); break;
                    case '\0': sb.Append(@"\0"); break;
                    default: sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
        public static bool TryParseScopeDelimiterBegin(string str, ref RangeInt cursor, bool hadEscape, List<ScopeDelimiter> delemiters, out ScopeDelimiter delemiter)
        {
            var c = cursor;
            foreach (var sd in delemiters)
            {
                if (TryParseAt(str, ref c, sd.Begin))
                {
                    if (!sd.CanEscape || !hadEscape)
                    {
                        cursor = c;
                        delemiter = sd;
                        return true;
                    }
                }
            }
            delemiter = null;
            return false;
        }
        public static bool TryParseScopeDelimiterEnd(string str, ref RangeInt cursor, bool hadEscape, List<ScopeDelimiter> delemiters, out ScopeDelimiter delemiter)
        {
            var c = cursor;
            foreach (var sd in delemiters)
            {
                if (TryParseAtEnd(str, ref c, sd.End))
                {
                    if (!sd.CanEscape || !hadEscape)
                    {
                        cursor = c;
                        delemiter = sd;
                        return true;
                    }
                }
            }
            delemiter = null;
            return false;
        }
        //public static bool TryParseScopeDelimiter(string str, ref int cursor, out ScopeDelimiter delemiter, out string sequence)
        //{
        //    var c = cursor;

        //    var sequenceBegin = c;
        //    var delStack = new Stack<ScopeDelimiter>();

        //    var l = str.Length;
        //    while (c < l)
        //    {
        //        // check for end current scope
        //        if (delStack.Count > 0)
        //        {
        //            if (TryParseAt(str, ref c, delStack.Peek().End))
        //            {
        //                var sdEnd = delStack.Pop();
        //                if (delStack.Count == 0)
        //                {
        //                    // no more scope to parse, TryParseScopeSequence is successful
        //                    sequence = str.Substring(sequenceBegin, c - sdEnd.End.Length - sequenceBegin);
        //                    cursor = c;
        //                    return true;
        //                }
        //                // c has advanced passed the delemiter end
        //                continue;
        //            }
        //        }

        //        // check for begin new scope
        //        if (TryParseScopeDelimiterBegin(str, ref c, out var sdBegin))
        //        {
        //            delStack.Push(sdBegin);
        //            // c has advanced passed the delemiter begin
        //            continue;
        //        }

        //        // nothing special at c, advance to next char
        //        ++c;
        //    }
        //}
        //public static bool TryParseScopeSequence(string str, ref RangeInt cursor, ScopeDelimiter delimiter, out string sequence)
        //{
        //    if (TryParseScopeSequenceRange(str, ref cursor, delimiter, out var range))
        //    {
        //        sequence = str.Substring(range.start, range.length);
        //        return true;
        //    }
        //    sequence = default;
        //    return false;
        //}
        public static bool TryParseScopeSequenceRange(string str, ref RangeInt cursor, ScopeDelimiter delimiter, out RangeInt range, out RangeInt rangerWithDelimiters)
        {
            rangerWithDelimiters = new RangeInt();
            var c = cursor;
            if (TryParseAt(str, ref c, delimiter.Begin))
            {
                int sequenceBegin = c.start;
                rangerWithDelimiters.start = cursor.start;
                var delStack = new Stack<ScopeDelimiter>();
                delStack.Push(delimiter);
                List<ScopeDelimiter> currentDelimiters = delimiter.SubDelemiters;
                bool hadEscape = false;
                while (c.length > 0)
                {
                    // check for end current scope
                    if (delStack.Count > 0)
                    {
                        var sdEnd = delStack.Peek();
                        Debug.Assert(sdEnd.SubDelemiters == currentDelimiters);
                        if (TryParseAt(str, ref c, sdEnd.End))
                        {
                            if (!sdEnd.CanEscape || !hadEscape)
                            {
                                delStack.Pop();

                                if (delStack.Count == 0)
                                {
                                    // no more scope to parse, TryParseScopeSequence is successful
                                    rangerWithDelimiters.length = c.start - rangerWithDelimiters.start;
                                    range = new RangeInt(sequenceBegin, c.start - sdEnd.End.Length - sequenceBegin);

                                    cursor = c;
                                    return true;
                                }
                                currentDelimiters = delStack.Peek().SubDelemiters;
                                hadEscape = false;
                                // c has advanced passed the delemiter end
                                continue;
                            }
                        }
                    }

                    // check for begin new scope
                    if (TryParseScopeDelimiterBegin(str, ref c, hadEscape, currentDelimiters, out var sdBegin))
                    {
                        delStack.Push(sdBegin);
                        currentDelimiters = sdBegin.SubDelemiters;
                        hadEscape = false;
                        // c has advanced passed the delemiter begin
                        continue;
                    }

                    hadEscape = (str[c.start] == '\\');

                    // nothing special at c, advance to next char
                    c.TrimHead(1);
                }
            }
            range = default;
            return false;
        }

        public static bool TryParseScopeSequenceRangeReverse(string str, ref RangeInt cursor, ScopeDelimiter delimiter, out RangeInt range, out RangeInt rangerWithDelimiters)
        {
            rangerWithDelimiters = new RangeInt();
            var c = cursor;
            if (TryParseAtEnd(str, ref c, delimiter.End))
            {
                int rangeEnd = c.end;
                int rangeEndWithDelemiter = cursor.end;
                var delStack = new Stack<ScopeDelimiter>();
                delStack.Push(delimiter);
                List<ScopeDelimiter> currentDelimiters = delimiter.SubDelemiters;
                bool hadEscape = false;
                while (c.length > 0)
                {
                    // check for end current scope
                    if (delStack.Count > 0)
                    {
                        var sdBegin = delStack.Peek();
                        Debug.Assert(sdBegin.SubDelemiters == currentDelimiters);
                        if (TryParseAtEnd(str, ref c, sdBegin.Begin))
                        {
                            if (!sdBegin.CanEscape || !hadEscape)
                            {
                                delStack.Pop();

                                if (delStack.Count == 0)
                                {
                                    // no more scope to parse, TryParseScopeSequence is successful
                                    rangerWithDelimiters = new RangeInt(c.end, rangeEndWithDelemiter - c.end);
                                    var rangeBegin = c.end + sdBegin.Begin.Length;
                                    range = new RangeInt(rangeBegin, rangeEnd - rangeBegin);

                                    cursor = c;
                                    return true;
                                }
                                currentDelimiters = delStack.Peek().SubDelemiters;
                                hadEscape = false;
                                // c has advanced passed the delemiter end
                                continue;
                            }
                        }
                    }

                    // check for begin new scope
                    if (TryParseScopeDelimiterEnd(str, ref c, hadEscape, currentDelimiters, out var sdEnd))
                    {
                        delStack.Push(sdEnd);
                        currentDelimiters = sdEnd.SubDelemiters;
                        hadEscape = false;
                        // c has advanced passed the delemiter begin
                        continue;
                    }

                    hadEscape = (str[c.end-1] == '\\');

                    // nothing special at c, advance to next char
                    c.TrimTail(1);
                }
            }
            range = default;
            return false;
        }
        public static bool TryParseIndexerInt(string str, ref RangeInt cursor, out int index, out RangeInt indexRange, out RangeInt indexerRange)
        {
            var c = cursor;
            if (TryParseScopeSequenceRange(str, ref c, DelemiterSquareBracket, out indexRange, out indexerRange))
            {
                return TryParseIntAt(str, ref indexRange, out index);
            }
            index = default;
            return false;
        }

        // cursor will be set to part before last part. Will not include trailing '.'
        // part will be the last part of the path. will include leading '.' or indexer '[...]'
        public static bool TryExtractLastPathPart(string path, ref RangeInt cursor, out RangeInt part)
        {
            if (cursor.start >= path.Length || cursor.length == 0)
            {
                part = default;
                return false;
            }
            var c = cursor;
            if (path[c.end - 1] == ']')
            {
                if (TryParseScopeSequenceRangeReverse(path, ref c, DelemiterSquareBracket, out var partIn, out part))
                {
                    cursor = c;
                    return true;
                }
                else
                {
                    part = default;
                    return false;
                }
            }
            else if (path[c.end-1] == '.')
            {
                c.TrimTail(1);
            }
            if (TryParseIdentifierRangeEnd(path, ref c, out part))
            {
                if (c.length > 0 && path[c.end - 1] == '.')
                {
                    c.TrimTail(1);
                }
                cursor = c;
                return true;
            }
            part = default;
            return false;
        }
        public static bool TryExtractPathPart(string path, ref RangeInt cursor, out RangeInt part)
        {
            if (cursor.start >= path.Length || cursor.length == 0)
            {
                part = default;
                return false;
            }
            var c = cursor;
            if (path[c.start] == '[')
            {
                if (TryParseScopeSequenceRange(path, ref c, DelemiterSquareBracket, out var partIn, out part))
                {
                    cursor = c;
                    return true;
                }
                else
                {
                    part = default;
                    return false;
                }
            }
            else if (path[c.start] == '.')
            {
                c.TrimHead(1);
            }
            if (TryParseIdentifierRange(path, ref c, out part))
            {
                cursor = c;
                return true;
            }
            part = default;
            return false;
        }

        public static bool TryExtractPropertyPathPart(string path, ref RangeInt cursor, out RangeInt part, out RangeInt partIdentifierOnly)
        {
            if (cursor.start >= path.Length || cursor.length == 0)
            {
                part = default;
                partIdentifierOnly = default;
                return false;
            }
            var c = cursor;
            if (path[c.start] == '.')
            {
                c.TrimHead(1);
            }
            bool hasIdentifier = TryParseIdentifierRange(path, ref c, out partIdentifierOnly);
            if (c.start < path.Length && path[c.start] == '[')
            {
                if (TryParseScopeSequenceRange(path, ref c, DelemiterSquareBracket, out var partIn, out var partIndexer))
                {
                    if (hasIdentifier)
                    {
                        part = new RangeInt(partIdentifierOnly.start, partIndexer.end - partIdentifierOnly.start);
                    }
                    else
                    {
                        part = partIndexer;
                    }
                    cursor = c;
                    return true;
                }
            }
            if (hasIdentifier)
            {
                part = partIdentifierOnly;
                cursor = c;
                return true;
            }
            part = default;
            return false;
        }
    }


}