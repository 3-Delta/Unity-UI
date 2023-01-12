using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public static class SearchUtility
	{
		private static bool Match(string query, string haystack, bool strictOnly, float strictWeight, float looseWeight, out float score, bool[] indices)
		{
			score = 0;
			var isStreaking = false;
			var haystackIndex = 0;
			var matchedAny = false;
			var wordBeginnings = 0;
			var expectsWordBeginning = true;

			if (haystack == null)
			{
				return false;
			}

			for (var queryIndex = 0; queryIndex < query.Length; queryIndex++)
			{
				var queryCharacter = query[queryIndex];

				if (char.IsWhiteSpace(queryCharacter))
				{
					expectsWordBeginning = true;
					continue;
				}
				else if (!expectsWordBeginning)
				{
					expectsWordBeginning = char.IsUpper(queryCharacter);
				}

				var matched = false;
				
				for (; haystackIndex < haystack.Length; haystackIndex++)
				{
					var haystackCharacter = haystack[haystackIndex];
					
					if (char.IsWhiteSpace(haystackCharacter))
					{
						isStreaking = false;
						continue;
					}

					var matchesInvariant = char.ToLowerInvariant(queryCharacter) == char.ToLowerInvariant(haystackCharacter);
					var isWordBeginning = matchesInvariant && StringUtility.IsWordBeginning(haystack, haystackIndex);
					var matchesLoose = matchesInvariant && (isWordBeginning || !expectsWordBeginning || (char.IsUpper(queryCharacter) && queryCharacter == haystackCharacter));
					var matchesStrict = matchesLoose && (isStreaking || isWordBeginning);

					if (isWordBeginning)
					{
						wordBeginnings++;
					}

					var matches = strictOnly ? matchesStrict : matchesLoose;

					if (matches)
					{
						score += matchesStrict ? strictWeight : looseWeight;
						matched = true;

						if (indices != null)
						{
							indices[haystackIndex] = true;
						}

						isStreaking = true;
						haystackIndex++;
						break;
					}
					else
					{
						isStreaking = false;
					}
				}

				if (matched)
				{
					matchedAny = true;
				}
				else
				{
					return false;
				}

				expectsWordBeginning = false;
			}

			if (!matchedAny)
			{
				return false;
			}

			// Bonus to single word queries
			if (wordBeginnings == 1)
			{
				score += strictWeight;
			}

			score /= query.Length;
			return true;
		}

		private static float? Score(string query, string haystack, bool strictOnly, float strictWeight, float looseWeight)
		{
			if (Match(query, haystack, strictOnly, strictWeight, looseWeight, out var score, null))
			{
				return score;
			}
			else
			{
				return null;
			}
		}

		private static bool[] Indices(string query, string haystack, bool strictOnly)
		{
			bool[] indices = new bool[haystack.Length];

			if (Match(query, haystack, strictOnly, 0, 0, out var score, indices))
			{
				return indices;
			}
			else
			{
				return null;
			}
		}

		public static float Relevance(string query, string haystack)
		{
			// Configurable score weights
			var strictWeight = 1;
			var looseWeight = 0.25f;
			var shortnessWeight = 1;

			// Calculate the strict score first, then the loose score if the former fails
			var score = Score(query, haystack, true, strictWeight, looseWeight) ?? Score(query, haystack, false, strictWeight, looseWeight);

			if (score == null)
			{
				// If loose matching fails as well, return a failure
				return -1;
			}

			// Calculate the shortness factor
			var shortness = (float)query.Length / haystack.Length;

			// Sum and normalize scores
			var scoreWeight = Mathf.Max(strictWeight, looseWeight);
			var maxWeight = scoreWeight + shortnessWeight;
			var totalWeight = (score.Value * scoreWeight) + (shortness * shortnessWeight);
			return totalWeight / maxWeight;
		}
		
		public static bool Matches(string query, string haystack)
		{
			return Matches(Relevance(query, haystack));
		}
		
		public static bool Matches(float relevance)
		{
			return relevance > 0;
		}
		
		public static bool Matches(ISearchResult result)
		{
			return Matches(result.relevance);
		}

		public static string HighlightQuery(string haystack, string query, string openTag = "<b>", string closeTag = "</b>")
		{
			if (string.IsNullOrEmpty(query))
			{
				return haystack;
			}

			bool[] matchingIndices = Indices(query, haystack, true) ?? Indices(query, haystack, false);

			if (matchingIndices == null)
			{
				return haystack;
			}

			var sb = new StringBuilder();

			var isHighlighted = false;
			var wasHighlighted = false;

			for (var i = 0; i < haystack.Length; i++)
			{
				var character = haystack[i];
				isHighlighted = matchingIndices[i];

				if (!wasHighlighted && isHighlighted)
				{
					sb.Append(openTag);
				}
				else if (wasHighlighted && !isHighlighted)
				{
					sb.Append(closeTag);
				}

				sb.Append(character);

				wasHighlighted = isHighlighted;
			}

			if (isHighlighted)
			{
				sb.Append(closeTag);
			}

			return sb.ToString();
		}

		public static IEnumerable<ISearchResult<TResult>> OrderableSearchFilter<THaystack, TResult>(this IEnumerable<THaystack> enumeration,
		                                                                                           Func<THaystack, TResult> getResult,
		                                                                                           string query,
		                                                                                           Func<THaystack, string> getHaystack,
			CancellationToken? cancellation = null)
		{
			var results = new ConcurrentBag<ISearchResult<TResult>>();

			var parallelOptions = new ParallelOptions();

			if (cancellation != null)
			{
				parallelOptions.CancellationToken = cancellation.Value;
			}

			parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount / 2;

			var parallelize = true;

			if (parallelize)
			{
				try
				{
					Parallel.ForEach(enumeration, parallelOptions, item =>
					{
						results.Add(new SearchResult<TResult>(getResult(item), Relevance(query, getHaystack(item))));
					});
				}
				catch (OperationCanceledException)
				{
					return Enumerable.Empty<ISearchResult<TResult>>();
				}
			}
			else
			{
				foreach (var item in enumeration)
				{
					results.Add(new SearchResult<TResult>(getResult(item), Relevance(query, getHaystack(item))));
				}
			}

			return results.Where(result => Matches(result.relevance));
		}

		public static IEnumerable<ISearchResult<T>> OrderableSearchFilter<T>(this IEnumerable<T> enumeration,
		                                                                    string query,
		                                                                    Func<T, string> haystack,
			CancellationToken? cancellation = null)
		{
			return OrderableSearchFilter(enumeration, r => r, query, haystack, cancellation);
		}

		public static IEnumerable<T> OrderByRelevance<T>(this IEnumerable<ISearchResult<T>> results)
		{
			return results.OrderByDescending(osr => osr.relevance).Select(osr => osr.result);
		}

		public static IEnumerable<object> OrderByRelevance(this IEnumerable<ISearchResult> results)
		{
			return results.OrderByDescending(osr => osr.relevance).Select(osr => osr.result);
		}

		public static IEnumerable<T> UnorderedSearchFilter<T>(this IEnumerable<T> enumeration, string query, Func<T, string> haystack)
		{
			return enumeration.Where(item => Matches(query, haystack(item)));
		}

		public static IEnumerable<T> OrderedSearchFilter<T>(this IEnumerable<T> enumeration, string query, Func<T, string> haystack, CancellationToken? cancellation = null)
		{
			return enumeration.OrderableSearchFilter(r => r, query, haystack, cancellation).OrderByRelevance();
		}
	}
}
