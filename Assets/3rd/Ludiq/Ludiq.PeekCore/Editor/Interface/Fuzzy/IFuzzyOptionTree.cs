using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Ludiq.PeekCore
{
	public interface IFuzzyOptionTree
	{
		bool multithreaded { get; }

		GUIContent header { get; }

		bool searchable { get; }

		ICollection<IFuzzyOption> favorites { get; }

		ICollection<object> selected { get; }

		void Prewarm();

		void Rewarm();

		bool prewarmed { get; set; }

		IEnumerable<IFuzzyOption> Root();
		IEnumerable<IFuzzyOption> Children(IFuzzyOption parent);
		IEnumerable<IFuzzyOption> OrderedSearchResults(string query, IFuzzyOption parent, CancellationToken cancellation);
		IEnumerable<ISearchResult<IFuzzyOption>> SearchResults(string query, IFuzzyOption parent, CancellationToken cancellation);
		string SearchResultLabel(IFuzzyOption item, string query);
		bool UseExplicitLabel(IFuzzyOption parent, IFuzzyOption item);
		string ExplicitLabel(IFuzzyOption item);
		bool CanFavorite(IFuzzyOption item);
		void OnFavoritesChange();
	}
}